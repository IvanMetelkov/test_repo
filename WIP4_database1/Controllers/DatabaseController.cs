using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WIP4_database1.Models;
using WIP4_database1.Repository;

namespace WIP4_database1.Controllers
{
    public class DatabaseController : Controller
    {
        private readonly TableRepository tableRepository;

        public DatabaseController(IConfiguration configuration)
        {
            tableRepository = new TableRepository(configuration);
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Check()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = reader.ReadToEnd();
                return Content(tableRepository.DatabaseCheck(body), "application/json");
            }
        }
        [HttpPost]
        public IActionResult Copy()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                try
                {
                    var body = reader.ReadToEnd();
                    tableRepository.DatabaseCopy(body);
                    return Ok();
                }
                catch (Exception e)
                {
                    string exception = e.ToString();
                    ErrorMessage errorMessage = new ErrorMessage();
                    errorMessage.error.Message = exception;
                    string output = JsonConvert.SerializeObject(errorMessage);
                    return Content(output, "application/json");
                }
            }
        }
    }
}