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
    [Authorize]
    [Route("api/[controller]")]
    public class StateController : Controller
    {
        private readonly IHostingEnvironment _appEnvironment;
        public StateController(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        
        [HttpGet]
        public IEnumerable<State> GetAll()
        {
            var rootPath = _appEnvironment.WebRootPath;
            using (StreamReader r = new StreamReader(rootPath + "/files/json/state.json"))
            {
                string json = r.ReadToEnd();
                List<State> items = JsonConvert.DeserializeObject<List<State>>(json);
                return items;
            }
        }

        [HttpGet("{code}")]
        public IActionResult GetByCode(string code)
        {
            var rootPath = _appEnvironment.WebRootPath;
            using (StreamReader r = new StreamReader(rootPath + "/files/json/state.json"))
            {
                string json = r.ReadToEnd();
                List<State> items = JsonConvert.DeserializeObject<List<State>>(json);
                var item = items.FirstOrDefault(o => o.Code.ToLower() == code.ToLower());
                return new ObjectResult(item);
            }
        }
    }
}