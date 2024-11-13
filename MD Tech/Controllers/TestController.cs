using MD_Tech.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MD_Tech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IStorageApi storageApi;

        public TestController(IStorageApi storageApi)
        {
            this.storageApi = storageApi;
        }

        [HttpPost]
        public async Task<ActionResult> Put(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest();
            }
            using var fileStream = file.OpenReadStream();
            var result = await storageApi.PutObjectAsync(new StorageApiDto()
            {
                Stream = fileStream,
                Name = file.FileName,
                Type = file.ContentType
            });
            return result is null ? BadRequest() : Created(Url.Action(nameof(Get), "Test", new { fileName = result.Name }), new { result.Status });
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string fileName)
        {
            var response = await storageApi.GetObjectAsync(fileName);
            return response is null ? NotFound() : File(response.Stream, response.Type);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromQuery] string fileName)
        {
            var response = await storageApi.DeleteObjectAsync(fileName);
            return response ? NoContent() : BadRequest();
        }
    }
}
