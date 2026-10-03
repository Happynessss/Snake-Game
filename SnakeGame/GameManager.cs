using System;
using System.Drawing;
using System.IO;

namespace SnakeGame
{
    public class GameManager
    {
        private static readonly string HighScoreFile =
            Path.Combine(AppContext.BaseDirectory, "highscore.txt");

        private readonly Random _random = new Random();

        public GameSettings Settings { get; private set; }
        public Snake Snake { get; private set; }
        public Food Food { get; private set; }

        public int Score { get; private set; }
        public int HighScore { get; private set; }
        public GameState State { get; private set; }

        // Текущий интервал таймера: игра ускоряется с каждой съеденной едой
        public int CurrentInterval
        {
            get
            {
                int foodEaten = Score / Settings.PointsPerFood;
                int interval = Settings.TimerInterval - foodEaten * Settings.SpeedUpPerFood;
                return Math.Max(interval, Settings.MinTimerInterval);
            }
        }

        public GameManager(GameSettings settings)
        {
            Settings = settings;
            HighScore = LoadHighScore();
            Reset();
        }

        // Создаёт новую змейку и еду, сбрасывает счёт. Игра переходит в состояние Ready.
        public void Reset()
        {
            Point center = new Point(Settings.GridWidth / 2, Settings.GridHeight / 2);
            Snake = new Snake(center, Settings.InitialSnakeLength);
            Food = new Food();
            Score = 0;
            State = GameState.Ready;

            Food.Respawn(_random, Settings, Snake.Body);
        }

        public void Start()
        {
            if (State == GameState.Ready || State == GameState.Paused)
                State = GameState.Running;
        }

        public void TogglePause()
        {
            if (State == GameState.Running)
                State = GameState.Paused;
            else if (State == GameState.Paused)
                State = GameState.Running;
        }

        // Вызывается на каждый тик таймера
        public void Update()
        {
            if (State != GameState.Running)
                return;

            Snake.Move();

            CheckWallCollision();
            if (State != GameState.Running) return;

            CheckSelfCollision();
            if (State != GameState.Running) return;

            CheckFoodCollision();
        }

        public void ChangeDirection(Direction direction)
        {
            if (State == GameState.Running)
                Snake.ChangeDirection(direction);
        }

        private void CheckFoodCollision()
        {
            if (Snake.Head != Food.Position)
                return;

            Snake.Grow();
            AddScore();

            // Если свободных клеток нет — поле заполнено, это победа
            if (!Food.Respawn(_random, Settings, Snake.Body))
                Win();
        }

        private void CheckWallCollision()
        {
            Point head = Snake.Head;

            if (head.X < 0 || head.X >= Settings.GridWidth ||
                head.Y < 0 || head.Y >= Settings.GridHeight)
            {
                GameOver();
            }
        }

        private void CheckSelfCollision()
        {
            // Начинаем с индекса 1, чтобы не сравнивать голову с самой собой
            for (int i = 1; i < Snake.Body.Count; i++)
            {
                if (Snake.Body[i] == Snake.Head)
                {
                    GameOver();
                    return;
                }
            }
        }

        private void AddScore()
        {
            Score += Settings.PointsPerFood;
        }

        private void GameOver()
        {
            State = GameState.GameOver;
            UpdateHighScore();
        }

        private void Win()
        {
            State = GameState.Won;
            UpdateHighScore();
        }

        private void UpdateHighScore()
        {
            if (Score > HighScore)
            {
                HighScore = Score;
                SaveHighScore();
            }
        }

        private static int LoadHighScore()
        {
            try
            {
                if (File.Exists(HighScoreFile) &&
                    int.TryParse(File.ReadAllText(HighScoreFile), out int value))
                {
                    return value;
                }
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }

            return 0;
        }

        private void SaveHighScore()
        {
            try
            {
                File.WriteAllText(HighScoreFile, HighScore.ToString());
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }
}
