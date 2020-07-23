﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WIP4_database1.Models;
using WIP4_database1.Repository;

namespace WIP4_database1.Controllers
{
    public class TestCopyController : Controller
    {
        private readonly TableRepository tableRepository;

        public TestCopyController(IConfiguration configuration)
        {
            tableRepository = new TableRepository(configuration);
        }
        [HttpPost]
        public IActionResult Index()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = reader.ReadToEnd();
                tableRepository.DatabaseCopy(body);
                return View();
            }
        }
    }
}
