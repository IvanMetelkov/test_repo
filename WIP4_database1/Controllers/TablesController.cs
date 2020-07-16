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
    public class TablesController : Controller
    {
        private readonly TableRepository tableRepository;

        public TablesController(IConfiguration configuration)
        {
            tableRepository = new TableRepository(configuration);
        }


        public IActionResult Index()
        {
            return View(tableRepository.FindAllTablesDB0());
        }
    }
}
