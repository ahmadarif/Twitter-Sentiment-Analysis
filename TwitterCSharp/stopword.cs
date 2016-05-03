using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterCSharp
{
    public class stopword
    {
        public string stopWord(string kata)
        {
            string text = kata;
            string[] words = text.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == "dan")
                {
                    
                    words[i] = words[i].Remove(0);
                }
                if (words[i] == "di")
                {
                    
                    words[i] = words[i].Remove(0);
                }
                if (words[i] == "ingin")
                {
                    
                    words[i] = words[i].Remove(0);
                }
                if (words[i] == "ini")
                {
                    words[i] = words[i].Remove(0);
                }
                if (words[i] == "kepada")
                {
                    words[i] = words[i].Remove(0);
                }
                if (words[i] == "dalam")
                {
                    words[i] = words[i].Remove(0);
                }
                if (words[i] == "selalu")
                {
                    words[i] = words[i].Remove(0);
                }
                if (words[i] == "lalu")
                {
                    words[i] = words[i].Remove(0);
                }
                if (words[i] == "yaitu")
                {
                    words[i] = words[i].Remove(0);
                }
                if (words[i] == "bahwa")
                {
                    words[i] = words[i].Remove(0);
                }
                if (words[i] == "terdiri")
                {
                    words[i] = words[i].Remove(0);
                }
                if (words[i] == "sekali")
                {
                    words[i] = words[i].Remove(0);
                }
                if (words[i] == "dulu")
                {
                    words[i] = words[i].Remove(0);
                }
                if (words[i] == "sekalian")
                {
                    words[i] = words[i].Remove(0);
                }
                if (words[i] == "enggak")
                {
                    words[i] = words[i].Remove(0);
                }
                if (words[i] == "bagian")
                {
                    words[i] = words[i].Remove(0);
                }              
                if (words[i].Length == 1 || words[i].Length == 2)
                {
                    
                    words[i] = words[i].Remove(0);
                }

            }
            string hasil = String.Join(" ", words);
            hasil = hasil.Replace("  ", " ");
            hasil = hasil.Replace("  ", " ");
            hasil = hasil.Replace("  ", " ");
            hasil = hasil.Trim();
            return hasil;
        }
    }
}
