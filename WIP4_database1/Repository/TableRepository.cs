using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data;
using Npgsql;
using WIP4_database1.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

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
        public string DatabaseCheck()
        {
            string output = "";
            string s = File.ReadAllText(@"c:\database_structure1.json");
            JObject DBstructure = JObject.Parse(s); 
            int DBType = (int)DBstructure.GetValue("database");
            IList<JToken> Tables = DBstructure["tables"].Children().ToList();
            IList<JToken> Functions = DBstructure["functions"].Children().ToList();
            IList<String> TablesList = new List<String>();
            DBJsonStructure jsonreply = new DBJsonStructure();
            jsonreply.database = DBType;
            foreach (JToken Result in Tables)
            {
                String TableName = (string)Result;
                TablesList.Add(TableName);
            }
            IList<String> FunctionList = new List<String>();
            foreach (JToken Result in Functions)
            {
                String FunctionName = (string)Result;
                FunctionList.Add(FunctionName);
            }
            IList<String> DBtables = new List<String>();
            IList<String> DBfunctions = new List<String>();
            if (DBType == 0)
            {
                foreach (DbComponent test in FindAllTablesDB0())
                {
                    DBtables.Add(test.Name);
                }
                foreach (DbComponent test in FindAllFunctionsDB0())
                {
                    DBfunctions.Add(test.Name);
                }
            }
            if (DBType == 1)
            {
                foreach (DbComponent test in FindAllTablesDB1())
                {
                    DBtables.Add(test.Name);
                }
                foreach (DbComponent test in FindAllFunctionsDB1())
                {
                    DBfunctions.Add(test.Name);
                }
            }
            int TablesCount = 0;
            int FunctionsCount = 0;
            var TablesIntersect = DBtables.Intersect(TablesList);
            var TablesMissing = TablesList.Except(TablesIntersect);
            TablesCount = TablesMissing.Count();
            if (TablesCount == 0)
            {

            }
            else
            {
                Console.WriteLine("The following tables are missing in DB:");
                foreach (String test in TablesMissing)
                {
                    jsonreply.tables.Add(test);
                }
            }
            var FunctionsIntersect = DBfunctions.Intersect(FunctionList);
            var FuntionsMissing = FunctionList.Except(FunctionsIntersect);
            FunctionsCount = FuntionsMissing.Count();
            if (FunctionsCount == 0)
            {

            }
            else
            {
                Console.WriteLine("The following functions are missing in DB:");
                foreach (String test in FuntionsMissing)
                {
                    jsonreply.functions.Add(test);
                }
            }
            if (FunctionsCount > 0 || TablesCount > 0)
            {
                output = JsonConvert.SerializeObject(jsonreply);
            }
            return output;
        }
        public string DatabaseCheck2(string s)
        {
            string output = "";
            //string s = File.ReadAllText(@"c:\database_structure1.json");
            JObject DBstructure = JObject.Parse(s);
            int DBType = (int)DBstructure.GetValue("database");
            IList<JToken> Tables = DBstructure["tables"].Children().ToList();
            IList<JToken> Functions = DBstructure["functions"].Children().ToList();
            IList<String> TablesList = new List<String>();
            DBJsonStructure jsonreply = new DBJsonStructure();
            jsonreply.database = DBType;
            foreach (JToken Result in Tables)
            {
                String TableName = (string)Result;
                TablesList.Add(TableName);
            }
            IList<String> FunctionList = new List<String>();
            foreach (JToken Result in Functions)
            {
                String FunctionName = (string)Result;
                FunctionList.Add(FunctionName);
            }
            IList<String> DBtables = new List<String>();
            IList<String> DBfunctions = new List<String>();
            if (DBType == 0)
            {
                foreach (DbComponent test in FindAllTablesDB0())
                {
                    DBtables.Add(test.Name);
                }
                foreach (DbComponent test in FindAllFunctionsDB0())
                {
                    DBfunctions.Add(test.Name);
                }
            }
            if (DBType == 1)
            {
                foreach (DbComponent test in FindAllTablesDB1())
                {
                    DBtables.Add(test.Name);
                }
                foreach (DbComponent test in FindAllFunctionsDB1())
                {
                    DBfunctions.Add(test.Name);
                }
            }
            int TablesCount = 0;
            int FunctionsCount = 0;
            var TablesIntersect = DBtables.Intersect(TablesList);
            var TablesMissing = TablesList.Except(TablesIntersect);
            TablesCount = TablesMissing.Count();
            if (TablesCount == 0)
            {

            }
            else
            {
                Console.WriteLine("The following tables are missing in DB:");
                foreach (String test in TablesMissing)
                {
                    jsonreply.tables.Add(test);
                }
            }
            var FunctionsIntersect = DBfunctions.Intersect(FunctionList);
            var FuntionsMissing = FunctionList.Except(FunctionsIntersect);
            FunctionsCount = FuntionsMissing.Count();
            if (FunctionsCount == 0)
            {

            }
            else
            {
                Console.WriteLine("The following functions are missing in DB:");
                foreach (String test in FuntionsMissing)
                {
                    jsonreply.functions.Add(test);
                }
            }
            if (FunctionsCount > 0 || TablesCount > 0)
            {
                output = JsonConvert.SerializeObject(jsonreply);
            }
            return output;
        }
        public IEnumerable<Column> GetTableColumns(int DBType, string s)
        {
            if (DBType == 0)
            {
                using (IDbConnection dbConnection = Connection0)
                {
                    dbConnection.Open();
                    return dbConnection.Query<Column>("SELECT column_name as ColumnName, data_type as ColumnType FROM information_schema.columns WHERE table_name = @StringName;", new { StringName = s });
                }
            }
            else
            {
                using (IDbConnection dbConnection = Connection1)
                {
                dbConnection.Open();
                return dbConnection.Query<Column>("SELECT column_name as ColumnName, data_type as ColumnType FROM information_schema.columns WHERE table_name = @StringName; ", new { StringName = s });

                }
            }
        }
        public IEnumerable<string> GetColumnContent(int DBType, string s, string t)
        {
            if (DBType == 0)
            {
                using (IDbConnection dbConnection = Connection0)
                {
                    dbConnection.Open();
                    return dbConnection.Query<string>("SELECT @ColumnName as ColumnContent FROM @tableName", new { ColumnName = s , tableName = t });
                }
            }
            else
            {
                using (IDbConnection dbConnection = Connection1)
                {
                    dbConnection.Open();
                    return dbConnection.Query<string>("SELECT @ColumnName as ColumnContent FROM @tableName", new { ColumnName = s, tableName = t });

                }
            }
        }
        public IEnumerable<List<string>> GetTableData(int DBType, string s)
        {
            if (DBType == 0)
            {
                using (IDbConnection dbConnection = Connection0)
                {
                    dbConnection.Open();
                    return dbConnection.Query<List<string>>("SELECT * FROM @tableName", new { tableName = s });
                }
            }
            else
            {
                using (IDbConnection dbConnection = Connection1)
                {
                    dbConnection.Open();
                    return dbConnection.Query<List<string>>("SELECT * FROM @tableName", new { tableName = s });

                }
            }
        }
        public void DatabaseCopy()
        {
            int DBType = 1; //исправить потом на считывание из джсона
            IList<DbTable> DBtables = new List<DbTable>();
            int countTables = 0;
            int countColumns = 0;
            int rowsCount = 0;
            int totalRows;
            if (DBType == 0)
            {
                foreach (DbComponent test in FindAllTablesDB0())
                {
                    DBtables[countTables].TableName = test.Name;
                    foreach (Column currentColumn in GetTableColumns(DBType, DBtables[countTables].TableName))
                    {
                        DBtables[countTables].columns[countColumns].ColumnName = currentColumn.ColumnName;
                        DBtables[countTables].columns[countColumns].ColumnType = currentColumn.ColumnType;
                        foreach (string data in GetColumnContent(DBType, currentColumn.ColumnName, DBtables[countTables].TableName))
                        {
                            DBtables[countTables].columns[countColumns].columnContent[rowsCount] = data;
                            rowsCount++;
                        }
                        countColumns++;
                        rowsCount = 0;
                    }
                    countTables++;
                    countColumns = 0;
                }
            }
            if (DBType == 1)
            {
                foreach (DbComponent test in FindAllTablesDB1())
                {
                    DBtables[countTables].TableName = test.Name;
                    foreach (Column currentColumn in GetTableColumns(DBType, DBtables[countTables].TableName))
                    {
                        DBtables[countTables].columns[countColumns].ColumnName = currentColumn.ColumnName;
                        DBtables[countTables].columns[countColumns].ColumnType = currentColumn.ColumnType;
                        foreach (string data in GetColumnContent(DBType, currentColumn.ColumnName, DBtables[countTables].TableName))
                        {
                            DBtables[countTables].columns[countColumns].columnContent[rowsCount] = data;
                            rowsCount++;
                        }
                        countColumns++;
                        rowsCount = 0;
                    }
                    countTables++;
                    countColumns = 0;
                }
            }
        }
    }
}
