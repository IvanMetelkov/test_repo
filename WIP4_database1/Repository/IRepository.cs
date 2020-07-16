﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIP4_database1.Models;

namespace WIP4_database1.Repository
{
    public interface IRepository<T> where T : DbComponent
    {
        //  T FindByID(int id);
        // string FindNameById(int id);
        IEnumerable<T> FindByName(string componentType, int dbID, string s);
        //   string ReturnNameOFTable(string s);
        IEnumerable<T> FindAllTablesDB0();
        IEnumerable<T> FindAllFunctionsDB0();
        IEnumerable<T> FindAllTablesDB1();
        IEnumerable<T> FindAllFunctionsDB1();
    }
}