using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameOfLife
{
    class World
    {
        // the worl is a one dimensional array, for simpicity
        public bool[] worldState;
        // width of world
        private int cellX;
        //heigth of world
        private int cellY;
        //appeareance of cells
        private string aliveString = "# ";
        private string deadString = ". ";


        // Inizialize the "world", made of X time Y cells. like a cartesian graph
        public World(int numCellX, int numCellY)
        {
            this.cellX = numCellX;
            this.cellY = numCellY;
            // It's all initialized to false, which means no life in the cell
            this.worldState = new bool[cellX * cellY];
        }

        //Get the state of a cell at (x,y)
        public bool getStateAt(int x, int y)
        {
            int index = coordToIndex(x, y);
            if (index < 0 || index >= cellX * cellY)
            {
                return false;
            }
            return worldState[index];
        }

        // Change the state of the cell at (x,y) with bool newState 
        // (use a different array not to mess with coutning neighboors)
        public void changeState(int x, int y, bool newState, bool[] state)
        {
            int index = coordToIndex(x, y);
            state[index] = newState;
        }

        // Print the actual world state on the screen
        public void printWorld()
        {
            
            string toPrint = "";
            int i = 0;
            //Could be done with this oneliner, but it's a bit messy and I dopn't really trust it
            //Array.ForEach(worldState, x =>  Console.WriteLine(x ? "T " : "F ") )
            for (i = 0; i < worldState.Length; i++)
            {
                if (i % this.cellY == 0 && i != 0)
                {
                    toPrint+= "\n";
                }
                toPrint += worldState[i] ? aliveString : deadString;
            }
            toPrint += "\n";
            Console.Clear();
            Console.Write(toPrint);
        }
    
        // Get the number of alive cells in the neighborhood
        public int aliveCloseTo(int x, int y)
        {
            int counter = 0;
            //N
            if (getStateAt(x, y - 1) == true) counter++;
            //E
            if (getStateAt(x - 1, y) == true) counter++;
            //W
            if (getStateAt(x + 1, y) == true) counter++;
            //S
            if (getStateAt(x, y + 1) == true) counter++;
            //NE
            if (getStateAt(x - 1, y - 1) == true) counter++;
            //NW
            if (getStateAt(x + 1, y - 1) == true) counter++;
            //SE
            if (getStateAt(x - 1, y + 1) == true) counter++;
            //SW
            if (getStateAt(x + 1, y + 1) == true) counter++;
            return counter;
        }

        // Decide if a cell should live (if the cell is void, decide if life should be born,
        // if the cell is alive, decide if it should keep living)
        public bool shouldLive(int x, int y)
        {
            //Uccidi una cellula con meno di due o più di tre cellule vive vicino
            if (getStateAt(x,y) == true)
            {
                if (aliveCloseTo(x,y) < 2 || aliveCloseTo(x,y) > 3)
                {
                    return false;
                }
                return true;
            }
            //Genera una cellula se ce ne sono tre esattamente vicine
            if (getStateAt(x, y) == false)
            {
                if (aliveCloseTo(x,y) == 3)
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        // From index to (x,y) coord
        public int coordToIndex(int x, int y)
        {
            return x + y * cellX;
        }

        //Advance one turn
        public void oneTurn()
        {
            var newState = (bool[])worldState.Clone();
            for (int x = 0; x < cellX; x++)
            {
                for (int y = 0; y < cellY; y++)
                {
                    if (shouldLive(x, y))
                    {
                        changeState(x, y, true, newState);
                    } else
                    {
                        changeState(x, y, false, newState);
                    }
                }
            }
            worldState = newState;
            printWorld();
        }

        //Choose the layout of alive cells at the start (really really buggy and trippy)
        public void chooseStart()
        {
            int xtemp, ytemp;
            int selector = 0;
            string toPrint = "\n";
            var pressedButton = new ConsoleKeyInfo();
            while (pressedButton.Key != ConsoleKey.Escape) {
                toPrint = "";
                for (int i = 0; i < worldState.Length; i++)
                {
                    if (i % this.cellY == 0 && i != 0)
                    {
                        toPrint += "\n";
                    }
                    if (i == selector)
                    {
                        toPrint += "X ";
                    }
                    else
                    {
                        toPrint += worldState[i] ? aliveString : deadString;
                    }
                }
                toPrint += "\n";
                Console.Clear();
                Console.Write(toPrint);
                pressedButton = Console.ReadKey();
                //Enter
                if (pressedButton.Key == ConsoleKey.Enter)
                {
                    xtemp = selector % cellX;
                    ytemp = selector / cellY;
                    if (getStateAt(xtemp, ytemp))
                    {
                        changeState(xtemp, ytemp, false, worldState);
                    } else
                    {
                        changeState(xtemp, ytemp, true, worldState);
                    }
                }
                //^
                if (pressedButton.Key == ConsoleKey.UpArrow)
                {
                    selector = Math.Max(selector - cellX, 0);
                }
                //v
                if (pressedButton.Key == ConsoleKey.DownArrow)
                {
                    selector = Math.Min(selector + cellX, cellX*cellY - 1);
                }
                //->
                if (pressedButton.Key == ConsoleKey.RightArrow)
                {
                    selector = Math.Min(selector + 1, cellX * cellY - 1);
                }
                //<-
                if (pressedButton.Key == ConsoleKey.LeftArrow)
                {
                    selector = Math.Max(selector -1, 0);
                }
            }
        }
        
        // Start the simulation for n turns
        public void start(int delay, int turns)
        {
            if (turns == 0)
            {
                for (;;)
                {
                    oneTurn();
                    Thread.Sleep(delay);
                }
            } else
            {
                for (int i = 0; i < turns; i++)
                {
                    oneTurn();
                    Thread.Sleep(delay);
                }
            }
        }

    }


    class Program
    {
        static void Main(string[] args)
        {
            string banner = @"

 ________  ________  _____ ______   _______           ________  ________     
|\   ____\|\   __  \|\   _ \  _   \|\  ___ \         |\   __  \|\  _____\    
\ \  \___|\ \  \|\  \ \  \\\__\ \  \ \   __/|        \ \  \|\  \ \  \__/     
 \ \  \  __\ \   __  \ \  \\|__| \  \ \  \_|/__       \ \  \\\  \ \   __\    
  \ \  \|\  \ \  \ \  \ \  \    \ \  \ \  \_|\ \       \ \  \\\  \ \  \_|    
   \ \_______\ \__\ \__\ \__\    \ \__\ \_______\       \ \_______\ \__\     
    \|_______|\|__|\|__|\|__|     \|__|\|_______|        \|_______|\|__|     
                                                                             
                                                                             
                                                                             
                    ___       ___  ________ _______                          
                   |\  \     |\  \|\  _____\\  ___ \                         
                   \ \  \    \ \  \ \  \__/\ \   __/|                        
                    \ \  \    \ \  \ \   __\\ \  \_|/__                      
                     \ \  \____\ \  \ \  \_| \ \  \_|\ \                     
                      \ \_______\ \__\ \__\   \ \_______\                    
                       \|_______|\|__|\|__|    \|_______|                    
";
            string banner2 = @"
                         Made by TrinTragula in 2017
            A C# ascii implementation of Conway's game of life
             
                                   Guide:
                    - Place the starting cells with Enter
                    - When ready to go, press Esc

                          (Press any key to start)
";
            Console.WindowWidth = 77;
            Console.WindowHeight = 30;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(banner);
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(banner2);
            Console.ReadKey();
            var delay = 100; //delay in milliseconds between turns
            var width = 50; //width of the field
            var heigth = 50; //heigth of the field
            Console.WindowWidth = width*2+1;
            Console.WindowHeight = heigth+1;
            var mondoProva = new World(width, heigth);

            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkBlue;



            mondoProva.chooseStart();
            mondoProva.start(delay, 0);

        }
    }
}
