namespace Block_Blast.Models;

/// <summary>
/// Подсчёт очков по реальной логике Block Blast:
///
///   Размещение блока : 1 очко за каждую клетку
///   Бонус за линии  : нелинейная таблица (1→10, 2→20, 3→60, 4→120, 5→200, 6+→300)
///   Board Clear      : +360 поверх линейного бонуса
///   Комбо            : итоговый_бонус × (1 + combo × 0.5), combo растёт с каждым
///                      ходом где очищалась хотя бы одна линия, сбрасывается иначе
/// </summary>
public class Player
{
    // ── Свойства ──────────────────────────────────────────────
    public string Name { get; set; }
    public int Score { get; private set; }
    public int BestScore { get; private set; }

    /// <summary>Сколько ходов подряд чистились линии (для комбо-множителя).</summary>
    public int ComboCount { get; private set; }

    /// <summary>Самый длинный комбо за игру (для статистики в Game Over).</summary>
    public int MaxCombo { get; private set; }

    /// <summary>Сколько линий очищено за всю игру.</summary>
    public int TotalLinesCleared { get; private set; }

    // ── Конструктор ───────────────────────────────────────────
    public Player(string name, int bestScore = 0)
    {
        Name = name;
        Score = 0;
        BestScore = bestScore;
        ComboCount = 0;
        MaxCombo = 0;
        TotalLinesCleared = 0;
    }

    // ── Публичные методы ──────────────────────────────────────

    /// <summary>
    /// Очки за размещение блока: 1 очко × кол-во клеток.
    /// </summary>
    public void AddPlacementScore(int cellsPlaced)
    {
        AddPoints(cellsPlaced);
    }

    /// <summary>
    /// Очки за очистку линий с комбо-множителем.
    /// isBoardClear = true если поле полностью пустое после очистки.
    /// </summary>
    public void AddLineScore(int linesCleared, bool isBoardClear = false)
    {
        ComboCount++;
        if (ComboCount > MaxCombo) MaxCombo = ComboCount;

        TotalLinesCleared += linesCleared;

        // ── Базовый бонус за линии ────────────────────────────
        int lineBonus = linesCleared switch
        {
            1 => 10,
            2 => 20,
            3 => 60,
            4 => 120,
            5 => 200,
            _ => 300   // 6+ линий
        };

        // ── Board Clear бонус ─────────────────────────────────
        if (isBoardClear) lineBonus += 360;

        // ── Комбо-множитель: Score × (1 + combo × 0.5) ───────
        // combo начинается с 0 на первом ходу с очисткой,
        // поэтому первый ход = ×1.0, второй = ×1.5, третий = ×2.0 и т.д.
        double multiplier = 1.0 + (ComboCount - 1) * 0.5;
        int total = (int)Math.Round(lineBonus * multiplier);

        AddPoints(total);
    }

    /// <summary>
    /// Вызывается если ход прошёл БЕЗ очистки линий — сбрасывает комбо.
    /// </summary>
    public void ResetCombo()
    {
        ComboCount = 0;
    }

    /// <summary>Сбрасывает счёт текущей игры. Рекорд, MaxCombo и строки — обнуляются.</summary>
    public void ResetScore()
    {
        Score = 0;
        ComboCount = 0;
        MaxCombo = 0;
        TotalLinesCleared = 0;
    }

    // ── Приватное ─────────────────────────────────────────────
    private void AddPoints(int points)
    {
        Score += points;
        if (Score > BestScore)
            BestScore = Score;
    }
}