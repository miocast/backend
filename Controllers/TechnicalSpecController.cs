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
    public class TechnicalSpecController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public TechnicalSpecController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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

                //var directory = Path.GetDirectoryName(path);
                //if (!Directory.Exists(directory))
                //{
                //    Directory.CreateDirectory(directory);
                //}

                //using (var stream = new FileStream(path, FileMode.Create))
                //{
                //    await fileStream.CopyToAsync(stream, cts);
                //}

                var technicalSpec = new TechnicalSpec(userId, fileStream.FileName, path);

                await _dbContext.TechnicalSpecs.AddAsync(technicalSpec, cts);
                await _dbContext.SaveChangesAsync(cts);

                // to-do: send on python server
                //using (var client = new HttpClient())
                //{
                //    using (var content = new MultipartFormDataContent())
                //    {
                //        var fileContent = new StreamContent(fileStream.OpenReadStream());
                //        fileContent.Headers.ContentType = new MediaTypeHeaderValue(fileStream.ContentType);
                //        content.Add(fileContent, "fileStream", fileStream.FileName);

                //        HttpResponseMessage response = await client.PostAsync(url, content);

                //        if (!response.IsSuccessStatusCode)
                //        {
                //            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                //        }
                //    }
                //}

                return Ok(technicalSpec.Id);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
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
                .Select(ts => new TechnicalSpecDto(ts.Id, ts.UserId, ts.Name, ts.Link))
                .ToListAsync(cts);

            return Ok(new GetTechnicalSpecResponse(techSpecsDtos));
        }

        [HttpGet("{id}")]
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

                    var response = new
                    {
                        technicalSpec.Id,
                        technicalSpec.UserId,
                        technicalSpec.Name,
                        Content = fileContent
                    };

                    return Ok(response);
                }
                else
                {
                    return NotFound();
                }
            }
            return NotFound();
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

    }
}
