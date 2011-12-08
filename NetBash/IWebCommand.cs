using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBash
{
    interface IWebCommand
    {
        string Process(string commandText);
    }
}
