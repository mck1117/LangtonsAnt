using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangtonsAnt
{
    class Program
    {
        private static Color[] Colors = new Color[]
        {
            Color.White,
            Color.Black,
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Yellow,
            Color.Orange,
            Color.Purple,
            Color.Magenta,
            Color.Cyan,
            Color.Coral,
            Color.Crimson,
            Color.SkyBlue
        };

        static void Main(string[] args)
        {
            Console.Write("Enter directions: ");
            string line = Console.ReadLine().ToUpperInvariant();
            var dirs = (from c in line
                        select (c == 'L') ? TurnDirection.Left : TurnDirection.Right).ToArray();

            Console.Write("Enter steps: ");
            int steps = int.Parse(Console.ReadLine());

            var grid = new ExtendingGrid<int>(50, 50, () => 0);

            Ant ant = new Ant(dirs, grid);

            for (int i = 0; i < steps; i++)
            {
                ant.Step();
            }

            Console.WriteLine("Ant done, generate image.");

            GridToBitmap(ant.Grid, Colors).Save("C:\\Users\\Matthew\\Desktop\\img\\" + line + "_" + steps + ".png");

            Console.WriteLine("Done!");
        }

        private static Bitmap GridToBitmap(ExtendingGrid<int> grid, Color[] colors)
        {
            Bitmap saveBitmap = new Bitmap(grid.Width, grid.Height);

            for (int i = 0; i < grid.Width; i++)
            {
                for (int j = 0; j < grid.Height; j++)
                {
                    saveBitmap.SetPixel(i, j, colors[grid[i, j]]);
                }
            }

            return saveBitmap;
        }
    }

    class Ant
    {
        public ExtendingGrid<int> Grid { get; private set; }
        private int x, y;
        private TurnDirection[] turnDirections;
        private Direction currentHeading = Direction.Up;

        public Ant(TurnDirection[] turnDirections, ExtendingGrid<int> grid)
        {
            this.Grid = grid;
            this.turnDirections = turnDirections;
            x = 25;
            y = 25;
        }

        public void Step()
        {
            int state = Grid[x, y];
            TurnDirection dir = turnDirections[state];
            Turn(dir);
            Grid[x, y] = NextState(state);
            Move();
        }

        private void Turn(TurnDirection direction)
        {
            if(direction == TurnDirection.Left)
            {
                switch (currentHeading)
                {
                    case Direction.Up:
                        currentHeading = Direction.Left;
                        break;
                    case Direction.Right:
                        currentHeading = Direction.Up;
                        break;
                    case Direction.Down:
                        currentHeading = Direction.Right;
                        break;
                    case Direction.Left:
                        currentHeading = Direction.Down;
                        break;
                }
            }
            else
            {
                switch (currentHeading)
                {
                    case Direction.Up:
                        currentHeading = Direction.Right;
                        break;
                    case Direction.Right:
                        currentHeading = Direction.Down;
                        break;
                    case Direction.Down:
                        currentHeading = Direction.Left;
                        break;
                    case Direction.Left:
                        currentHeading = Direction.Up;
                        break;
                }
            }
        }

        private void Move()
        {
            switch (currentHeading)
            {
                case Direction.Up:
                    if(y == 0)
                    {
                        Grid.ExtendUp(1);
                    }
                    else
                    {
                        y--;
                    }

                    break;
                case Direction.Down:
                    if(y == Grid.Height - 1)
                    {
                        Grid.ExtendDown(1);
                    }

                    y++;

                    break;
                case Direction.Left:
                    if (x == 0)
                    {
                        Grid.ExtendLeft(1);
                    }
                    else
                    {
                        x--;
                    }
                    break;
                case Direction.Right:
                    if(x == Grid.Width - 1)
                    {
                        Grid.ExtendRight(1);
                    }

                    x++;
                    break;
            }
        }

        private int NextState(int state)
        {
            return (state + 1) % this.turnDirections.Length;
        }
    }


    enum TurnDirection { Left, Right }

    enum Direction { Up, Down, Left, Right }

    interface IGrid<T>
    {
        T this[int x, int y] { get; set; }

        int Width { get; }
        int Height { get; }
    }

    class ExtendingGrid<T> : IGrid<T>
    {
        private List<List<T>> grid;
        private Func<T> defaultVal;

        public ExtendingGrid(int width, int height, Func<T> defaultValue)
        {
            Width = width;
            Height = height;
            defaultVal = defaultValue;
            grid = new List<List<T>>();

            for (int x = 0; x < width; x++)
            {
                var column = new List<T>();

                for (int y = 0; y < height; y++)
                {
                    column.Add(defaultValue());
                }

                grid.Add(column);
            }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public T this[int x, int y]
        {
            get
            {
                if (y >= Height || y < 0)
                {
                    throw new ArgumentException("Y out of range.");
                }
                if (x >= Width || x < 0)
                {
                    throw new ArgumentException("X out of range.");
                }

                return grid[x][y];
            }
            set
            {
                if (y >= Height || y < 0)
                {
                    throw new ArgumentException("Y out of range.");
                }
                if (x >= Width || x < 0)
                {
                    throw new ArgumentException("X out of range.");
                }

                grid[x][y] = value;
            }
        }

        public void ExtendUp(int count)
        {
            foreach (var c in grid)
            {
                for (int i = 0; i < count; i++)
                {
                    c.Insert(0, defaultVal());
                }
            }

            Height += count;
        }

        public void ExtendLeft(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var newColumn = new List<T>();

                for (int j = 0; j < Height; j++)
                {
                    newColumn.Add(defaultVal());
                }

                grid.Insert(0, newColumn);
            }

            Width += count;
        }

        public void ExtendRight(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var newColumn = new List<T>();

                for (int j = 0; j < Height; j++)
                {
                    newColumn.Add(defaultVal());
                }

                grid.Add(newColumn);
            }

            Width += count;
        }

        public void ExtendDown(int count)
        {
            foreach(var c in grid)
            {
                for (int i = 0; i < count; i++)
                {
                    c.Add(defaultVal());
                }
            }

            Height += count;
        }
    }
}
