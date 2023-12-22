using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMen
{
    public class Gost
    {
        public Gost(int x,int y,char c)
        {
            X= x; Y = y; Symbol = c;
        }
        public char Symbol { get; set; }
        //make X/Y coordinates
        public int X { get; set; }
        public int Y { get; set; }
    }
}
