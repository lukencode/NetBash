using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBash
{
    internal class CommandResult
    {
        public string Result { get; set; }
        public bool IsHtml { get; set; }
        public string FileName { get; set; }
    }
}
