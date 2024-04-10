using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LotusGoIMWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly IConfiguration configuration;
        public UploadFileController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null)
            {
                var fileDir = configuration["UploadFileConfig:Path"];
                if (!Directory.Exists(fileDir))
                {
                    Directory.CreateDirectory(fileDir);
                }

                string projectFileName = file.FileName;
                Guid guid = Guid.NewGuid();
                string fileName = $@"{guid.ToString() + '.' + projectFileName.Split('.')[^1]}";
                string filePath = fileDir + fileName;
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                }
                var result = configuration["UploadFileConfig:Header"].ToString() + fileName;
                Console.WriteLine(result);
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
