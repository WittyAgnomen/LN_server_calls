//Needed to write to csv
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WSKCons
{
    /// <summary>
    /// Class to store one CSV row
    /// </summary>
    public class CsvRow : List<string>
    {
        public string LineText { get; set; }
    }
}
