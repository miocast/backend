using backend.Contracts;
using backend.DAL;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace backend.Controllers
{
    [ApiController]
    [Route("v1/TechnicalSpecController")]
    public class TechnicalSpecController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public TechnicalSpecController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("/Upload")]
        public async Task<IActionResult> UploadFile(IFormFile fileStream, string userId, CancellationToken cts)
        {
            //var url = "{host}/v1/documents/tz-check";
            try
            {
                if (fileStream == null || fileStream.Length == 0)
                {
                    return BadRequest();
                }

                var path = $"./Storage/{fileStream.Name}";

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

       



    }
}
