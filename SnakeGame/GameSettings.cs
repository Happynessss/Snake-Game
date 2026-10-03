namespace SnakeGame
{
    public class GameSettings
    {
        // Размер одной клетки в пикселях
        public int CellSize { get; set; } = 20;

        // Размер поля в клетках
        public int GridWidth { get; set; } = 25;
        public int GridHeight { get; set; } = 20;

        // Начальный интервал таймера в миллисекундах (чем меньше, тем быстрее игра)
        public int TimerInterval { get; set; } = 150;

        // Минимальный интервал (предел ускорения)
        public int MinTimerInterval { get; set; } = 60;

        // На сколько мс ускоряется игра за каждую съеденную еду
        public int SpeedUpPerFood { get; set; } = 3;

        // Начальная длина змейки
        public int InitialSnakeLength { get; set; } = 3;

        // Сколько очков даёт одна еда
        public int PointsPerFood { get; set; } = 10;

        // Размер поля в пикселях (вычисляется)
        public int FieldPixelWidth => GridWidth * CellSize;
        public int FieldPixelHeight => GridHeight * CellSize;
    }
}
