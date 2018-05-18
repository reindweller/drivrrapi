using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Drivrr.Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Drivrr.Api.Controllers
{
    [Route("api/[controller]")]
    public class SuburbController : Controller
    {
        private readonly IHostingEnvironment _appEnvironment;
        public SuburbController(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        [Authorize]
        [HttpGet]
        public IEnumerable<Suburb> GetAll()
        {
            var rootPath = _appEnvironment.WebRootPath;
            using (StreamReader r = new StreamReader(rootPath + "/files/json/suburb.json"))
            {
                string json = r.ReadToEnd();
                List<Suburb> items = JsonConvert.DeserializeObject<List<Suburb>>(json);
                return items;
            }
        }

        //[Authorize]
        //[HttpGet]
        //public IEnumerable<Suburb> GetAll()
        //{
        //    var rootPath = _appEnvironment.WebRootPath;
        //    using (StreamReader r = new StreamReader(rootPath + "/files/json/suburb.json"))
        //    {
        //        string json = r.ReadToEnd();
        //        List<Suburb> items = JsonConvert.DeserializeObject<List<Suburb>>(json);
        //        return items;
        //    }
        //}

        [Authorize]
        [HttpGet("state/{stateCode}")]
        public IEnumerable<Suburb> GetByState(string stateCode)
        {
            var rootPath = _appEnvironment.WebRootPath;
            using (StreamReader r = new StreamReader(rootPath + "/files/json/suburb.json"))
            {
                string json = r.ReadToEnd();
                List<Suburb> items = JsonConvert.DeserializeObject<List<Suburb>>(json);
                return items.Where(o=>o.State.ToLower() == stateCode.ToLower());
            }
            
        }


        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var rootPath = _appEnvironment.WebRootPath;
            using (StreamReader r = new StreamReader(rootPath + "/files/json/suburb.json"))
            {
                string json = r.ReadToEnd();
                List<Suburb> items = JsonConvert.DeserializeObject<List<Suburb>>(json);
                var item = items.FirstOrDefault(o => o.Name.ToLower() == name.ToLower());
                return new ObjectResult(item);
            }
        }
    }
}