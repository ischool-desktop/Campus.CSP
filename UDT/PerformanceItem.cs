using FISCA.UDT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Behavior.CSP.UDT
{
    [TableName("campus.csp.performance_item")]
    class PerformanceItem : FISCA.UDT.ActiveRecord
    {
        [Field(Field = "item")]
        public string Item { get; set; }
        [Field(Field = "order")]
        public int Order { get; set; }
    }
}
