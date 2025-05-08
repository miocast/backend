using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using backend.Contracts;
using backend.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using System.IO;

namespace backend.Controllers
{

    [ApiController]
    [Route("api/v1/Npa")]
    //[Authorize]
    public class NpaController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private string ApiUrl { get; init; }

        public NpaController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            this.ApiUrl = configuration["AppSettings:ApiUrl"] ?? string.Empty;
        }

        [HttpPost("store-from-worker")]
        public async Task<IActionResult> StoreDocumentsFromWorkerAsync([FromBody, Required] List<NpaDocument> documents)
        {
            try
            {
                var docsModels = documents.Select(doc => new NpaDocument
                {
                    Id = doc.Id,
                    FilePath = doc.FilePath,
                    Name = doc.Name,
                    Type = doc.Type,
                });

                var ids = documents.Select(doc => doc.Id).ToList();

                var existItems = await _dbContext.NpaDocuments.Where(doc => ids.Contains(doc.Id)).Select(doc => doc.Id).ToListAsync();

                if (existItems is not null && existItems.Count > 0)
                {
                    docsModels = docsModels.Where(doc => !existItems.Contains(doc.Id));
                }


                await _dbContext.NpaDocuments.AddRangeAsync(docsModels);
                await _dbContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }

        [HttpGet("/api/v1/documents/npa")]
        [ProducesResponseType(200, Type = typeof(List<NpaKnowlabe>))]
        public async Task<IActionResult> GetNpasAsync()
        {
            try
            {
                var npasModels = await _dbContext.NpaDocuments.ToListAsync();

                if (npasModels is null || npasModels.Count == 0)
                {
                    return new OkObjectResult(new List<NpaKnowlabe>());
                }

                var npas = npasModels.Select(np => new NpaKnowlabe
                {
                    Id = np.Id,
                    FilePath = np.FilePath,
                    LastUpdated = DateTime.UtcNow,
                    Title = np.Name
                }).ToList();

                return new OkObjectResult(npas);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }

        public class ParseQuery
        {
            public string Query { get; set; }
        }

        [HttpPost("/api/v1/documents/parse-by-theme")]
        public async Task<IActionResult> AddNewTheme([FromBody] ParseQuery query)
        {
            using (var client = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {
                    // TODO: �� ����� ����� api ������ ������. ����� ������� ��� ����� ��� ����������
                    HttpResponseMessage response = await client.PostAsync($"{this.ApiUrl}/api/v1/documets/parse-by-theme?theme={query.Query}", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                    }
                }
            }

            return new OkResult();
        }

    }
}
