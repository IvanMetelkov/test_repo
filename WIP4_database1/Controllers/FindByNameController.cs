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
    public class FindByNameController : Controller
    {
        private readonly TableRepository tableRepository;

        public FindByNameController(IConfiguration configuration)
        {
            tableRepository = new TableRepository(configuration);
        }


        public IActionResult Index(string componentType, int? dbID, string s)
        {
            if (dbID == null)
            {
                dbID = 0;
            }
            if(s == null)
            {
                s = "table1";
            }
            if(componentType == null)
            {
                componentType = "table";
            }
            return View(tableRepository.FindByName(componentType, dbID.Value, s));
        }
    }
}