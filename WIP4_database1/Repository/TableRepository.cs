using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data;
using Npgsql;
using WIP4_database1.Models;

namespace WIP4_database1.Repository
{
    public class TableRepository : IRepository<DbComponent>
    {
        private string connectionString0;
        private string connectionString1;

        public TableRepository(IConfiguration configuration)
        {
            connectionString0 = configuration.GetValue<string>("database0");
            connectionString1 = configuration.GetValue<string>("database1");
        }

        internal IDbConnection Connection0
        {
            get
            {
                return new NpgsqlConnection(connectionString0);
            }
        }
        internal IDbConnection Connection1
        {
            get
            {
                return new NpgsqlConnection(connectionString1);
            }
        }
        /*  public string FindByID(int id)
          {
              return "0";
          }
          public string FindNameById(int id)
          {
              return "0";
          }

          public string ReturnNameOFTable(string s)
          {
              return "0";
          }*/
        public IEnumerable<DbComponent> FindByName(string componentType, int dbID, string s)
        {
            if (dbID == 0)
            {
                using (IDbConnection dbConnection = Connection0)
                {
                    dbConnection.Open();
                    if (componentType == "table")
                    {
                        return dbConnection.Query<DbComponent>("SELECT row_number() OVER () as Id, table_name as Name FROM information_schema.tables  where table_schema='public' AND table_name = @StringName ORDER BY table_name;", new { StringName = s });
                    }
                    else
                        return dbConnection.Query<DbComponent>("SELECT row_number() OVER () as Id, routines.routine_name  as Name FROM information_schema.routines LEFT JOIN information_schema.parameters ON routines.specific_name = parameters.specific_name WHERE routines.specific_schema = 'public' AND routines.routine_name = @StringName ORDER BY routines.routine_name, parameters.ordinal_position;", new { StringName = s });

                }
            }
            else
            {
                using (IDbConnection dbConnection = Connection1)
                {
                    dbConnection.Open();
                    if (componentType == "table")
                    {
                        return dbConnection.Query<DbComponent>("SELECT row_number() OVER () as Id, table_name as Name FROM information_schema.tables  where table_schema='public' AND table_name = @StringName ORDER BY table_name;", new { StringName = s });
                    }
                    else
                        return dbConnection.Query<DbComponent>("SELECT row_number() OVER () as Id, routines.routine_name  as Name FROM information_schema.routines LEFT JOIN information_schema.parameters ON routines.specific_name = parameters.specific_name WHERE routines.specific_schema = 'public' AND routines.routine_name = @StringName ORDER BY routines.routine_name, parameters.ordinal_position;", new { StringName = s });

                }
            }
        }
        public IEnumerable<DbComponent> FindAllTablesDB0()
        {
            using (IDbConnection dbConnection = Connection0)
            {
                dbConnection.Open();
                //dbConnection.Execute("CREATE OR REPLACE VIEW testview5 AS SELECT row_number() OVER () as Id, table_name AS Name FROM information_schema.tables;");
                return dbConnection.Query<DbComponent>("SELECT row_number() OVER () as Id, table_name as Name FROM information_schema.tables  where table_schema='public' ORDER BY table_name;");

            }
        }
        public IEnumerable<DbComponent> FindAllFunctionsDB0()
        {
            using (IDbConnection dbConnection = Connection0)
            {
                dbConnection.Open();
                return dbConnection.Query<DbComponent>("SELECT row_number() OVER () as Id, routines.routine_name  as Name FROM information_schema.routines LEFT JOIN information_schema.parameters ON routines.specific_name = parameters.specific_name WHERE routines.specific_schema = 'public' ORDER BY routines.routine_name, parameters.ordinal_position;");

            }
        }
        public IEnumerable<DbComponent> FindAllTablesDB1()
        {
            using (IDbConnection dbConnection = Connection1)
            {
                dbConnection.Open();
                //dbConnection.Execute("CREATE OR REPLACE VIEW testview5 AS SELECT row_number() OVER () as Id, table_name AS Name FROM information_schema.tables;");
                return dbConnection.Query<DbComponent>("SELECT row_number() OVER () as Id, table_name as Name FROM information_schema.tables  where table_schema='public' ORDER BY table_name;");

            }
        }
        public IEnumerable<DbComponent> FindAllFunctionsDB1()
        {
            using (IDbConnection dbConnection = Connection1)
            {
                dbConnection.Open();
                return dbConnection.Query<DbComponent>("SELECT row_number() OVER () as Id, routines.routine_name  as Name FROM information_schema.routines LEFT JOIN information_schema.parameters ON routines.specific_name = parameters.specific_name WHERE routines.specific_schema = 'public' ORDER BY routines.routine_name, parameters.ordinal_position;");

            }
        }
    }
}
