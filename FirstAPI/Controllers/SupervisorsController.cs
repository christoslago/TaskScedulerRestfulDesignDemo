using DataRepository.Models;
using Logic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Microsoft.Net.Http.Headers;
using Org.BouncyCastle.Utilities;
using System.Text;

namespace Sample.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SupervisorsController : CoreController<Supervisor>
    {
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;
        private ISupervisorsService SupervisorsService;
        public SupervisorsController(ISupervisorsService supervisorsService, Microsoft.AspNetCore.Hosting.IHostingEnvironment env) : base(supervisorsService)
        {
            Environment = env;
            SupervisorsService = supervisorsService;
        }
        [RequiredScope("data.write")]
        [HttpPost("TestAttachmentFromFile")]
        public IActionResult UploadAttachment(List<IFormFile> postedFiles)
        {
            byte[] fileToPreview = new byte[0];
            List<string> uploadedFiles = new List<string>();

            if (postedFiles[0].Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    postedFiles[0].CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    fileToPreview = fileBytes;
                    // act on the Base64 data
                }
            }

            string fileName = Path.GetFileName(postedFiles[0].FileName);
            using (FileStream stream = new FileStream(Path.Combine("Data", fileName), FileMode.Create))
            {
                postedFiles[0].CopyTo(stream);
                uploadedFiles.Add(fileName);
                var Message = string.Format("<b>{0}</b> uploaded.<br />", fileName);
            }
            return File(fileToPreview, postedFiles[0].ContentType);
        }

        [RequiredScope("data.write")]
        [HttpPost("TestAttachmentFromBinary")]
        public IActionResult UploadAttachmentBinary(string postedFiles)
        {
           
            byte[] fileToPreview = new byte[0];
           

            if (postedFiles.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    var fileBytes = ms.ToArray();
                    fileToPreview = fileBytes;
                    // act on the Base64 data
                }
            }

            //string fileName = Path.GetFileName(postedFiles[0].FileName);
            //using (FileStream stream = new FileStream(Path.Combine("Data",fileName), FileMode.Create))
            //{
            //    postedFiles[0].CopyTo(stream);
            //    uploadedFiles.Add(fileName);
            //    var Message = string.Format("<b>{0}</b> uploaded.<br />", fileName);
            //}
            return File(fileToPreview, "*/*");
        }






        [RequiredScope("data.write")]
        [HttpPost("TestAttachmentFromBase64")]
        public IActionResult UploadAttachmentBase64([FromBody] Payload payload)
        {
            string output = payload.Base64String.Substring(payload.Base64String.IndexOf(',') + 1);
            byte[] bytes = Convert.FromBase64String(output);


            string input = payload.Base64String;
            int index = input.IndexOf(",");
            if (index >= 0)
                input = input.Substring(0,index);
            int indexOfData = input.IndexOf(":");
            if (indexOfData >= 0)
                input = input.Substring(indexOfData + 1);
            int indexOfEnd = input.IndexOf(";");
            if (indexOfEnd >= 0)
                input = input.Substring(0,indexOfEnd);

            return File(bytes, input);
        }
    }
    public class ByteArrayPayload
    {
        public byte[] ByteArray { get; set; }
    }
    public class Payload
    {
        public string Base64String { get; set; }
    }
}
