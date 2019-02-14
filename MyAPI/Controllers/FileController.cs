using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyAPI.Common;

namespace MyAPI.Controllers
{
    [Produces("application/json")]
    [Consumes("application/json","multipart/form-data")]
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private AppSet Appset { get; set; }
        private IHostingEnvironment _env { get; set; }

        public FileController(IOptions<AppSet> setting, IHostingEnvironment env)
        {
            _env = env;
            Appset = setting.Value;
        }

        [HttpPost]
        public ActionResult PostFiles(IFormCollection files)
        {
            if(files.Files.Count <= 0)
            {
                return BadRequest();
            }

            foreach (IFormFile file in files.Files)
            {
                StreamToFile(file.OpenReadStream(), file.FileName);
            }
            return Ok();
        }

        private void StreamToFile(Stream stream, string filename)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Seek(0, SeekOrigin.Begin);
            
            FileStream fs = new FileStream(_env.ContentRootPath + (Path.Combine(Appset.FileUpload, filename)), FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(buffer);
            bw.Close();
            fs.Close();
        }
    }
}