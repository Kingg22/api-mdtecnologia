using MD_Tech.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oci.ObjectstorageService.Responses;

namespace MD_Tech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IStorageApi storageApi;
        private readonly LogsApi logger;

        public TestController(IStorageApi storageApi)
        {
            this.storageApi = storageApi;
            logger = new LogsApi(GetType());
        }

        [HttpPost]
        public async Task<ActionResult> Put(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest();
            }
            using var fileStream = file.OpenReadStream();
            var result = await storageApi.PutObjectAsync(fileStream, file.FileName, file.ContentType, ["products-images"]);
            if (result is null)
            {
                return Problem();
            }
            return Accepted(result);
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string fileName)
        {
            var response = await storageApi.GetObjectAsync(fileName);
            if (response is null)
            {
                return NotFound();
            }
            if (response is GetObjectResponse getObject)
            {
                logger.Informacion("Se ha obtenido de OCI");
                return File(getObject.InputStream, getObject.ContentType);
            }
            return Problem("implementación no soportada");
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromQuery] string fileName)
        {
            var response = await storageApi.DeleteObjectAsync(fileName);
            if (response)
            {
                return NoContent();
            } 
            else 
            {
                return Problem();
            }
        }
    }
}
