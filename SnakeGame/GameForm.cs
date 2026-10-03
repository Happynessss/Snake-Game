using System;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeGame
{
    public class GameForm : Form
    {
        private const int FieldOffsetX = 10;
        private const int FieldOffsetY = 40;

        private readonly GameSettings _settings;
        private readonly GameManager _game;
        private readonly System.Windows.Forms.Timer _timer;

        public GameForm()
        {
            _settings = new GameSettings();
            _game = new GameManager(_settings);

            // Настройка окна
            Text = "Snake";
            DoubleBuffered = true;
            KeyPreview = true; // форма получает клавиши раньше, чем контролы
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(
                _settings.FieldPixelWidth + FieldOffsetX * 2,
                _settings.FieldPixelHeight + FieldOffsetY + FieldOffsetX);

            // Игровой цикл
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = _settings.TimerInterval;
            _timer.Tick += GameLoop;
            _timer.Start();

            KeyDown += OnKeyDown;
        }

        // Один тик игры: обновить логику, подстроить скорость и перерисовать
        private void GameLoop(object sender, EventArgs e)
        {
            _game.Update();
            _timer.Interval = _game.CurrentInterval;
            Invalidate();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.W:
                    _game.ChangeDirection(Direction.Up);
                    break;
                case Keys.Down:
                case Keys.S:
                    _game.ChangeDirection(Direction.Down);
                    break;
                case Keys.Left:
                case Keys.A:
                    _game.ChangeDirection(Direction.Left);
                    break;
                case Keys.Right:
                case Keys.D:
                    _game.ChangeDirection(Direction.Right);
                    break;
                case Keys.Space:
                    if (_game.State == GameState.Ready)
                        _game.Start();
                    else
                        _game.TogglePause();
                    break;
                case Keys.Enter:
                    _game.Reset();
                    _timer.Interval = _settings.TimerInterval;
                    break;
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // Счёт, рекорд и состояние
            using (Font font = new Font("Segoe UI", 12))
            {
                g.DrawString($"Score: {_game.Score}", font, Brushes.Black, FieldOffsetX, 8);
                g.DrawString($"Best: {_game.HighScore}", font, Brushes.DarkGoldenrod, FieldOffsetX + 130, 8);
                g.DrawString(_game.State.ToString(), font, Brushes.Gray, ClientSize.Width - 110, 8);
            }

            // Фон поля
            Rectangle field = new Rectangle(
                FieldOffsetX, FieldOffsetY,
                _settings.FieldPixelWidth, _settings.FieldPixelHeight);
            g.FillRectangle(Brushes.Black, field);

            // Еда
            g.FillRectangle(Brushes.Red, CellToRect(_game.Food.Position));

            // Змейка
            for (int i = 0; i < _game.Snake.Body.Count; i++)
            {
                Brush brush = (i == 0) ? Brushes.Lime : Brushes.Green;
                g.FillRectangle(brush, CellToRect(_game.Snake.Body[i]));
            }

            // Рамка поля
            g.DrawRectangle(Pens.White, field);

            // Сообщения поверх поля
            switch (_game.State)
            {
                case GameState.Ready:
                    DrawOverlay(g, field, "SNAKE", "Space — старт\nСтрелки / WASD — управление");
                    break;
                case GameState.Paused:
                    DrawOverlay(g, field, "ПАУЗА", "Space — продолжить");
                    break;
                case GameState.GameOver:
                    DrawOverlay(g, field, "GAME OVER", $"Счёт: {_game.Score}\nEnter — заново");
                    break;
                case GameState.Won:
                    DrawOverlay(g, field, "ПОБЕДА!", $"Счёт: {_game.Score}\nEnter — заново");
                    break;
            }
        }

        // Полупрозрачная плашка с заголовком и подсказкой по центру поля
        private void DrawOverlay(Graphics g, Rectangle field, string title, string subtitle)
        {
            using (Brush shade = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
            using (Font titleFont = new Font("Segoe UI", 28, FontStyle.Bold))
            using (Font subFont = new Font("Segoe UI", 12))
            using (StringFormat center = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                g.FillRectangle(shade, field);

                Rectangle titleRect = new Rectangle(field.X, field.Y + field.Height / 2 - 70, field.Width, 60);
                Rectangle subRect = new Rectangle(field.X, field.Y + field.Height / 2 - 5, field.Width, 70);

                g.DrawString(title, titleFont, Brushes.White, titleRect, center);
                g.DrawString(subtitle, subFont, Brushes.LightGray, subRect, center);
            }
        }

        // Переводит координаты клетки в прямоугольник в пикселях
        private Rectangle CellToRect(Point cell)
        {
            int size = _settings.CellSize;
            return new Rectangle(
                FieldOffsetX + cell.X * size,
                FieldOffsetY + cell.Y * size,
                size, size);
        }
    }
}
