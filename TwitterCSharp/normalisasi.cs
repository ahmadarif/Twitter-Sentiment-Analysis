using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterCSharp
{
    public class normalisasi
    {
        string hasil;
        public string normal(string kata)
        {
            string text = kata;
            string[] words = text.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == "rt")
                {
                   
                    words[i] = words[i].Remove(0);
                }
                if (words[i] == "kemenkp")
                {
                    
                    words[i] = words[i].Remove(0);
                }
                if (words[i].IndexOf("@") != -1)
                {
                    words[i] = words[i].Remove(0);
                                   }
                if (words[i].IndexOf("http") != -1)
                {
                    words[i] = words[i].Remove(0);
                }
                if (words[i].IndexOf("#") != -1)
                {
                    words[i] = words[i].Remove(0);
                }
                hasil = String.Join(" ", words).Replace("  ", " ");
                hasil = hasil.ToString().Replace("  ", " ");
                hasil = hasil.ToString().Replace("  ", " ");
                hasil = hasil.Trim();
            }
            return hasil;
        }
    }
}
