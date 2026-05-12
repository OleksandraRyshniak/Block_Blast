using Block_Blast.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Block_Blast.Services
{
    public class ScoreService
    {
        // ── Константы ─────────────────────────────────────────
        private const string ScoresKey = "top_scores";
        private const int MaxScores = 10;

        // ── Методы ────────────────────────────────────────────

        // Сохраняет результат игрока
        public void Save(Player player)
        {
            var scores = GetTopScores();

            scores.Add(new ScoreEntry
            {
                Name = player.Name,
                Score = player.Score
            });

            // Сортируем по убыванию и оставляем топ 10
            scores = scores
                .OrderByDescending(s => s.Score)
                .Take(MaxScores)
                .ToList();

            // Сохраняем как строку: "Имя:Очки|Имя:Очки|..."
            string data = string.Join("|", scores.Select(s => $"{s.Name}:{s.Score}"));
            Preferences.Set(ScoresKey, data);
        }

        // Возвращает топ результатов
        public List<ScoreEntry> GetTopScores()
        {
            string data = Preferences.Get(ScoresKey, "");

            if (string.IsNullOrEmpty(data))
                return new List<ScoreEntry>();

            return data.Split('|')
                .Select(entry =>
                {
                    var parts = entry.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int score))
                        return new ScoreEntry { Name = parts[0], Score = score };
                    return null;
                })
                .Where(e => e != null)
                .OrderByDescending(e => e!.Score)
                .ToList()!;
        }

        // Возвращает лучший результат конкретного игрока
        public int GetBestScore(string playerName)
        {
            return GetTopScores()
                .Where(s => s.Name == playerName)
                .Select(s => s.Score)
                .DefaultIfEmpty(0)
                .Max();
        }

        // Очищает все результаты
        public void ClearAll()
        {
            Preferences.Remove(ScoresKey);
        }
    }

    // ── Вспомогательный класс записи результата ───────────────
    public class ScoreEntry
    {
        public string Name { get; set; } = "";
        public int Score { get; set; }
    }

}
