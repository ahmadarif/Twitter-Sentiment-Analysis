using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterCSharp
{
    public class tokenizing
    {
        public string token(string kata)
        {
            string text = kata;
            text = text.Replace("…", " ");
            text = text.Replace("_", " ");
            text = text.Replace(".", " ");
            text = text.Replace(",", " ");
            text = text.Replace("-", " ");
            text = text.Replace("?", " ");
            text = text.Replace("!", " ");
            text = text.Replace("=", " ");
            text = text.Replace(":", " ");
            text = text.Replace("*", " ");
            text = text.Replace("+", " ");
            text = text.Replace("\"", " ");
            text = text.Replace("0", " ");
            text = text.Replace("1", " ");
            text = text.Replace("2", " ");
            text = text.Replace("3", " ");
            text = text.Replace("4", " ");
            text = text.Replace("5", " ");
            text = text.Replace("6", " ");
            text = text.Replace("7", " ");
            text = text.Replace("8", " ");
            text = text.Replace("9", " ");
            text = text.Replace("(", " ");
            text = text.Replace(")", " ");
            text = text.Replace("@", " ");
            text = text.Replace("#", " ");
            text = text.Replace("%", " ");
            text = text.Replace("^", " ");
            text = text.Replace("&", " ");
            text = text.Replace("'", " ");
            text = text.Replace("  ", " ");
            text = text.Replace("  ", " ");
            text = text.Replace("  ", " ");
            text = text.Trim();
            return text;
           
        }
    }
}
