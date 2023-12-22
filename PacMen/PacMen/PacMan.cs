using PacMen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PacMan
{
    internal class PacMan
    {
        //window setting
        const int WindowHeight = 15;
        const int WindowWidth = 40;
        //variables
        int TickTime = 150;
        int framePacman = 0;
        int score = 0;
        int countStar = 0;
        string name = "Scorebord.txt";
        //coordinats
        int positionX = 0;
        int positionY = 0;
        //ways
        bool upWay = false;
        bool downWay = false;
        bool rightWay = false;
        bool leftWay = false;
        //switcher
        bool CanMove = true;

        //features
        public bool GameOver { get; private set; }
        public bool LevelComplite { get; private set; }

        Random r = new Random();

        //frames
        char[] pacmanFrames = { 'C', 'O' };
        //game objects
        public char[] elements = { '█', ' ', '*','C','#','^'};
        List<Gost> gosts = new List<Gost>();
        //levels
        char[,] Level = new char[WindowHeight - 2, WindowWidth];
        
        void StartWindow()
        {
            string[] menuItems = { "Начать", "Лидерборд" };
            int select = 0;

            ConsoleKeyInfo key;

            do
            {
                Console.Clear();
                Console.WriteLine("\n\n\n\n");
                for (int i = 0; i < menuItems.Length; i++)
                {
                    if (i == select)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(new String(' ',(WindowWidth/2) - (menuItems[i].Length / 2)) + $"{menuItems[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(new String(' ',(WindowWidth/2) - (menuItems[i].Length / 2)) + $"{menuItems[i]}");
                    }
                }
                key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.W:
                        if(select < 1)
                        {
                            select = menuItems.Length - 1;
                        }
                        else
                        {
                            select--;
                        }
                        break;
                    case ConsoleKey.S:
                        if(select > menuItems.Length - 2)
                        {
                            select = 0;
                        } else
                        {
                            select++;
                        }
                        break;
                    case ConsoleKey.Enter:
                        switch (select)
                        {
                            case 0:
                                return;
                            case 2:
                                Console.Clear();
                                PressAnyButton();
                                break;
                            case 1:
                                Process.Start(name);
                                break;
                        }  
                        break;
                }
            } while (key.Key != ConsoleKey.Escape);
        }
        
        
        //reset game
        public void Reset()
        {
            GameOver = false;
            score = 0;
            countStar = 0;
        }

        //start game
        public void Game()
        {
            InitializeGame();
            while(!GameOver)
            {
                //update every frame
                Console.Clear();
                Update();
                Render();
                ProcessInput();
                Thread.Sleep(TickTime);//tick delay
            }
            Console.Clear();

            using (StreamWriter sw = new StreamWriter(name,true))
            {
                sw.WriteLine($"Score: {score}");
            }

            Console.WriteLine("\n\n\n");
            Console.WriteLine(new String(' ', (WindowWidth / 2) - ("Вы проиграли".Length / 2)) + "Вы проиграли");
            Console.WriteLine(new String(' ', (WindowWidth / 2) - ("Счёт:".Length / 2)) + "Счёт:");
            Console.WriteLine(new String(' ', (WindowWidth / 2) - ($"{score}".Length / 2)) + $"{score}");
            GameOver = true;
            PressAnyButton();
        }
        //Initialize game

        public void InitializeGame()
        {
            Console.CursorVisible = false;
            Console.Title = "PacMan";
            //set window values
            Console.WindowHeight = WindowHeight;
            Console.BufferHeight = WindowHeight;

            Console.WindowWidth = WindowWidth;
            Console.BufferWidth = WindowWidth;

            if (!File.Exists(name))
            {
                File.Create(name).Close();
            }

            GenLevel();
            StartWindow();
        }
        //updats every frame
        void Update()
        {
            //pacman`s animation
            if (framePacman >= 1)
            {
                framePacman = 0;
            }
            else
            {
                framePacman++;
            }
            //add score
            if(score > 999999999)
            {
                score = 999999999;
            }
            //bots
            BotLogic();
            //moving player
            Move();
        }
        //priccesing game onject
        public void Render()
        {
            DisplayLevel();
        }
        //check input every frame
        void ProcessInput()
        {
            //PacMan`s controller 
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo consoleKey = Console.ReadKey();
                switch(consoleKey.Key)
                {
                    case ConsoleKey.W:
                        upWay = true;
                        downWay = false;
                        rightWay = false;
                        leftWay = false;
                        break;
                    case ConsoleKey.S:
                        upWay = false;
                        downWay = true;
                        rightWay = false;
                        leftWay = false;
                        break;
                    case ConsoleKey.D:
                        upWay = false;
                        downWay = false;
                        rightWay = true;
                        leftWay = false;
                        break;
                    case ConsoleKey.A:
                        upWay = false;
                        downWay = false;
                        rightWay = false;
                        leftWay = true;
                        break;
                    case ConsoleKey.E://debug moment
                        CanMove = true;
                        break;
                    case ConsoleKey.Q:
                        GameOver = true;
                        break;
                    case ConsoleKey.Z:
                        score += 1000;
                        break;
                    case ConsoleKey.Escape:
                        Console.Clear();
                        MetaStatistic();
                        PressAnyButton();
                        break;
                    default:
                        upWay = false;
                        downWay = false;
                        rightWay = false;
                        leftWay = false;
                        break;
                }

            }
        }


        void BotLogic()
        {
            char tempChar;
            Random n = new Random();
            int directions = n.Next(1, 3);

            foreach (var bot in gosts)
            {

                switch (directions)
                {
                    case 1:
                        if(bot.X < Level.GetLength(0) - 2)
                        {
                            bot.X++;
                            Level[bot.X - 1, bot.Y] = elements[5];
                        }
                        else
                        {
                            Level[Level.GetLength(0) - 2, bot.Y] = elements[5];
                            bot.X = 1;
                        }
                        if (Level[bot.X + 1,bot.Y] == pacmanFrames[0] | Level[bot.X + 1, bot.Y] == pacmanFrames[1])
                        {
                            GameOver = true;
                        }
                            break;
                    case 2:
                        if (bot.Y < Level.GetLength(1) - 2)
                        {
                            bot.Y++;
                            Level[bot.X, bot.Y - 1] = elements[5];
                        }
                        else
                        {
                            Level[bot.X, Level.GetLength(1) - 2] = elements[5];
                            bot.Y = 1;
                        }
                        if (Level[bot.X, bot.Y + 1] == pacmanFrames[0] | Level[bot.X, bot.Y + 1] == pacmanFrames[1])
                        {
                            GameOver = true;
                        }
                        break;
                }
            }
        }





        //random spawn entites
        void SpawnEnities()
        {
            //random coorditanats
            int tempPosX = r.Next(1, Level.GetLength(0) - 1);
            int tempPosY = r.Next(1, Level.GetLength(1) - 1);

            positionX = tempPosX;
            positionY = tempPosY;

            Gost gost = new Gost(r.Next(2, Level.GetLength(0) - 2), r.Next(2, Level.GetLength(1) - 2), elements[4]);
            Gost gost1 = new Gost(r.Next(2, Level.GetLength(0) - 2), r.Next(2, Level.GetLength(1) - 2), elements[4]);
            Gost gost2 = new Gost(r.Next(2, Level.GetLength(0) - 2), r.Next(2, Level.GetLength(1) - 2), elements[4]);
            Gost gost3 = new Gost(r.Next(2, Level.GetLength(0) - 2), r.Next(2, Level.GetLength(1) - 2), elements[4]);
            gosts.Add(gost);
            gosts.Add(gost1);
            gosts.Add(gost2);
            gosts.Add(gost3);
        }
        //moving player
        void Move()
        {
 
            //check walls
            if (Level[positionX - 1, positionY] == elements[0])
            {
                upWay = false;
            }
            if (Level[positionX + 1, positionY] == elements[0])
            {
                downWay = false;
            }
            if (Level[positionX, positionY + 1] == elements[0])
            {
                rightWay = false;
            }
            if (Level[positionX, positionY - 1] == elements[0])
            {
                leftWay = false;
            }

            if (Level[positionX - 1,positionY] == elements[4])
            {
                GameOver = true;
            }
            if (Level[positionX + 1, positionY] == elements[4])
            {
                GameOver = true;
            }
            if (Level[positionX, positionY - 1] == elements[4])
            {
                GameOver = true;
            }
            if (Level[positionX, positionY + 1] == elements[4])
            {
                GameOver = true;
            }
            if (Level[positionX,positionY] == elements[4])
            {
                GameOver = true;
            }

            //moving
            if (CanMove & upWay)//move up
            {
                Level[positionX, positionY] = elements[1];
                
                if (CanMove & upWay)
                {
                    CanMove = true;
                    positionX--;
                    if (Level[positionX - 1,positionY] == elements[2])
                    {
                        score++;
                    } 
                    if (Level[positionX - 1, positionY] == elements[5])
                    {
                        score += 5;
                    }
                } else 
                {
                    CanMove = false;
                }
            } else if(CanMove & downWay)//move down
            {
                Level[positionX, positionY] = elements[1];

                if (CanMove & downWay)
                {
                    CanMove = true;
                    positionX++;
                    if (Level[positionX + 1, positionY] == elements[2])
                    {
                        score++;
                    }
                    if (Level[positionX + 1, positionY] == elements[5])
                    {
                        score += 5;
                    }
                }
                else
                {
                    CanMove = false;
                }

            } else if(CanMove & rightWay)//move right
            {
                Level[positionX, positionY] = elements[1];

                if (CanMove & rightWay)
                {
                    CanMove = true;
                    positionY++;
                    if (Level[positionX, positionY + 1] == elements[2])
                    {
                        score++;
                    }
                    if (Level[positionX, positionY + 1] == elements[5])
                    {
                        score += 5;
                    }
                }
                else
                {
                    CanMove = false;
                }
            } else if(CanMove & leftWay)//move left
            {
                Level[positionX, positionY] = elements[1];

                if (CanMove & leftWay)
                {
                    CanMove = true;
                    positionY--;
                    if (Level[positionX, positionY - 1] == elements[2])
                    {
                        score++;
                    } 
                    if (Level[positionX, positionY + 1] == elements[5])
                    {
                        score += 5;
                    }
                }
                else
                {
                    CanMove = false;
                }
            }
        }
        //output matrix Level
        void DisplayLevel()
        {
            Level[positionX, positionY] = pacmanFrames[framePacman];
            foreach(var bot in gosts)
            {
                Level[bot.X, bot.Y] = elements[4];
            }
            int temp = 0;
            //star count
            for (int i = 0; i < Level.GetLength(0); i++)
            {
                for (int j = 0; j < Level.GetLength(1); j++)
                {
                    ColorChar(Level[i, j]);
                    if(Level[i, j] == elements[2])
                    {
                        temp++;
                    }
                }
            }
            countStar = temp;
            temp = 0;
            Statistic();
        }
        //generate level
        void GenLevel(int type = 0)
        {
            Random r = new Random();
            switch (type) 
            {
                case 0:
                    for(int i = 0; i < Level.GetLength(0); i++)
                    {
                        for(int j = 0;j < Level.GetLength(1); j++)
                        {
                            //fill random symbols
                            Level[i,j] = GenChanceSymbol();
                        }
                    }
                    for (int i = 0; i < Level.GetLength(0); i++)
                    {
                        if(i == 0)
                        {
                            for (int j = 0; j < Level.GetLength(1); j++)
                            {
                                //make first row
                                Level[i, j] = elements[0];
                            }
                        } else if(i == Level.GetLength(0) - 1)
                        {
                            for (int j = 0; j < Level.GetLength(1); j++)
                            {
                                //make last row
                                Level[i, j] = elements[0];
                            }
                        }
                        else
                        {
                            for (int j = 0; j < Level.GetLength(1); j++)
                            {
                                //make first and last column
                                if(j == 0 | j == Level.GetLength(1) - 1)
                                {
                                    Level[i, j] = elements[0];
                                } 
                            }
                        }
                    }
                    //fill void
                    for (int i = 1; i < Level.GetLength(0) - 1; i++)
                    {
                        for (int j = 1; j < Level.GetLength(1) - 1; j++)
                        {
                            if (Level[i, j] == elements[2])
                            {
                                if ((Level[i - 1, j] == elements[0] & Level[i + 1, j] == elements[0] & Level[i, j - 1] == elements[0] & Level[i, j + 1] == elements[0]) |//around
                                    (Level[i + 1, j] == elements[0] & Level[i, j - 1] == elements[0] & Level[i, j + 1] == elements[0]) |//top
                                    (Level[i - 1, j] == elements[0] & Level[i, j - 1] == elements[0] & Level[i, j + 1] == elements[0]) |//bottom
                                    (Level[i - 1, j] == elements[0] & Level[i + 1, j] == elements[0] & Level[i, j + 1] == elements[0]) |//left
                                    (Level[i, j - 1] == elements[0] & Level[i, j + 1] == elements[0]) |//rigt and left
                                    (Level[i - 1, j] == elements[0] & Level[i + 1, j] == elements[0] & Level[i, j - 1] == elements[0]))//right
                                {
                                    Level[i, j] = elements[0];
                                }
                            }
                        }
                    }
                    //spawn entites on random coordinate
                    SpawnEnities();
                break;
                
            }
        }
        //can control chance generate walls 
        char GenChanceSymbol()
        {
            int temp = r.Next(100);
            if(temp < 15)
            {
                return elements[0];
            } else if(temp >= 15 & temp < 90)
            {
                return elements[2];
            }
            else
            {
                return elements[2];
            }
        }
        //outputs statistic game
        void Statistic()
        {
            //string str = $"t:{upWay}|d:{downWay}|r:{rightWay}|l:{leftWay}|{CanMove}";
            string str = String.Format("Score: {0,-10:000,000,000}         FPS: {1,0:0.00}", score, Math.Round((double)1000 / (double)TickTime, 2));
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(str);
            Console.ResetColor();
        }

        void MetaStatistic()
        {
            Console.WriteLine($"star: {countStar}");
            Console.WriteLine($"{gosts.Count}");
        }

        //outputs the colored symbols
        void ColorChar(char c)
        {
            switch (c)
            {
                case '█':
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.Write(elements[0]);
                    Console.ResetColor();
                    break;
                case ' ':

                    Console.Write(elements[1]);
                    break;
                case '*':
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(elements[2]);
                    Console.ResetColor();
                    break;
                case 'C':
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.Write(pacmanFrames[0]);
                    Console.ResetColor();
                    break;
                case 'O':
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.Write(pacmanFrames[1]);
                    Console.ResetColor();
                    break;
                case '#':
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.BackgroundColor = ConsoleColor.Cyan;
                    Console.Write(elements[4]);
                    Console.ResetColor();
                    break;
                case '^':
                    Console.ForegroundColor = ConsoleColor.Blue;
                    //Console.BackgroundColor = ConsoleColor.Cyan;
                    Console.Write(elements[5]);
                    Console.ResetColor();
                    break;
                default:
                    Console.Write(c);
                    break;
            }
        }
        //for testing window size
        public void Test()
        {
            Random r = new Random();
            char[,] matrix = new char[WindowHeight,WindowWidth];
            for(int i = 0; i < matrix.GetLength(0); i++)
            {
                for(int n = 0; n < matrix.GetLength(1); n++)
                {
                    matrix[i, n] = '#';//elements[r.Next(elements.Length)];
                }
            }
            for (int i = 0; i < matrix.GetLength(0); i++) 
            {
                for (int n = 0; n < matrix.GetLength(1); n++)
                {
                    Console.Write(matrix[i,n]);
                }
            }
        }
        void PressAnyButton(bool t = false)
        {
            if (!t)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Нажмите любую кнопку для продолжения...");
                Console.ReadKey();
                Console.ResetColor();
            } else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Нажмите любую кнопку для продолжения...");
                Console.ReadLine();
                Console.ResetColor();
            }
        }

    }
}
