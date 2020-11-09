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
        private readonly string connectionString0;
        private readonly string connectionString1;

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

        public IDbConnection OpenConnection(int dbID)
        {
            if (dbID == 0)
                return Connection0;
            else
                return Connection1;
        }
        public IEnumerable<DbComponent> FindByName(string componentType, int dbID, string s)
        {
            using (IDbConnection dbConnection = OpenConnection(dbID))
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
        public IEnumerable<DbComponent> FindAllTablesDB0()
        {
            using (IDbConnection dbConnection = Connection0)
            {
                dbConnection.Open();
                //dbConnection.Execute("CREATE OR REPLACE VIEW testview5 AS SELECT row_number() OVER () as Id, table_name AS Name FROM information_schema.tables;");
                return dbConnection.Query<DbComponent>("SELECT row_number() OVER () as Id, table_name as Name FROM information_schema.tables  where table_schema='public' ORDER BY table_name;");

            }
        }

        public IEnumerable<DbComponent> FindAllTablesDB(int dbID)
        {
            using (IDbConnection dbConnection = OpenConnection(dbID))
            {
                dbConnection.Open();
                //dbConnection.Execute("CREATE OR REPLACE VIEW testview5 AS SELECT row_number() OVER () as Id, table_name AS Name FROM information_schema.tables;");
                return dbConnection.Query<DbComponent>("SELECT row_number() OVER () as Id, table_name as Name FROM information_schema.tables  where table_schema='public' ORDER BY table_name;");
            }
        }

        public IEnumerable<DbComponent> FindAllFunctionsDB(int dbID)
        {
            using (IDbConnection dbConnection = OpenConnection(dbID))
            {
                dbConnection.Open();
                return dbConnection.Query<DbComponent>("SELECT row_number() OVER () as Id, routines.routine_name  as Name FROM information_schema.routines LEFT JOIN information_schema.parameters ON routines.specific_name = parameters.specific_name WHERE routines.specific_schema = 'public' ORDER BY routines.routine_name, parameters.ordinal_position;");
            }
        }
        public string DatabaseCheck(string s)
        {
            string output = "";
            JObject dbStructure = JObject.Parse(s);
            int dbType = (int)dbStructure.GetValue("database");
            IList<JToken> tables = dbStructure["tables"].Children().ToList();
            IList<JToken> functions = dbStructure["functions"].Children().ToList();
            IList<string> tablesList = new List<string>();
            DBJsonStructure jsonreply = new DBJsonStructure();
            jsonreply.database = dbType;

            foreach (JToken result in tables)
            {
                string tableName = (string)result;
                tablesList.Add(tableName);
            }

            IList<string> functionList = new List<string>();

            foreach (JToken result in functions)
            {
                string functionName = (string)result;
                functionList.Add(functionName);
            }

            IList<string> dbTables = new List<string>();
            IList<string> dbFunctions = new List<string>();

            foreach (DbComponent test in FindAllTablesDB(dbType))
            {
                dbTables.Add(test.Name);
            }

            foreach (DbComponent test in FindAllFunctionsDB(dbType))
            {
                dbFunctions.Add(test.Name);
            }

            int tablesCount;
            int functionsCount;
            var tablesIntersect = dbTables.Intersect(tablesList);
            var tablesMissing = tablesList.Except(tablesIntersect);
            tablesCount = tablesMissing.Count();

            if (tablesCount == 0)
            {

            }
            else
            {
                Console.WriteLine("The following tables are missing in DB:");
                foreach (String test in tablesMissing)
                {
                    jsonreply.tables.Add(test);
                }
            }

            var functionsIntersect = dbFunctions.Intersect(functionList);
            var funtionsMissing = functionList.Except(functionsIntersect);
            functionsCount = funtionsMissing.Count();

            if (functionsCount == 0)
            {

            }
            else
            {
                Console.WriteLine("The following functions are missing in DB:");
                foreach (String test in funtionsMissing)
                {
                    jsonreply.functions.Add(test);
                }
            }

            if (functionsCount > 0 || tablesCount > 0)
            {
                output = JsonConvert.SerializeObject(jsonreply);
            }
            return output;
        }
        public IEnumerable<Column> GetTableColumns(int dbID, string s)
        {
            using (IDbConnection dbConnection = OpenConnection(dbID))
            {
                dbConnection.Open();
                return dbConnection.Query<Column>("SELECT column_name as ColumnName, data_type as ColumnType FROM information_schema.columns WHERE table_name = @StringName;", new { StringName = s });
            }
        }
        public IEnumerable<string> GetColumnContent(int dbID, string s, string t)
        {
            using (IDbConnection dbConnection = OpenConnection(dbID))
            {
                dbConnection.Open();
                string sql = string.Format("SELECT {0} as ColumnContent FROM {1};", s, t);
                return dbConnection.Query<string>(sql);
            }
        }
        public IEnumerable<List<string>> GetTableData(int dbID, string s)
        {
            using (IDbConnection dbConnection = OpenConnection(dbID))
            {
                dbConnection.Open();
                string sql = string.Format("SELECT * FROM {0};", s);
                return dbConnection.Query<List<string>>(sql);
            }
        }
        public int NumberOfTableRows(int dbID, string s)
        {
            using (IDbConnection dbConnection = OpenConnection(dbID))
            {
                dbConnection.Open();
                string sql = string.Format("SELECT count(*) FROM {0};", s);
                return dbConnection.Query<int>(sql).First();
            }
        }

        public void DropDatabaseTable(int dbID, string s)
        {
            using (IDbConnection dbConnection = OpenConnection(dbID))
            {
                dbConnection.Open();
                string sql = string.Format("DROP TABLE {0};", s);
                dbConnection.Execute(sql);
            }
        }
        public void Createtable(int dbID, string s)
        {
            using (IDbConnection dbConnection = OpenConnection(dbID))
            {
                dbConnection.Open();
                string sql = string.Format("CREATE TABLE {0}(table_row_count integer);", s);
                dbConnection.Execute(sql);
            }
        }

        public void AddRowCount(int dbID, string s, int i)
        {
            using (IDbConnection dbConnection = OpenConnection(dbID))
            {
               dbConnection.Open();
               string sql = string.Format("INSERT INTO {0} (table_row_count) VALUES ('{1}');", s, i);
               dbConnection.Execute(sql);
            }
        }
        public void AddColumn(int dbID, string s, string c, string t)
        {
            using (IDbConnection dbConnection = OpenConnection(dbID))
            {
                dbConnection.Open();
                string sql = string.Format("ALTER TABLE {0} ADD COLUMN {1} {2};", s, c, t);
                dbConnection.Execute(sql);
            }
        }
        public void CopyColumnData(int dbID, string s, string t, string f, int i)
        {
            using (IDbConnection dbConnection = OpenConnection(dbID))
            {
                dbConnection.Open();
                string sql = string.Format("UPDATE {0} SET {1} = '{2}' WHERE table_row_count = {3};", s, t, f, i);
                dbConnection.Execute(sql);
            }
        }
        public void DropRowCountColumn(int dbID, string s)
        {
            using (IDbConnection dbConnection = OpenConnection(dbID))
            {
                dbConnection.Open();
                string sql = string.Format("ALTER TABLE {0} DROP COLUMN table_row_count RESTRICT;", s);
                dbConnection.Execute(sql);
            }
        }
        public void DatabaseCopy(string s)
        {
            JObject DBstructure = JObject.Parse(s);
            int FormerDB = (int)DBstructure.GetValue("database");
            int TargetDB;
            if (FormerDB == 0)
            {
                TargetDB = 1;
            }
            else
            {
                TargetDB = 0;
            }
            IList<DbTable> DBtables = new List<DbTable>();
            int countTables = 0;
            int countColumns = 0;
            int rowsCount = 0;
                foreach (DbComponent table in FindAllTablesDB(TargetDB))
                {
                    DropDatabaseTable(TargetDB, table.Name);
                }
                foreach (DbComponent table in FindAllTablesDB(FormerDB))
                {
                    DbTable blankTable = new DbTable();
                    DBtables.Add(blankTable);
                    DBtables[countTables].TableName = table.Name;
                    Createtable(TargetDB, DBtables[countTables].TableName);
                    DBtables[countTables].RowsCount = NumberOfTableRows(FormerDB, DBtables[countTables].TableName);
                    for (int i = 0; i < DBtables[countTables].RowsCount; i++)
                    {
                        AddRowCount(TargetDB, DBtables[countTables].TableName, i + 1);
                    }
                    foreach (Column currentColumn in GetTableColumns(FormerDB, DBtables[countTables].TableName))
                    {
                        Column blankColumn = new Column();
                        DBtables[countTables].columns.Add(blankColumn);
                        DBtables[countTables].columns[countColumns].ColumnName = currentColumn.ColumnName;
                        DBtables[countTables].columns[countColumns].ColumnType = currentColumn.ColumnType;
                        AddColumn(TargetDB, DBtables[countTables].TableName, DBtables[countTables].columns[countColumns].ColumnName, DBtables[countTables].columns[countColumns].ColumnType);
                        foreach (string data in GetColumnContent(FormerDB, currentColumn.ColumnName, DBtables[countTables].TableName))
                        {
                            string blankString = "";
                            DBtables[countTables].columns[countColumns].columnContent.Add(blankString);
                            DBtables[countTables].columns[countColumns].columnContent[rowsCount] = data;
                            CopyColumnData(TargetDB, DBtables[countTables].TableName, DBtables[countTables].columns[countColumns].ColumnName, data, rowsCount + 1);
                            rowsCount++;
                        }
                        countColumns++;
                        rowsCount = 0;
                    }
                    DropRowCountColumn(TargetDB, DBtables[countTables].TableName);
                    countTables++;
                    countColumns = 0;
                }
        }
    }
}
