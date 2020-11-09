using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WIP4_database1.Models;
using WIP4_database1.Repository;

namespace WIP4_database1.Controllers
{
    public class FunctionsController : Controller
    {
        private readonly TableRepository tableRepository;

        public FunctionsController(IConfiguration configuration)
        {
            tableRepository = new TableRepository(configuration);
        }


        public IActionResult Index()
        {
            return View(tableRepository.FindAllFunctionsDB(0));
        }
    }
}

