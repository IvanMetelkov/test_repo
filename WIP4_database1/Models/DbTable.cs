using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIP4_database1.Models
{
    public class DbTable
    {
        public string TableName { get; set; }
        public IList<Column> columns = new List<Column>();
        public int RowsCount { get; set; }
    }
    public class Column
    {
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public IList<string> columnContent = new List<string>();
    }
}