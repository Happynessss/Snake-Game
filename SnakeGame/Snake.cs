using System.Collections.Generic;
using System.Drawing;

namespace SnakeGame
{
    public class Snake
    {
        // Направление, в котором змейка двигалась на последнем шаге
        private Direction _currentDirection;

        // Направление, которое будет применено на следующем шаге
        private Direction _nextDirection;

        // Нужно ли вырасти на следующем шаге (хвост не удаляется)
        private bool _growPending;

        // Сегменты тела в клетках сетки. Body[0] — голова.
        public List<Point> Body { get; private set; }

        // Текущее направление движения
        public Direction Direction => _currentDirection;

        // Голова змейки
        public Point Head => Body[0];

        public Snake(Point startPosition, int initialLength)
        {
            Body = new List<Point>();
            _currentDirection = Direction.Right;
            _nextDirection = Direction.Right;

            // Начальное тело: голова + сегменты слева от неё
            for (int i = 0; i < initialLength; i++)
            {
                Body.Add(new Point(startPosition.X - i, startPosition.Y));
            }
        }

        public void ChangeDirection(Direction newDirection)
        {
            // Нельзя развернуться на 180°. Сравниваем с направлением последнего
            // реального шага, поэтому двойное нажатие за один тик не даст разворот.
            if (IsOpposite(_currentDirection, newDirection))
                return;

            _nextDirection = newDirection;
        }

        public void Move()
        {
            _currentDirection = _nextDirection;

            Point head = Head;
            Point newHead;

            switch (_currentDirection)
            {
                case Direction.Up:
                    newHead = new Point(head.X, head.Y - 1);
                    break;
                case Direction.Down:
                    newHead = new Point(head.X, head.Y + 1);
                    break;
                case Direction.Left:
                    newHead = new Point(head.X - 1, head.Y);
                    break;
                default:
                    newHead = new Point(head.X + 1, head.Y);
                    break;
            }

            Body.Insert(0, newHead);

            if (_growPending)
                _growPending = false;               // хвост остаётся — змейка выросла
            else
                Body.RemoveAt(Body.Count - 1);      // обычный шаг — убираем хвост
        }

        public void Grow()
        {
            _growPending = true;
        }

        private static bool IsOpposite(Direction a, Direction b)
        {
            return (a == Direction.Up && b == Direction.Down)
                || (a == Direction.Down && b == Direction.Up)
                || (a == Direction.Left && b == Direction.Right)
                || (a == Direction.Right && b == Direction.Left);
        }
    }
}
