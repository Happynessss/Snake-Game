namespace SnakeGame
{
    public enum GameState
    {
        Ready,      // игра создана, ждём старта
        Running,    // игра идёт
        Paused,     // пауза
        GameOver,   // игра окончена (проигрыш)
        Won         // победа (поле полностью заполнено змейкой)
    }
}
