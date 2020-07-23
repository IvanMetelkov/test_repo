using System;
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
        string DatabaseCheck();
        string DatabaseCheck2(string s);
        void DatabaseCopy();
        IEnumerable<Column> GetTableColumns(int DBType, string s);
        IEnumerable<string> GetColumnContent(int DBType, string s, string t);
        IEnumerable<List<string>> GetTableData(int DBType, string s);
        int NumberOfTableRows(int DBType, string s);
        void DropDatabaseTable(int DBType, string s);

        void Createtable(int DBType, string s);
        void AddRowCount(int DBType, string s, int i);
        void AddColumn(int DBType, string s, string c, string t);
        void CopyColumnData(int DBType, string s, string t, string f, int i);
        void DropRowCountColumn(int DBtype, string s);
    }
}
