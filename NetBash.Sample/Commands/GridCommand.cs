using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using NetBash.Formatting;

namespace NetBash.Sample.Commands
{
    [WebCommand("grid", "Usage: grid")]
    public class GridCommand : IWebCommand
    {
        public bool ReturnHtml
        {
            get { return false; }
        }

        public string Process(string[] args)
        {
            var tempData = new List<GridData>
            {
                new GridData
                {
                    RowId = 1,
                    SomeName = "NUMBER1",
                    Longtext = "12345678901234567890",
                    SomeDate = DateTime.Today
                },
                new GridData
                {
                    RowId = 2,
                    SomeName = "sup bro",
                    Longtext = "I AM THE GREATEST",
                    SomeDate = DateTime.Now.AddDays(21)
                },
                new GridData
                {
                    RowId = 5,
                    SomeName = "GridCommand",
                    Longtext = "List<GridData>",
                    SomeDate = DateTime.Now.AddDays(-3)
                }
            };

            return tempData.ToConsoleTable();
        }
    }

    public class GridData
    {
        public int RowId { get; set; }
        public string SomeName { get; set; }
        public string Longtext { get; set; }
        public DateTime SomeDate { get; set; }
    }
}