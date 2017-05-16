using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NowFloatsTest.Models
{
    public class SQLTable
    {
        public string TableName { get; set; }
        public string UserId { get; set; }
        public Dictionary<string, string> Columns { get; set; }
    }
}