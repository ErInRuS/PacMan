using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    internal class Program
    {
        public const int WindowHeight = 15;
        public const int WindowWidth = 40;
        static void Main(string[] args)
        {            
            //initialization game
            PacMan pacMan = new PacMan();
            pacMan.InitializeGame();
            while (true)
            {
                pacMan.Game();
                pacMan.Reset();
            }
        }
    }
}
