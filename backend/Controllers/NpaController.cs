using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using backend.Contracts;
using backend.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{

    [ApiController]
    [Route("api/v1/Npa")]
    //[Authorize]
    public class NpaController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public NpaController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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


    }
}
