using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBash
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WebCommandAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public WebCommandAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
