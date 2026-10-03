using System;
using System.Collections.Generic;
using System.Drawing;

namespace SnakeGame
{
    public class Food
    {
        // Позиция еды в клетках сетки
        public Point Position { get; private set; }

        public Food()
        {
            Position = new Point(0, 0);
        }

        // Ставит еду в случайную свободную клетку.
        // Возвращает false, если свободных клеток не осталось (змейка заняла всё поле).
        public bool Respawn(Random random, GameSettings settings, List<Point> snakeBody)
        {
            HashSet<Point> occupied = new HashSet<Point>(snakeBody);
            List<Point> freeCells = new List<Point>();

            for (int x = 0; x < settings.GridWidth; x++)
            {
                for (int y = 0; y < settings.GridHeight; y++)
                {
                    Point cell = new Point(x, y);
                    if (!occupied.Contains(cell))
                        freeCells.Add(cell);
                }
            }

            if (freeCells.Count == 0)
                return false;

            Position = freeCells[random.Next(freeCells.Count)];
            return true;
        }
    }
}
