using System;
using System.Collections.Generic;
using System.Text;

namespace Block_Blast.Models
{
    public class Player
    {
        // ── Свойства ──────────────────────────────────────────
        public string Name { get; set; }
        public int Score { get; private set; }
        public int BestScore { get; private set; }

        // ── Конструктор ───────────────────────────────────────
        public Player(string name, int bestScore = 0)
        {
            Name = name;
            Score = 0;
            BestScore = bestScore;
        }

        // ── Методы ────────────────────────────────────────────

        // Добавляет очки за очищенные линии
        // 1 линия = 100 очков, 2 = 300, 3 = 600, 4+ = 1000
        public void AddScore(int linesCleared)
        {
            int points = linesCleared switch
            {
                1 => 100,
                2 => 300,
                3 => 600,
                _ => 1000
            };

            Score += points;

            if (Score > BestScore)
                BestScore = Score;
        }

        // Добавляет очки за размещение блока (10 за каждую занятую клетку)
        public void AddPlacementScore(int cellsPlaced)
        {
            Score += cellsPlaced * 10;

            if (Score > BestScore)
                BestScore = Score;
        }

        // Сбрасывает текущий счёт (лучший результат сохраняется)
        public void ResetScore()
        {
            Score = 0;
        }
    }

}
