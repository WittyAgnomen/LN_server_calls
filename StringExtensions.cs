//needed to capture portions of search term and write as a string in csv
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSKCons
{
    public static class StringExtensions
    {
        public static string Right(this string str, int length)
        {
            return str.Substring(str.Length - length, length);
        }

    }
}

