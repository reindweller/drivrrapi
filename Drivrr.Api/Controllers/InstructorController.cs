using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Drivrr.Api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace Drivrr.Api.Controllers
{
    [Route("api/[controller]")]
    public class InstructorController : Controller
    {
        private readonly IFileProvider _fileProvider;
        private readonly DrivrrContext _context;

        public InstructorController(DrivrrContext context, IFileProvider fileProvider)
        {
            _context = context;
            this._fileProvider = fileProvider;
        }

        [HttpGet]
        public IEnumerable<InstructorViewModel> GetAll()
        {
            //return _context.Instructors.ToList();
            return _context.Instructors.Select(o => new InstructorViewModel
            {
                Id = o.Id,
                FirstName = o.FirstName,
                LastName = o.LastName,
                State = o.State,
                Suburb = o.Suburb,
                PostCode = o.PostCode,
                Address = o.Address,
                School = o.School,
                Email = o.Email,
                Phone = o.Phone,
                Mobile = o.Mobile,
                Info = o.Info,
                //ProfilePic = o.ProfilePic,
                Price = o.Price,
                Language = o.Language
            }).ToList();
        }

        [HttpGet("{id}", Name = "GetInstructor")]
        public IActionResult GetById(Guid id)
        {
            var item = _context.Instructors.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpPost("profilepic")]
        public async Task<IActionResult> UploadFile(IFormFile file, Guid id)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            var ext = file.GetFilename().Split('.').Last();
            var fullFileName = string.Format("{0}.{1}", id.ToString(), ext);
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/images/profilepics",
                        //file.GetFilename()
                        fullFileName
                        );

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return RedirectToAction("Files");
        }

        [HttpPost("profilepic/update")]
        public async Task<IActionResult> UpdateFile(IFormFile file, Guid id)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            foreach (var item in this._fileProvider.GetDirectoryContents(""))
            {
                if (item.PhysicalPath.StartsWith(id.ToString()))
                {
                    System.IO.File.Delete(item.PhysicalPath);
                    break;
                }
            }


            var ext = file.GetFilename().Split('.').Last();
            var fullFileName = string.Format("{0}.{1}", id.ToString(), ext);
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/images/profilepics",
                        //file.GetFilename()
                        fullFileName
                        );

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return RedirectToAction("Files");
        }

        [HttpPost]
        public IActionResult Create([FromBody] InstructorViewModel item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            var instructor = new Instructor
            {
                Id = item.Id,
                FirstName = item.FirstName,
                LastName = item.LastName,
                State = item.State,
                Suburb = item.Suburb,
                Address = item.Address,
                School = item.School,
                Email = item.Email,
                Phone = item.Phone,
                Mobile = item.Mobile,
                Info = item.Info,
                //ProfilePic = item.
                Price = item.Price,
                Language = item.Language
            };

            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            return CreatedAtRoute("GetInstructor", new { id = instructor.Id }, instructor);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] InstructorViewModel item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var instructor = _context.Instructors.FirstOrDefault(t => t.Id == id);
            if (instructor == null)
            {
                return NotFound();
            }

            //instructor.Id = item.;
            instructor.FirstName = item.FirstName;
            instructor.LastName = item.LastName;
            instructor.State = item.State;
            instructor.Suburb = item.Suburb;
            instructor.Address = item.Address;
            instructor.School = item.School;
            instructor.Email = item.Email;
            instructor.Phone = item.Phone;
            instructor.Mobile = item.Mobile;
            instructor.Info = item.Info;
            instructor.Price = item.Price;
            instructor.Language = item.Language;


            _context.Instructors.Update(instructor);
            _context.SaveChanges();
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var Instructor = _context.Instructors.FirstOrDefault(t => t.Id == id);
            if (Instructor == null)
            {
                return NotFound();
            }

            _context.Instructors.Remove(Instructor);


            foreach (var item in this._fileProvider.GetDirectoryContents(""))
            {
                if (item.Name.StartsWith(id.ToString()))
                {
                    System.IO.File.Delete(item.PhysicalPath);
                    break;
                }
            }


            _context.SaveChanges();
            return new NoContentResult();
        }


        #region ForFile

        public IActionResult Files()
        {
            var model = new FilesViewModel();
            foreach (var item in this._fileProvider.GetDirectoryContents(""))
            {
                model.Files.Add(
                    new FileDetails { Name = item.Name, Path = item.PhysicalPath });
            }
            return View(model);
        }

        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot/images/profilepics", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
        #endregion
    }
}