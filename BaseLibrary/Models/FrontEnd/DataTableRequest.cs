using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.Models.FrontEnd
{
   
    public class DataTableRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public string SearchValue { get; set; }
        //public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public ColumnFilter[] Columns { get; set; }
    }

    public class ColumnFilter
    {
        public string Data { get; set; }
        public string SearchValue { get; set; }
        public bool Searchable { get; set; }
        public bool IsMultipleSearch { get; set; }
        public string[] SearchValues { get; set; }
    }
}

