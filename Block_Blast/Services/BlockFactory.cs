using Block_Blast.GameModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Block_Blast.Services
{
    public class BlockFactory
    {
        // ── Поля ──────────────────────────────────────────────
        private readonly Random _random = new Random();

        // Цвета блоков
        private readonly List<Color> _colors = new List<Color>
        {
            Color.FromArgb("#E74C3C"), // красный
            Color.FromArgb("#3498DB"), // синий
            Color.FromArgb("#2ECC71"), // зелёный
            Color.FromArgb("#F39C12"), // оранжевый
            Color.FromArgb("#9B59B6"), // фиолетовый
            Color.FromArgb("#1ABC9C"), // бирюзовый
            Color.FromArgb("#E91E63"), // розовый
        };

        // ── Методы ────────────────────────────────────────────

        // Возвращает один случайный блок
        public Block GetRandom()
        {
            var color = _colors[_random.Next(_colors.Count)];
            return CreateRandom(color);
        }

        // Возвращает три следующих блока
        public List<Block> GetNext3()
        {
            return new List<Block>
            {
                GetRandom(),
                GetRandom(),
                GetRandom()
            };
        }

        // ── Приватные ─────────────────────────────────────────
        private Block CreateRandom(Color color)
        {
            int index = _random.Next(11);

            return index switch
            {
                0 => Block.Square(color),
                1 => Block.LineH(color),
                2 => Block.LineV(color),
                3 => Block.LShape(color),
                4 => Block.LShapeReverse(color),
                5 => Block.SShape(color),
                6 => Block.ZShape(color),
                7 => Block.TShape(color),
                8 => Block.Single(color),
                9 => Block.LineH3(color),
                _ => Block.LineV3(color)
            };
        }
    }

}
