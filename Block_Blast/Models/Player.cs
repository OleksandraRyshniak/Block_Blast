namespace Block_Blast.Models;

public class Player
{
    // ── Свойства ──────────────────────────────────────────────
    public string Name { get; set; }
    public int Score { get; private set; }
    public int BestScore { get; private set; }

    /// <summary>Текущий комбо-счётчик (сколько ходов подряд чистились линии).</summary>
    public int ComboCount { get; private set; }

    // ── Конструктор ───────────────────────────────────────────
    public Player(string name, int bestScore = 0)
    {
        Name = name;
        Score = 0;
        BestScore = bestScore;
        ComboCount = 0;
    }

    // ── Методы ────────────────────────────────────────────────

    /// <summary>
    /// Добавляет очки за очищенные линии с учётом комбо-множителя.
    /// 1 линия = 100, 2 = 300, 3 = 600, 4+ = 1000.
    /// Комбо: каждый следующий ход с очисткой даёт +50% (макс ×3).
    /// </summary>
    public void AddLineScore(int linesCleared)
    {
        ComboCount++;

        int basePoints = linesCleared switch
        {
            1 => 100,
            2 => 300,
            3 => 600,
            _ => 1000
        };

        // Множитель: ×1 / ×1.5 / ×2 / ×2.5 / ×3 (максимум)
        double multiplier = Math.Min(1.0 + (ComboCount - 1) * 0.5, 3.0);
        int total = (int)(basePoints * multiplier);

        AddPoints(total);
    }

    /// <summary>Сбрасывает комбо (вызывать если ход прошёл без очистки линий).</summary>
    public void ResetCombo()
    {
        ComboCount = 0;
    }

    /// <summary>Очки за размещение блока: 10 × кол-во клеток.</summary>
    public void AddPlacementScore(int cellsPlaced)
    {
        AddPoints(cellsPlaced * 10);
    }

    /// <summary>Сбрасывает счёт текущей игры (рекорд сохраняется).</summary>
    public void ResetScore()
    {
        Score = 0;
        ComboCount = 0;
    }

    // ── Приватное ─────────────────────────────────────────────
    private void AddPoints(int points)
    {
        Score += points;
        if (Score > BestScore)
            BestScore = Score;
    }
}