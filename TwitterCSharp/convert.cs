using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterCSharp
{
    public class convert
    {

        public string convr(string kata)
        {
            string text = kata;
            text = text.Replace(">:]", "senang");
            text = text.Replace(":-)", "senang");
            text = text.Replace(":)", "senang");
            text = text.Replace(":0)", "senang");
            text = text.Replace(":]", "senang");
            text = text.Replace(":3", "senang");
            text = text.Replace(":c)", "senang");
            text = text.Replace(":>", "senang");
            text = text.Replace("=]", "senang");
            text = text.Replace("8)", "senang");
            text = text.Replace("=)", "senang");
            text = text.Replace(":}", "senang");
            text = text.Replace("(~^.^)~", "senang");
            text = text.Replace("^_^", "senang");
            text = text.Replace("^^", "senang");
            text = text.Replace("^ ^", "senang");
            text = text.Replace(":^)", "senang");
            text = text.Replace(">:D", "senang");
            text = text.Replace(":-D", "senang");
            text = text.Replace(":D", "senang");
            text = text.Replace(":))", "senang");
            text = text.Replace("=))", "senang");
            text = text.Replace("8-D)", "senang");
            text = text.Replace("8D", "senang");
            text = text.Replace("x-D", "senang");
            text = text.Replace("xD", "senang");
            text = text.Replace("=-D", "senang");
            text = text.Replace("=D", "senang");
            text = text.Replace("=-3", "senang");
            text = text.Replace("=3", "senang");        
            text = text.Replace(">:[", "sedih");
            text = text.Replace(":-(", "sedih");
            text = text.Replace(":(", "sedih");
            text = text.Replace(":-c)", "sedih");
            text = text.Replace(":c", "sedih");
            text = text.Replace(":-<", "sedih");
            text = text.Replace(":<", "sedih");
            text = text.Replace(":-[", "sedih");
            text = text.Replace(":[", "sedih");
            text = text.Replace(":{", "sedih");
            text = text.Replace(">.>", "sedih");
            text = text.Replace("<.<", "sedih");
            text = text.Replace(">.<", "sedih");
            text = text.Replace("%", " persen");
            text = text.Replace("/", " atau ");
            text = text.Replace("&amp;", "");
            text = text.Replace("&gt;", "");
            text = text.Replace("&lt;", "");
            return text;
        }
    }
}
