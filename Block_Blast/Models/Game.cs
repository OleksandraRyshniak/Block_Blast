using Block_Blast.GameModels;
using Block_Blast.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Block_Blast.Models
{
    public class Game
    {
        // ── Поля ──────────────────────────────────────────────
        private readonly BlockFactory _factory;

        // ── Свойства ──────────────────────────────────────────
        public Board Board { get; private set; }
        public Player Player { get; private set; }
        public List<Block> NextBlocks { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsOver { get; private set; }

        // ── События ───────────────────────────────────────────
        public event Action? GameStarted;
        public event Action? GameOver;
        public event Action<int>? ScoreChanged;      // передаёт новый счёт
        public event Action<int>? LinesCleared;      // передаёт кол-во линий
        public event Action? BoardUpdated;           // перерисовать поле

        // ── Конструктор ───────────────────────────────────────
        public Game(Player player)
        {
            Player = player;
            Board = new Board();
            _factory = new BlockFactory();
            NextBlocks = new List<Block>();

            // Подписываемся на событие очистки линий из Board
            Board.LinesCleared += OnLinesCleared;
        }

        // ── Управление игрой ──────────────────────────────────

        // Запускает новую игру
        public void Start()
        {
            Player.ResetScore();
            Board.Reset();
            IsOver = false;
            IsRunning = true;

            // Генерируем первые три блока
            NextBlocks = _factory.GetNext3();

            GameStarted?.Invoke();
            ScoreChanged?.Invoke(Player.Score);
        }

        // Останавливает игру (пауза или выход)
        public void Stop()
        {
            IsRunning = false;
        }

        // ── Игровая логика ────────────────────────────────────

        // Пытается разместить блок по индексу (0, 1 или 2) на позиции (row, col)
        // Возвращает true если размещение успешно
        public bool TryPlaceBlock(int blockIndex, int row, int col)
        {
            if (!IsRunning || IsOver) return false;
            if (blockIndex < 0 || blockIndex >= NextBlocks.Count) return false;

            var block = NextBlocks[blockIndex];

            if (!Board.CanPlace(block, row, col)) return false;

            // Начисляем очки за размещение
            int cellsPlaced = block.GetCells().Count;
            Player.AddPlacementScore(cellsPlaced);

            // Размещаем блок (Board сам вызовет LinesCleared если нужно)
            Board.PlaceBlock(block, row, col);

            // Убираем использованный блок
            NextBlocks.RemoveAt(blockIndex);

            // Если все три блока использованы — генерируем новые
            if (NextBlocks.Count == 0)
                NextBlocks = _factory.GetNext3();

            BoardUpdated?.Invoke();
            ScoreChanged?.Invoke(Player.Score);

            // Проверяем конец игры
            if (CheckGameOver())
            {
                IsOver = true;
                IsRunning = false;
                GameOver?.Invoke();
            }

            return true;
        }

        // Проверяет, есть ли хоть одно возможное ходы для текущих блоков
        public bool CheckGameOver()
        {
            return Board.IsGameOver(NextBlocks);
        }

        // ── Обработчики событий ───────────────────────────────
        private void OnLinesCleared(int count)
        {
            Player.AddScore(count);
            LinesCleared?.Invoke(count);
            ScoreChanged?.Invoke(Player.Score);
        }
    }

}
