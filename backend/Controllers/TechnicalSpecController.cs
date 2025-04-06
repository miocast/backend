using backend.Contracts;
using backend.DAL;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

//using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/v1/TechnicalSpecification")]
    //[Authorize]
    public class TechnicalSpecController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private string ApiUrl { get; init; }

        public TechnicalSpecController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            this.ApiUrl = configuration["AppSettings:ApiUrl"] ?? string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile fileStream, string userId, CancellationToken cts)
        {
            //var url = "{host}/v1/documents/tz-check"; 
            try
            {
                if (fileStream == null || fileStream.Length == 0)
                {
                    return BadRequest();
                }

                //var path = $"./Storage/{fileStream.Name}";
                var path = $"./Storage/{fileStream.FileName}";

                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await fileStream.CopyToAsync(stream, cts);
                }

                var technicalSpec = new TechnicalSpec(userId, fileStream.FileName, path);

                await _dbContext.TechnicalSpecs.AddAsync(technicalSpec, cts);
                await _dbContext.SaveChangesAsync(cts);

                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        var fileContent = new StreamContent(fileStream.OpenReadStream());
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(fileStream.ContentType);
                        content.Add(fileContent, "document", fileStream.FileName);

                        // TODO: Из конфы брать api ссылку питона. СРАЗУ СДЕЛАТЬ КАК УВИЖУ ЧТО ЗАРАБОТАЛО
                        HttpResponseMessage response = await client.PostAsync($"{this.ApiUrl}/api/v1/documents/tz-check?document_id={technicalSpec.Id}", content);

                        if (!response.IsSuccessStatusCode)
                        {
                            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                        }
                    }
                }

                return Ok(technicalSpec.Id);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(GetTechnicalSpecResponse))]
        public async Task<IActionResult> Get([FromQuery] GetTechnicalSpecRequest request, CancellationToken cts)
        {
            int page = request.Page > 0 ? request.Page : 1;
            int size = request.Size > 0 ? request.Size : 10;

            var techSpecsQuery = _dbContext.TechnicalSpecs
                .Where(ts => string.IsNullOrWhiteSpace(request.Search) ||
               ts.Name.ToLower().Contains(request.Search.ToLower()));

            var totalCount = await techSpecsQuery.CountAsync(cts);

            var techSpecsDtos = await techSpecsQuery
                .Skip((page - 1) * size)
                .Take(size)
                .Select(ts => new TechnicalSpecDto(ts.Id, ts.UserId, ts.Name, ts.Link, ts.Description, ts.LastUpdate, ts.Category, ts.Status))
                .ToListAsync(cts);

            return Ok(new GetTechnicalSpecResponse(techSpecsDtos));
        }

        // GET "api/v1/technical-spec/{guid}
        // возвращает контент тз по id 
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(TechnicalSpecDto))]
        public async Task<IActionResult> GetById(string id)
        {
            if (!string.IsNullOrEmpty(id) && Guid.TryParse(id, out Guid guidId))
            {
                TechnicalSpec? technicalSpec = await _dbContext.TechnicalSpecs.FirstOrDefaultAsync(ts => ts.Id == guidId);

                var filePath = technicalSpec.Link;
                FileContentResult fileContentResult;

                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileContent = await System.IO.File.ReadAllBytesAsync(filePath);
                    fileContentResult = File(fileContent, "application/octet-stream", Path.GetFileName(filePath));

                    var response = new TechnicalSpecDto(technicalSpec.Id, technicalSpec.UserId, technicalSpec.Name, technicalSpec.Link, technicalSpec.Description, technicalSpec.LastUpdate, technicalSpec.Category, technicalSpec.Status);

                    return Ok(response);
                }
                else
                {
                    return NotFound();
                }
            }
            return NotFound();
        }

        [HttpGet("{id}/analys")]
        [ProducesResponseType(200, Type = typeof(DocumentAnalysBusiness))]
        public async Task<IActionResult> GetAnalysById([FromRoute] string id)
        {
            var analyses = await _dbContext.DocumentAnalys.Where(doc => doc.DocumentId == id).ToListAsync();

            if (analyses is null || analyses.Count == 0)
            {
                return new OkObjectResult(new List<Analys>());
            }

          
            var npas = await _dbContext.DocumentNpas.Where(doc => doc.DocumentId == id).ToListAsync();

            var docBusiness = new DocumentAnalysBusiness
            {
                DocumentId = id,
                Analyses = analyses.Select(anal => new Analys
                {
                    Explanation = anal.Explanation,
                    Id = anal.Id,
                    Regulation = anal.Regulation,
                    Text = anal.Text
                }).ToList(),
                Npas = npas.Select(np => new NpaBusiness
                {
                    DistancePercent = np.DistancePercent,
                    Source = np.Source
                }).ToList()
            };

            return new OkObjectResult(docBusiness);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            if (!string.IsNullOrEmpty(id) && Guid.TryParse(id, out Guid guidId))
            {
                TechnicalSpec? technicalSpec = await _dbContext.TechnicalSpecs.FirstOrDefaultAsync(ts => ts.Id == guidId);
                if (technicalSpec != null)
                {
                    _dbContext.TechnicalSpecs.Remove(technicalSpec);
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                }
            }
            return NotFound();
        }

        //GET "api/v1/technilac-spec/{guid}/file

        [HttpPost("{id}/file")] 
        public async Task<IActionResult> Download(string id)
        {
            if (!string.IsNullOrEmpty(id) && Guid.TryParse(id, out Guid guidId))
            {
                TechnicalSpec? technicalSpec = await _dbContext.TechnicalSpecs.FirstOrDefaultAsync(ts => ts.Id == guidId);

                if (technicalSpec != null)
                {
                    var filePath = technicalSpec.Link;

                    if (System.IO.File.Exists(filePath))
                    {
                        byte[] fileContent = await System.IO.File.ReadAllBytesAsync(filePath);
                        return File(fileContent, "application/octet-stream", Path.GetFileName(filePath));
                    }
                    else
                    {
                        return NotFound("Файл не найден.");
                    }
                }
                else
                {
                    return NotFound("Техническая спецификация не найдена.");
                }
            }
            return BadRequest("Неверный идентификатор.");

        }

        [HttpPost("process-from-worker")]
        public async Task<IActionResult> ProcessFromWorker([Required, FromBody] CompleteAnalys analys)
        {
            var document = await _dbContext.TechnicalSpecs.FirstOrDefaultAsync(doc => doc.Id.ToString() == analys.DocumentId);

            if (document is null)
            {
                return new NotFoundResult();
            }

            document.Status = Enums.TechStatus.OutWork;

            var newDocAnalyses = analys.Analyses.Select(an => new DocumentAnalys
            {
                Id = Guid.NewGuid().ToString(),
                DocumentId = document.Id.ToString(),
                Explanation = an.Explanation,
                Regulation = an.Regulation,
                Text = an.Text
            }).ToList();

            await _dbContext.DocumentAnalys.AddRangeAsync(newDocAnalyses);

            List<DocumentNpa> npaDoc = new List<DocumentNpa>();

            for (int i = 0; i < analys.Npa.Npas.Count(); i++)
            {
                npaDoc.Add(new DocumentNpa
                {
                    Id = Guid.NewGuid().ToString(),
                    NpaText = analys.Npa.Npas[i],
                    Source = analys.Npa.Sources[i],
                    DistancePercent = analys.Npa.Distances[i] * 100,
                    DocumentId = analys.DocumentId
                });
            }

            await _dbContext.DocumentNpas.AddRangeAsync(npaDoc);

            await _dbContext.SaveChangesAsync();

            return new OkResult();
        }

    }
}
