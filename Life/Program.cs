using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.Json;

namespace cli_life
{
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }
    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);
        }

        public Board(int width, int height, int cellSize)
        {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
        }

        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        public int AliveCount()
        {
            int count = 0;
            foreach (var cell in Cells)
            {
                if (cell.IsAlive)
                    count++;
            }
            return count;
        }
        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }
        private void ConnectNeighbors()
        {
            for (int x = 1; x < Columns - 1; x++)
            {
                for (int y = 1; y < Rows - 1; y++)
                {
                    int xL = x - 1;
                    int xR = x + 1;
                    int yT = y - 1;
                    int yB = y + 1;

                    Cells[x, y].neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].neighbors.Add(Cells[x, yT]);
                    Cells[x, y].neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].neighbors.Add(Cells[xL, y]);
                    Cells[x, y].neighbors.Add(Cells[xR, y]);
                    Cells[x, y].neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].neighbors.Add(Cells[x, yB]);
                    Cells[x, y].neighbors.Add(Cells[xR, yB]);
                }
            }
        }
    }

    public class Settings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Delay { get; set; }
    }

    public class Figure
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
        public int[] Matrix { get; set; }
        
        public int[,] Matrix2D()
        {
            int[,] mas = new int[Width, Height];
            int n = 0;
            for (int i=0; i<Height; i++)
            {
                for(int j=0; j<Width; j++)
                {
                    mas[i, j] = Matrix[n];
                    n++;
                }
            }
            return mas;
        }
    }

    class Program
    {
        static Board board;

        static Figure[] Get_fig()
        {
            string filename = "figure.json";
            string jsonString = File.ReadAllText(filename);
            Figure[] figure = JsonSerializer.Deserialize<Figure[]>(jsonString);
            return figure;
        }

        static int Find(Figure fig, Board b)
        {
            int count = 0;
            int[,] matr = new int[fig.Width, fig.Height];
            int[,] fmatr = fig.Matrix2D();
            for (int row = 0; row < b.Rows-fig.Height; row++)
            {
                for (int col = 0; col < b.Columns - fig.Width; col++)
                {
                    for (int i = 0; i < fig.Height; i++)
                    {
                        for (int j = 0; j < fig.Width; j++)
                        {
                            if (b.Cells[col+j, row+i].IsAlive)
                            {
                                matr[i, j] = 1;
                            }
                            else
                                matr[i, j] = 0;
                        }
                    }
                    count += Compare(matr, fmatr);
                }
            }
            return count;
        }

        static int Compare(int[,] matr1, int[,] matr2)
        {
            int res = 1;
            int n = matr1.GetUpperBound(0) + 1;
            int m = matr1.Length / n;
            for (int i =0; i<n; i++)
            {
                for(int j=0; j<m; j++)
                {
                    if (matr1[i, j] != matr2[i, j])
                        res = 0;
                }
            }
            return res;
        }
        static private void Reset()
        {
            string filename = "config.json";
            string jsonString = File.ReadAllText(filename);
            Settings settings = JsonSerializer.Deserialize<Settings>(jsonString);
            Thread.Sleep(1000);
            board = new Board(
                width: settings.Width,
                height: settings.Height,
                cellSize: 1,
                liveDensity: 0.5);
        }
        static void Render()
        {
            for (int row = 1; row < board.Rows - 1; row++)
            {
                for (int col = 1; col < board.Columns - 1; col++)
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        Console.Write('*');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
        }

        static void Save(int flag)
        {
            string filename;
            if (flag == 1)
            {
                filename = "Board.txt";
            }
            else
                filename = "Copy_board.txt";
            
            StreamWriter sw = new StreamWriter(filename);
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        sw.Write('1');
                    }
                    else
                    {
                        sw.Write('0');
                    }
                }
                sw.Write('\n');
            }
            sw.Close();
        }

        static int Simm()
        {
            int res = 1;
            int n = board.Rows;
            int m = board.Columns;
            for (int row = 0; row < n/2; row++)
            {
                for (int col = 0; col < m/2; col++)
                {
                    if (board.Cells[col, row].IsAlive != board.Cells[m - col-1, row].IsAlive)
                        return 0;
                
                }
            }
            return res;
        }
        static Board Copy()
        {
            int w = board.Columns;
            int h = board.Rows;
            Board copy = new Board(w, h, 1);
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    copy.Cells[col, row].IsAlive = board.Cells[col, row].IsAlive;
                }
            }
            return copy;
        }

        static int Stop(Board copy, Board b)
        {
            int flag = 1;
            for (int row = 0; row < b.Rows; row++)
            {
                for (int col = 0; col < b.Columns; col++)
                {
                    var a = b.Cells[col, row].IsAlive;
                    var c = copy.Cells[col, row].IsAlive;
                    if (b.Cells[col, row].IsAlive != copy.Cells[col, row].IsAlive)
                    {
                        flag = -1;
                    }
                }
            }
            return flag;
        }

        static void Upload()
        {
            StreamReader sr = new StreamReader("Board1.txt");
            board = new Board(
                width: 10,
                height: 10,
                cellSize: 1);
            string str = sr.ReadToEnd();
            int n = 0;
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    if (str[n] == '1')
                    {
                        board.Cells[col, row].IsAlive = true;
                    }
                    if (str[n] == '0')
                    {
                        board.Cells[col, row].IsAlive = false;
                    }
                    n++;
                }
                n += 2;
            }
            sr.Close();
            
        }

        static void Main(string[] args)
        {
            int k = 0;
            int n;
            int flag = 0;
            Reset();
            Figure[] fig = Get_fig();
            int len = fig.Length;
            Figure block = fig[0];
            Board bend;
            int count;
            string name;
            //Upload();
            while (flag != 1)
            {
                Console.Clear();
                Render();
                n = board.AliveCount();
                Board copy = Copy();
                board.Advance();
                Board b = Copy();
                flag = Stop(copy, b);
                k++;
                int simm;
                if (k == 50)
                {
                    Console.WriteLine("Alive cells: " + n);
                    simm = Simm();
                    if (simm == 1)
                        Console.WriteLine("board have simmetry");
                    else
                        Console.WriteLine("board dont have simmetry");
                    bend = Copy();
                    for(int i=0; i<len; i++)
                    {
                        name = fig[i].Name;
                        count = Find(fig[i], bend);
                        Console.WriteLine("Count " + name + " = " + count);
                    }
                    Console.WriteLine("Save board to file? y/n");
                    string ans = Console.ReadLine();
                    if (ans == "y")
                    {
                        Save(1);
                        Console.WriteLine("Board have been saved");
                    }
                    k = 0;
                }
                Thread.Sleep(500);
            }
            Console.WriteLine("Stable state reached");
        }
    }
}
