using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIP4_database1.Models;

namespace WIP4_database1.Repository
{
    public interface IRepository<T> where T : DbComponent
    {
        IEnumerable<T> FindByName(string componentType, int dbID, string s);
        IEnumerable<T> FindAllTablesDB(int dbID);
        IEnumerable<T> FindAllFunctionsDB(int dbID);
        string DatabaseCheck(string s);
        void DatabaseCopy(string s);
        IEnumerable<Column> GetTableColumns(int dbID, string s);
        IEnumerable<string> GetColumnContent(int dbID, string s, string t);
        IEnumerable<List<string>> GetTableData(int dbID, string s);
        int NumberOfTableRows(int dbID, string s);
        void DropDatabaseTable(int dbID, string s);
        void Createtable(int dbID, string s);
        void AddRowCount(int dbID, string s, int i);
        void AddColumn(int dbID, string s, string c, string t);
        void CopyColumnData(int dbID, string s, string t, string f, int i);
        void DropRowCountColumn(int dbID, string s);
    }
}
