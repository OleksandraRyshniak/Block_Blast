# 🎮 Block Blast - ПОЛНОЕ ОПИСАНИЕ ПРОГРАММЫ (Русский)

**Версия:** .NET MAUI (.NET 10)  
**Автор:** Oleksandra Ryshniak  
**GitHub:** https://github.com/OleksandraRyshniak/Block_Blast  
**Платформы:** iOS, Android, Windows

---

## 📚 ОГЛАВЛЕНИЕ

1. [Общий обзор](#общий-обзор)
2. [Архитектура проекта](#архитектура-проекта)
3. [Модели данных (Models)](#модели-данных-models)
4. [Игровая логика (Game Models)](#игровая-логика-game-models)
5. [Сервисы (Services)](#сервисы-services)
6. [Пользовательский интерфейс (Pages)](#пользовательский-интерфейс-pages)
7. [Полный цикл игры](#полный-цикл-игры)
8. [Система очков](#система-очков)
9. [Примеры кода](#примеры-кода)

---

## 🎯 Общий обзор

### Что это?
**Block Blast** — это мобильная головоломка, похожая на **Tetris** и **Bejeweled**. Игрок размещает цветные блоки различных форм на 8×8 сетке. Когда полная строка или столбец заполняется блоками, она уничтожается, принося очки.

### Основная механика

```
┌─────────────────────────────────────────┐
│ BLOCK BLAST - Геймплей                  │
├─────────────────────────────────────────┤
│                                         │
│  Текущий счёт: 1250    Рекорд: 3840     │
│                                         │
│  ┌─ Доска 8×8 ─────────────┐            │
│  │ ██ ██ ██ ██ ██ ██ ██ ██ │ Easy Mode  │
│  │ ██ ██ ██ ██ ██ ██ ██ ██ │ или       │
│  │ ██ ██ ██ ██ ██ ██ ██ ██ │ Hard Mode │
│  │ ██ ██ ██ ██ ██ ██ ██ ██ │           │
│  │ ██ ██ ██ ██ ██ ██ ██ ██ │ Перетащи  │
│  │ ██ ██ ██ ██ ██ ██ ██ ██ │ блок сюда │
│  │ ██ ██ ██ ██ ██ ██ ██ ██ │           │
│  │ ██ ██ ██ ██ ██ ██ ██ ██ │           │
│  └────────────────────────┘             │
│                                         │
│  Следующие 3 блока:  [██] [██] [██]    │
│                                         │
└─────────────────────────────────────────┘
```

### Режимы игры

| Режим | Описание | Сложность |
|-------|---------|-----------|
| **Easy** 🟢 | Доска начинает пустой | Лёгкий |
| **Hard** 🔴 | Доска начинает с 12 заполненными клетками | Сложный |

---

## 🏗️ Архитектура проекта

```
Block_Blast/
│
├── Models/                          # 📊 Бизнес-логика и данные
│   ├── Game.cs                      # Главная логика игры
│   ├── Player.cs                    # Профиль игрока (имя, очки, комбо)
│   ├── GameMode.cs                  # enum Easy, Hard
│   ├── Theme.cs                     # Тема оформления
│   └── Language.cs                  # Язык интерфейса
│
├── GameModels/                      # 🎮 Игровые модели
│   ├── Board.cs                     # 8×8 сетка с логикой
│   ├── Cell.cs                      # Отдельная клетка
│   └── Block.cs                     # Фигуры тетромино
│
├── Services/                        # ⚙️ Сервисы (хранилище, утилиты)
│   ├── BlockFactory.cs              # Генератор случайных блоков
│   ├── ScoreService.cs              # Сохранение топ-10 рейтингов
│   ├── AccountService.cs            # Управление текущим игроком
│   ├── ThemeService.cs              # Переключение тем
│   └── LanguageService.cs           # Переключение языков
│
├── Pages/                           # 🖥️ UI страницы
│   ├── StartPage.xaml.cs            # Главное меню
│   ├── GamePage.xaml.cs             # Игровой процесс
│   ├── LoginPage.xaml.cs            # Вход/регистрация
│   └── SettingsPage.xaml.cs         # Настройки
│
├── ViewModel/                       # 🔄 ViewModel для MVVM
│   └── ViewModel.cs                 # Команды смены языка
│
├── Resources/                       # 📝 Ресурсы (локализация)
│   └── Localization/AppResources.*
│
├── Platforms/                       # 🍎 Platform-specific код
│   └── iOS/
│
├── MauiProgram.cs                   # 🚀 Конфигурация DI контейнера
└── AppShell.xaml.cs                 # 🧭 Навигация приложения
```

### Слои архитектуры

```
┌────────────────────────────────────┐
│   Presentation Layer (UI Pages)    │  ← StartPage, GamePage, SettingsPage
├────────────────────────────────────┤
│   ViewModel/Controller             │  ← GamePage logic, event handling
├────────────────────────────────────┤
│   Business Logic (Models)          │  ← Game, Player, Board, Block
├────────────────────────────────────┤
│   Services (Services/)             │  ← ScoreService, AccountService, etc.
├────────────────────────────────────┤
│   Data Storage (Preferences)       │  ← Device local storage
└────────────────────────────────────┘
```

---

## 📊 Модели данных (Models)

### 1. **Game.cs** — Главный контроллер игры

**Назначение:** Управляет игровым процессом, состоянием, правилами и событиями.

**Свойства:**

```csharp
public class Game
{
    // ── Основные компоненты ──────────────────────────
    public Board Board { get; private set; }              // Игровая доска 8×8
    public Player Player { get; private set; }           // Текущий игрок
    public List<Block> NextBlocks { get; private set; }  // 3 следующих блока

    // ── Состояние игры ─────────────────────────────
    public bool IsRunning { get; private set; }          // Игра активна?
    public bool IsOver { get; private set; }             // Игра завершена?

    // ── Приватные поля ────────────────────────────
    private readonly BlockFactory _factory;              // Создатель блоков
    private readonly GameMode _mode;                     // Easy или Hard
    private bool _lineClearedThisTurn;                   // Была ли очистка в этот ход?
}
```

**События (Events):**

```csharp
// Срабатывает когда игра начинается
public event Action? GameStarted;

// Срабатывает когда игра заканчивается
public event Action? GameOver;

// Срабатывает когда меняются очки (передаёт новое значение)
public event Action<int>? ScoreChanged;

// Срабатывает при уничтожении линий
// Параметры: (кол-во_линий, список_линий, комбо_счёт, очистилась_ли_доска)
public event Action<int, List<(bool isRow, int index)>, int, bool>? LinesCleared;

// Срабатывает при обновлении доски
public event Action? BoardUpdated;
```

**Методы:**

#### `Start()` — Начало игры
```csharp
public void Start()
{
    // 1. Сбросить очки игрока
    Player.ResetScore();

    // 2. Очистить доску
    Board.Reset();

    // 3. Установить флаги состояния
    IsOver = false;
    IsRunning = true;

    // 4. В режиме Hard — заполнить 12 случайных клеток
    if (_mode == GameMode.Hard)
        Board.FillRandom(12);

    // 5. Сгенерировать первые 3 блока
    NextBlocks = _factory.GetNext3();

    // 6. Отправить событие о начале
    GameStarted?.Invoke();
    ScoreChanged?.Invoke(Player.Score);
}
```

**Как это работает:**
1. Когда игрок нажимает "Play", создаётся объект `Game` с режимом (Easy/Hard)
2. Вызывается метод `Start()`
3. Если Hard режим — доска заполняется 12 случайными блоками разных цветов (серого)
4. Генерируются 3 случайных блока для показа игроку
5. На экран выводится обновленная информация

#### `TryPlaceBlock()` — Попытка разместить блок
```csharp
public bool TryPlaceBlock(int blockIndex, int row, int col)
{
    // 1. Проверка: игра идёт?
    if (!IsRunning || IsOver) 
        return false;

    // 2. Проверка: валидный индекс блока?
    if (blockIndex < 0 || blockIndex >= NextBlocks.Count) 
        return false;

    // 3. Получить сам блок
    var block = NextBlocks[blockIndex];

    // 4. Проверка: можно ли разместить блок в этой позиции?
    if (!Board.CanPlace(block, row, col)) 
        return false;

    // ─────────────────────────────────────────────────
    // Если всё прошло — размещаем блок!
    // ─────────────────────────────────────────────────

    // 5. Добавить очки за размещение (по количеству клеток блока)
    int cellsPlaced = block.GetCells().Count;
    Player.AddPlacementScore(cellsPlaced);

    // 6. Сбросить флаг очистки линий
    _lineClearedThisTurn = false;

    // 7. Разместить блок на доске (и проверить линии)
    Board.PlaceBlock(block, row, col);

    // 8. Если линий не было очищено — сбросить комбо счёт
    if (!_lineClearedThisTurn)
        Player.ResetCombo();

    // 9. Удалить использованный блок из списка
    NextBlocks.RemoveAt(blockIndex);

    // 10. Если блоков меньше 3 — сгенерировать новые
    if (NextBlocks.Count == 0)
        NextBlocks = _factory.GetNext3();

    // 11. Отправить события об обновлении
    BoardUpdated?.Invoke();
    ScoreChanged?.Invoke(Player.Score);

    // 12. Проверить: может ли ещё что-то быть размещено?
    if (Board.IsGameOver(NextBlocks))
    {
        IsOver = true;
        IsRunning = false;
        GameOver?.Invoke();  // Игра закончена!
    }

    return true;  // Размещение было успешным
}
```

**Пошаговый пример:**
```
Шаг 1: Игрок видит 3 блока: [██] [███] [█████]
Шаг 2: Перетаскивает первый блок (██) на позицию (2, 3)
Шаг 3: TryPlaceBlock(0, 2, 3) проверяет:
       - Индекс 0 в диапазоне? ✓
       - Игра идёт? ✓
       - Можно разместить на (2,3)? ✓
Шаг 4: Board.PlaceBlock() размещает блок
Шаг 5: Board.ClearFullLines() проверяет линии → найдена 1 полная строка!
Шаг 6: Строка уничтожена, срабатывает LinesCleared event
Шаг 7: Player.AddLineScore() пересчитывает очки с комбо
Шаг 8: Первый блок удаляется из NextBlocks
Шаг 9: Генерируются новые 3 блока
Шаг 10: BoardUpdated event → UI обновляется
Шаг 11: Проверка GameOver → может ли что-то быть размещено? → Если нет → Game Over!
```

### 2. **Player.cs** — Профиль игрока

**Назначение:** Хранит данные игрока (имя, очки, рекорд, комбо, статистику).

```csharp
public class Player
{
    // ── Основные данные ──────────────────────────
    public string Name { get; set; }                // Имя игрока
    public int Score { get; private set; }          // Текущие очки за игру
    public int BestScore { get; private set; }      // Рекордные очки

    // ── Комбо-система ───────────────────────────
    public int ComboCount { get; private set; }     // Текущее комбо
    public int MaxCombo { get; private set; }       // Макс. комбо за игру

    // ── Статистика ────────────────────────────
    public int TotalLinesCleared { get; private set; }  // Всего уничтожено линий
}
```

**Методы:**

#### `AddPlacementScore()` — Добавить очки за размещение
```csharp
public void AddPlacementScore(int cellsPlaced)
{
    // Просто добавляем по 1 очку за каждую клетку
    // Блок 1×1 (Single) → 1 очко
    // Блок 2×2 (Square) → 4 очка
    // Блок 4×1 (LineH) → 4 очка
    AddPoints(cellsPlaced);
}
```

#### `AddLineScore()` — Добавить очки за очистку линий
```csharp
public void AddLineScore(int linesCleared, bool isBoardClear = false)
{
    // 1. Увеличить счётчик комбо
    ComboCount++;
    if (ComboCount > MaxCombo) 
        MaxCombo = ComboCount;

    // 2. Добавить к общему счётчику линий
    TotalLinesCleared += linesCleared;

    // 3. Базовый бонус в зависимости от количества
    int lineBonus = linesCleared switch
    {
        1 => 10,        // 1 линия = 10 очков
        2 => 20,        // 2 линии = 20 очков
        3 => 60,        // 3 линии = 60 очков (больше за больше!)
        4 => 120,       // 4 линии = 120 очков
        5 => 200,       // 5 линий = 200 очков
        _ => 300        // 6+ линий = 300 очков (макс)
    };

    // 4. Если всю доску очистили — добавить 360 бонус
    if (isBoardClear) 
        lineBonus += 360;

    // 5. КОМБО МУЛЬТИПЛИКАТОР
    // ComboCount = 1 → множитель = 1.0x (первая очистка)
    // ComboCount = 2 → множитель = 1.5x
    // ComboCount = 3 → множитель = 2.0x
    // ComboCount = 4 → множитель = 2.5x
    // И так далее: каждое комбо добавляет ×0.5
    double multiplier = 1.0 + (ComboCount - 1) * 0.5;
    int total = (int)Math.Round(lineBonus * multiplier);

    AddPoints(total);

    // ПРИМЕР:
    // Первое уничтожение: 1 линия → 10 × 1.0 = 10 очков
    // Второе подряд: 1 линия → 10 × 1.5 = 15 очков
    // Третье подряд: 1 линия → 10 × 2.0 = 20 очков
    // Четвёртое подряд: 3 линии → 60 × 2.5 = 150 очков! 🔥
}
```

**Пример подсчёта очков:**

```
Ход 1: Разместить блок 4 клетки → +4 очка (Всего: 4)

Ход 2: Уничтожена 1 строка
       BonusBase = 10
       ComboCount = 1, множитель = 1.0
       Добавлено: 10 × 1.0 = 10 очков (Всего: 14)

Ход 3: Разместить блок 1 клетка → +1 очко (Всего: 15)

Ход 4: Уничтожены 2 строки
       BonusBase = 20
       ComboCount = 2, множитель = 1.5
       Добавлено: 20 × 1.5 = 30 очков (Всего: 45)

Ход 5: Уничтожены 3 строки + вся доска очищена
       BonusBase = 60 + 360 = 420
       ComboCount = 3, множитель = 2.0
       Добавлено: 420 × 2.0 = 840 очков 💥💥💥 (Всего: 885)
```

### 3. **GameMode.cs** — Режимы игры

```csharp
public enum GameMode
{
    Easy,   // Пустая доска
    Hard    // Доска с 12 заполненными клетками
}
```

---

## 🎮 Игровая логика (GameModels)

### 1. **Block.cs** — Фигуры (Tetromino)

**Назначение:** Определяет форму и цвет блока.

```csharp
public class Block
{
    // ── Данные ────────────────────────────────────
    public int[,] Shape { get; private set; }   // Форма: 0=пусто, 1=клетка
    public Color BlockColor { get; private set; }  // Цвет блока
    public int Rows => Shape.GetLength(0);         // Высота фигуры
    public int Cols => Shape.GetLength(1);         // Ширина фигуры

    public Block(int[,] shape, Color color)
    {
        Shape = shape;
        BlockColor = color;
    }

    // ── Методы ────────────────────────────────────

    /// Возвращает список координат (row, col) занятых клеток в фигуре
    public List<(int row, int col)> GetCells()
    {
        var cells = new List<(int, int)>();
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                if (Shape[r, c] == 1)  // Если клетка занята (= 1)
                    cells.Add((r, c));
        return cells;
    }
}
```

**Пример: как работает GetCells()**

```
Блок "Square" (2×2 квадрат):
Shape = [[ 1, 1 ],
         [ 1, 1 ]]

GetCells() вернёт:
[ (0,0), (0,1), (1,0), (1,1) ]

Это означает: 4 клетки на позициях (0,0), (0,1), (1,0), (1,1)
Относительно левого верхнего угла блока.
```

**Все доступные формы:**

```csharp
// 1. SQUARE — 2×2 Квадрат (4 клетки)
█ █
█ █
Block.Square(color)

// 2. LINE_H — 1×4 Горизонтальная линия
█ █ █ █
Block.LineH(color)

// 3. LINE_H3 — 1×3 Короткая горизонтальная линия
█ █ █
Block.LineH3(color)

// 4. LINE_V — 4×1 Вертикальная линия
█
█
█
█
Block.LineV(color)

// 5. LINE_V3 — 3×1 Короткая вертикальная линия
█
█
█
Block.LineV3(color)

// 6. LINE_V5 — 5×1 Длинная вертикальная линия
█
█
█
█
█
Block.LineV5(color)

// 7. L_SHAPE — L-образная фигура
█
█
█ █
Block.LShape(color)

// 8. L_SHAPE_REVERSE — Обратная L
  █
  █
█ █
Block.LShapeReverse(color)

// 9. S_SHAPE — S-образная фигура
█ █
  █ █
Block.SShape(color)

// 10. Z_SHAPE — Z-образная фигура
  █ █
█ █
Block.ZShape(color)

// 11. T_SHAPE — T-образная фигура
█ █ █
  █
Block.TShape(color)

// 12. SINGLE — 1 клетка
█
Block.Single(color)

// 13. SQUARE3 — 3×3 Большой квадрат (9 клеток)
█ █ █
█ █ █
█ █ █
Block.Square3(color)
```

### 2. **Cell.cs** — Отдельная клетка доски

```csharp
public class Cell
{
    // ── Координаты ────────────────────────────────
    public int Row { get; private set; }           // Строка (0-7)
    public int Col { get; private set; }           // Столбец (0-7)

    // ── Состояние ─────────────────────────────────
    public bool IsOccupied { get; private set; }   // Занята ли клетка?
    public Color CellColor { get; private set; }   // Цвет (если занята)

    public Cell(int row, int col)
    {
        Row = row;
        Col = col;
        IsOccupied = false;
        CellColor = Colors.Transparent;  // Прозрачная в начале
    }

    /// Заполнить клетку цветом (блок размещен)
    public void Fill(Color color)
    {
        IsOccupied = true;
        CellColor = color;
    }

    /// Очистить клетку (когда строка/столбец уничтожены)
    public void Clear()
    {
        IsOccupied = false;
        CellColor = Colors.Transparent;
    }
}
```

### 3. **Board.cs** — Игровая доска 8×8

**Назначение:** Управляет сеткой, проверяет ходы, уничтожает линии.

```csharp
public class Board
{
    // ── Константы ─────────────────────────────────
    public const int Rows = 8;  // Высота доски
    public const int Cols = 8;  // Ширина доски

    // ── Основной компонент ─────────────────────────
    public Cell[,] Grid { get; private set; }  // 8×8 массив клеток

    // ── События ────────────────────────────────
    public event Action<int, List<(bool isRow, int index)>, bool>? LinesCleared;

    // Инициализация
    public Board()
    {
        Grid = new Cell[Rows, Cols];
        InitGrid();
    }

    private void InitGrid()
    {
        // Создаём все 64 клетки (8 × 8)
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                Grid[r, c] = new Cell(r, c);
    }
}
```

**Ключевые методы Board:**

#### `CanPlace()` — Проверить, можно ли разместить блок

```csharp
public bool CanPlace(Block block, int startRow, int startCol)
{
    // Для каждой клетки блока
    foreach (var (dr, dc) in block.GetCells())
    {
        // Рассчитать абсолютную позицию на доске
        int r = startRow + dr;
        int c = startCol + dc;

        // Проверка 1: не выходит ли за границы?
        if (r < 0 || r >= Rows || c < 0 || c >= Cols) 
            return false;  // ❌ Выходит за границы

        // Проверка 2: не пересекается ли с уже размещённым блоком?
        if (Grid[r, c].IsOccupied) 
            return false;  // ❌ Пересечение
    }

    return true;  // ✓ Можно разместить!
}
```

**Пример:**
```
Доска:
█ █ . . . █ █ █
█ █ . . . █ █ █
. . . . . . . .
. . . . . . . .
. . . . . . . .
. . . . . . . .
. . . . . . . .
. . . . . . . .

Попытка разместить Square на (0, 2):
- (0,2): IsOccupied = false ✓
- (0,3): IsOccupied = false ✓
- (1,2): IsOccupied = false ✓
- (1,3): IsOccupied = false ✓
Результат: CAN PLACE ✓

Попытка разместить Square на (0, 4):
- (0,4): IsOccupied = false ✓
- (0,5): IsOccupied = true ❌
Результат: CANNOT PLACE (пересечение)
```

#### `PlaceBlock()` — Разместить блок на доске

```csharp
public void PlaceBlock(Block block, int startRow, int startCol)
{
    // 1. Разместить каждую клетку блока
    foreach (var (dr, dc) in block.GetCells())
    {
        int r = startRow + dr;
        int c = startCol + dc;
        Grid[r, c].Fill(block.BlockColor);  // Заполнить клетку цветом блока
    }

    // 2. Проверить и уничтожить полные линии
    var (count, lines) = ClearFullLines();

    // 3. Если что-то было уничтожено — отправить событие
    if (count > 0)
    {
        bool boardClear = IsBoardEmpty();
        LinesCleared?.Invoke(count, lines, boardClear);
    }
}
```

#### `ClearFullLines()` — Найти и уничтожить полные строки/столбцы

```csharp
private (int count, List<(bool isRow, int index)> lines) ClearFullLines()
{
    int count = 0;
    var lines = new List<(bool isRow, int index)>();

    // 1. Проверить все строки
    for (int r = 0; r < Rows; r++)
        if (IsRowFull(r))  // Если строка полная
        {
            lines.Add((true, r));  // Пометить строку
            count++;
        }

    // 2. Проверить все столбцы
    for (int c = 0; c < Cols; c++)
        if (IsColFull(c))  // Если столбец полный
        {
            lines.Add((false, c));  // Пометить столбец
            count++;
        }

    // 3. Уничтожить найденные строки/столбцы
    foreach (var (isRow, idx) in lines)
    {
        if (isRow)
            ClearRow(idx);   // Очистить строку
        else
            ClearCol(idx);   // Очистить столбец
    }

    return (count, lines);
}

private bool IsRowFull(int row)
{
    // Проверить все 8 клеток в строке
    for (int c = 0; c < Cols; c++)
        if (!Grid[row, c].IsOccupied)  // Если есть пустая
            return false;  // Строка не полная
    return true;  // Все 8 клеток заполнены
}

private bool IsColFull(int col)
{
    // Проверить все 8 клеток в столбце
    for (int r = 0; r < Rows; r++)
        if (!Grid[r, col].IsOccupied)  // Если есть пустая
            return false;  // Столбец не полный
    return true;  // Все 8 клеток заполнены
}

private void ClearRow(int row)
{
    // Очистить все 8 клеток в строке
    for (int c = 0; c < Cols; c++)
        Grid[row, c].Clear();
}

private void ClearCol(int col)
{
    // Очистить все 8 клеток в столбце
    for (int r = 0; r < Rows; r++)
        Grid[r, col].Clear();
}
```

**Пример очистки:**
```
БЫЛО:
█ █ █ █ █ █ █ █  ← Полная строка!
█ . . . . . . █
. . . . . . . .

ПОСЛЕ (ClearRow):
. . . . . . . .  ← Очищена
█ . . . . . . █
. . . . . . . .

То же самое для столбцов - если вся колонна заполнена, она очищается.
```

#### `IsGameOver()` — Проверить конец игры

```csharp
public bool IsGameOver(List<Block> nextBlocks)
{
    // Для каждого из 3 следующих блоков
    foreach (var block in nextBlocks)
    {
        // Можно ли разместить этот блок ГДЕУГОДНО на доске?
        if (CanPlaceAnywhere(block)) 
            return false;  // Есть место! Игра продолжается
    }

    return true;  // Ни один блок не может быть размещён → GAME OVER
}

public bool CanPlaceAnywhere(Block block)
{
    // Попытаться разместить блок на каждой позиции доски
    for (int r = 0; r < Rows; r++)
        for (int c = 0; c < Cols; c++)
            if (CanPlace(block, r, c))  // Можно разместить?
                return true;  // Найдено место!

    return false;  // Нет места нигде
}
```

#### `GetPreviewCells()` — Показать preview блока

```csharp
public List<(int row, int col)> GetPreviewCells(Block block, int startRow, int startCol)
{
    var cells = new List<(int, int)>();

    // Для каждой клетки блока
    foreach (var (dr, dc) in block.GetCells())
    {
        int r = startRow + dr;
        int c = startCol + dc;

        // Проверить границы и пересечения
        if (r < 0 || r >= Rows || c < 0 || c >= Cols) 
            return new List<(int, int)>();  // Выходит за границы → пусто
        if (Grid[r, c].IsOccupied) 
            return new List<(int, int)>();  // Пересечение → пусто

        cells.Add((r, c));
    }

    return cells;
}
```

Это используется в UI для показа, где будет блок при перемещении мыши.

#### `FillRandom()` — Заполнить доску для режима Hard

```csharp
public void FillRandom(int count = 12)
{
    var rng = new Random();
    var positions = new List<(int r, int c)>();

    // 1. Создать список всех 64 позиций
    for (int r = 0; r < Rows; r++)
        for (int c = 0; c < Cols; c++)
            positions.Add((r, c));

    // 2. Перемешать позиции (Fisher-Yates shuffle)
    for (int i = positions.Count - 1; i > 0; i--)
    {
        int j = rng.Next(i + 1);
        (positions[i], positions[j]) = (positions[j], positions[i]);  // Поменять местами
    }

    // 3. Заполнить первые 12 позиций серым цветом
    var hardColor = Color.FromArgb("#607D8B");  // Серо-голубой
    int placed = 0;
    foreach (var (r, c) in positions)
    {
        if (placed >= count) break;
        Grid[r, c].Fill(hardColor);  // Заполнить
        placed++;
    }
}
```

---

## ⚙️ Сервисы (Services)

### 1. **BlockFactory.cs** — Генератор блоков

```csharp
public class BlockFactory
{
    private readonly Random _random = new Random();

    // Доступные цвета
    private readonly List<Color> _colors = new List<Color>
    {
        Color.FromArgb("#E74C3C"),  // Красный
        Color.FromArgb("#3498DB"),  // Синий
        Color.FromArgb("#2ECC71"),  // Зелёный
        Color.FromArgb("#F39C12"),  // Оранжевый
        Color.FromArgb("#9B59B6"),  // Фиолетовый
        Color.FromArgb("#1ABC9C"),  // Бирюзовый
        Color.FromArgb("#E91E63"),  // Розовый
    };

    /// Вернуть один случайный блок
    public Block GetRandom()
    {
        var color = _colors[_random.Next(_colors.Count)];  // Случайный цвет
        return CreateRandom(color);  // Случайная форма
    }

    /// Вернуть 3 следующих случайных блока
    public List<Block> GetNext3()
    {
        return new List<Block>
        {
            GetRandom(),
            GetRandom(),
            GetRandom()
        };
    }

    private Block CreateRandom(Color color)
    {
        int index = _random.Next(11);  // Случайно выбрать 0-11

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
            10 => Block.Square3(color),
            11 => Block.LineV5(color),
            _ => Block.LineV3(color)
        };
    }
}
```

### 2. **ScoreService.cs** — Сохранение рейтингов

```csharp
public class ScoreService
{
    private const string ScoresKey = "top_scores";
    private const int MaxScores = 10;

    /// Сохранить результат в топ-10
    public void Save(Player player)
    {
        var scores = GetTopScores();

        // Добавить новый результат
        scores.Add(new ScoreEntry
        {
            Name = player.Name,
            Score = player.Score
        });

        // Отсортировать по убыванию очков и оставить топ-10
        scores = scores
            .OrderByDescending(s => s.Score)
            .Take(MaxScores)
            .ToList();

        // Сохранить в локальное хранилище устройства
        // Формат: "Имя1:Очки1|Имя2:Очки2|Имя3:Очки3|..."
        string data = string.Join("|", scores.Select(s => $"{s.Name}:{s.Score}"));
        Preferences.Set(ScoresKey, data);
    }

    /// Получить все сохранённые результаты
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

    /// Получить лучший результат конкретного игрока
    public int GetBestScore(string playerName)
    {
        return GetTopScores()
            .Where(s => s.Name == playerName)
            .Select(s => s.Score)
            .DefaultIfEmpty(0)
            .Max();
    }
}

public class ScoreEntry
{
    public string Name { get; set; } = "";
    public int Score { get; set; }
}
```

**Пример использования:**
```csharp
var scoreService = new ScoreService();

// После окончания игры сохранить
scoreService.Save(player);  // Если в топ-10, сохранится

// Позже получить топ-10
var topScores = scoreService.GetTopScores();
foreach (var entry in topScores)
    Console.WriteLine($"{entry.Name}: {entry.Score}");

// Получить лучший результат игрока
int best = scoreService.GetBestScore("Alice");
```

### 3. **AccountService.cs** — Управление аккаунтом

```csharp
public class AccountService
{
    private const string CurrentPlayerKey = "current_player_name";

    /// Получить имя текущего залогиненного игрока (или null)
    public string? GetCurrentName()
    {
        var name = Preferences.Get(CurrentPlayerKey, "");
        return string.IsNullOrWhiteSpace(name) ? null : name;
    }

    /// Сохранить имя как «текущий активный игрок»
    public void SetCurrentName(string name)
    {
        Preferences.Set(CurrentPlayerKey, name.Trim());
    }

    /// Выход из аккаунта
    public void Logout()
    {
        Preferences.Remove(CurrentPlayerKey);
    }

    /// Проверка: есть ли залогиненный игрок?
    public bool IsLoggedIn => GetCurrentName() != null;
}
```

**Пример использования:**
```csharp
var accountService = new AccountService();

// Когда игрок вводит имя на LoginPage
accountService.SetCurrentName("Alice");

// При запуске приложения проверить
if (accountService.IsLoggedIn)
{
    string name = accountService.GetCurrentName();
    // Перейти на StartPage
}
else
{
    // Перейти на LoginPage
}

// При нажатии "Logout" в SettingsPage
accountService.Logout();
```

### 4. **ThemeService.cs** и **LanguageService.cs**

Эти сервисы управляют темой оформления (светлая/тёмная) и языком интерфейса (English/Русский/Eesti).

---

## 🖥️ Пользовательский интерфейс (Pages)

### 1. **StartPage.xaml.cs** — Главное меню

```csharp
public partial class StartPage : ContentPage
{
    private readonly ThemeService _themeService;
    private readonly ScoreService _scoreService;
    private readonly AccountService _accountService;

    private Label LblTitle;
    private Label LblBest;
    private Button BtnPlayEasy;
    private Button BtnPlayHard;
    private Button BtnSettings;

    public StartPage(ThemeService themeService, ScoreService scoreService, AccountService accountService)
    {
        _themeService = themeService;
        _scoreService = scoreService;
        _accountService = accountService;

        BuildUI();
        PrepareForEntrance();
    }

    private void BuildUI()
    {
        // Создание UI элементов программно (без XAML)

        // 1. Название игры
        LblTitle = new Label
        {
            Text = "BLOCK\nBLAST",
            FontFamily = "PressStart2P",
            FontSize = 30,
            TextColor = Color.FromArgb("#00F5FF"),  // Cyan
        };

        // 2. Лучший результат
        LblBest = new Label
        {
            Text = "Best: 0",
            FontFamily = "PressStart2P",
            FontSize = 16,
            TextColor = Colors.Gold,
        };

        // 3. Кнопки
        BtnPlayEasy = new Button
        {
            Text = "PLAY EASY",
            Clicked = async (s, e) => await PlayGame(GameMode.Easy),
        };

        BtnPlayHard = new Button
        {
            Text = "PLAY HARD",
            Clicked = async (s, e) => await PlayGame(GameMode.Hard),
        };

        BtnSettings = new Button
        {
            Text = "⚙️ SETTINGS",
            Clicked = async (s, e) => await Shell.Current.GoToAsync("settings"),
        };
    }

    private async Task PlayGame(GameMode mode)
    {
        // Переход на GamePage с выбранным режимом
        await Shell.Current.GoToAsync($"game?mode={(int)mode}");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // При загрузке StartPage проверить, залогинен ли игрок
        if (!_accountService.IsLoggedIn)
        {
            // Если нет → перейти на LoginPage
            Shell.Current.GoToAsync("login");
        }

        // Получить лучший результат и показать
        var topScores = _scoreService.GetTopScores();
        if (topScores.Count > 0)
        {
            LblBest.Text = $"Best: {topScores[0].Score}";
        }
    }
}
```

### 2. **GamePage.xaml.cs** — Игровой процесс

Это самая сложная страница. Она содержит:

- **Сетку 8×8** для отображения доски
- **BoxView** для каждой клетки (32 × 36 пикселей)
- **3 контейнера** для показа следующих блоков
- **Drag & Drop логику** для перемещения блоков
- **Preview** для показа, где будет размещён блок
- **Подсказки (Hint)**
- **Обработка событий** от Game модели

**Ключевые компоненты:**

```csharp
public partial class GamePage : ContentPage
{
    private const int CellSize = 36;               // 36×36 пиксели за клетку
    private const int BlockAreaSize = 100;         // Область для preview блока

    private readonly ThemeService _themeService;
    private readonly ScoreService _scoreService;
    private readonly Game _game;

    // ── UI элементы ────────────────────
    private Label LblScore;                        // "Score: 1250"
    private Label LblBest;                         // "Best: 3840"
    private Grid GameGrid;                         // 8×8 сетка
    private readonly ContentView[] BlockContainers = new ContentView[3];  // 3 блока
    private BoxView[,] _cellViews;                 // 64 BoxView для клеток

    // ── Drag & Drop ───────────────────
    private int _draggingIndex = -1;               // Какой блок перетаскивается?
    private bool _isDragging;                      // Идёт ли перетаскивание?

    // ── Preview ────────────────────────
    private List<(int row, int col)> _previewCells;  // Клетки preview
    private List<(int row, int col)> _lineClearPreviewCells;  // Клетки очистки

    // ── Hint ────────────────────────
    private List<(int row, int col)> _hintCells;
    private int _hintIndex = -1;

    public GamePage(Player player, GameMode mode, ThemeService themeService, ScoreService scoreService)
    {
        _themeService = themeService;
        _scoreService = scoreService;
        _game = new Game(player, mode);

        BuildUI();

        // Подписаться на события Game
        _game.ScoreChanged += OnScoreChanged;
        _game.BoardUpdated += OnBoardUpdated;
        _game.LinesCleared += OnLinesCleared;
        _game.GameOver += OnGameOver;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _game.Start();  // Начать игру!
        RefreshBoardUI();
    }

    // ────────────────────────────────────────────────────────
    // Event handlers
    // ────────────────────────────────────────────────────────

    private void OnScoreChanged(int newScore)
    {
        // Обновить на экране текущие очки
        LblScore.Text = newScore.ToString();
    }

    private void OnBoardUpdated()
    {
        // Обновить визуальное представление доски
        RefreshBoardUI();
        RefreshNextBlocksUI();
    }

    private void OnLinesCleared(int count, List<(bool isRow, int index)> lines, int combo, bool isBoardClear)
    {
        // Показать анимацию уничтожения линий
        HighlightClearedLines(lines);

        // Через 300ms освежить UI
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(300);
            RefreshBoardUI();
        });
    }

    private void OnGameOver()
    {
        // Сохранить результат
        _scoreService.Save(_game.Player);

        // Показать диалог "Game Over"
        ShowGameOverDialog();
    }

    // ────────────────────────────────────────────────────────
    // Drag & Drop
    // ────────────────────────────────────────────────────────

    private void OnBlockPointerPressed(int blockIndex, PointerEventArgs e)
    {
        _draggingIndex = blockIndex;
        _isDragging = true;

        // Создать "привидение" (ghost) блока
        _ghostView = new Border { BackgroundColor = Colors.Gray.WithAlpha(0.5f) };
    }

    private void OnPointerMoved(PointerEventArgs e)
    {
        if (!_isDragging) return;

        // Рассчитать позицию мыши
        var currentPos = e.GetPosition(GameGrid);

        // Конвертировать пиксели в координаты доски
        int row = (int)(currentPos.Value.Y / CellSize);
        int col = (int)(currentPos.Value.X / CellSize);

        // Получить preview клетки
        var block = _game.NextBlocks[_draggingIndex];
        _previewCells = _game.Board.GetPreviewCells(block, row, col);

        // Также получить, какие линии будут уничтожены
        _lineClearPreviewCells = _game.Board.GetWouldClearLines(block, row, col);

        // Обновить UI preview
        RefreshPreview();
    }

    private void OnPointerReleased(PointerEventArgs e)
    {
        if (!_isDragging || _draggingIndex == -1) return;

        _isDragging = false;

        // Рассчитать координаты
        var currentPos = e.GetPosition(GameGrid);
        int row = (int)(currentPos.Value.Y / CellSize);
        int col = (int)(currentPos.Value.X / CellSize);

        // Попытаться разместить блок
        if (_game.TryPlaceBlock(_draggingIndex, row, col))
        {
            // Успешно! onBoardUpdated сработает сам
        }
        else
        {
            // Не удалось → очистить preview
            ClearPreview();
        }

        _draggingIndex = -1;
    }

    // ────────────────────────────────────────────────────────
    // UI Refresh
    // ────────────────────────────────────────────────────────

    private void RefreshBoardUI()
    {
        // Обновить все 64 клетки
        for (int r = 0; r < Board.Rows; r++)
        {
            for (int c = 0; c < Board.Cols; c++)
            {
                var cell = _game.Board.Grid[r, c];
                var boxView = _cellViews[r, c];

                if (cell.IsOccupied)
                    boxView.BackgroundColor = cell.CellColor;
                else
                    boxView.BackgroundColor = Colors.Transparent;
            }
        }
    }

    private void RefreshNextBlocksUI()
    {
        // Для каждого из 3 следующих блоков
        for (int i = 0; i < 3; i++)
        {
            if (i < _game.NextBlocks.Count)
            {
                var block = _game.NextBlocks[i];
                DrawBlockInContainer(block, BlockContainers[i]);
            }
        }
    }

    private void RefreshPreview()
    {
        // Очистить предыдущий preview
        for (int r = 0; r < Board.Rows; r++)
            for (int c = 0; c < Board.Cols; c++)
                _cellViews[r, c].Opacity = 1.0;  // Полная прозрачность

        // Показать preview блока (полупрозрачный)
        foreach (var (r, c) in _previewCells)
            _cellViews[r, c].Opacity = 0.5;

        // Показать, какие линии будут очищены (ярко-жёлтый)
        foreach (var (r, c) in _lineClearPreviewCells)
            _cellViews[r, c].BackgroundColor = Color.FromArgb("#FFFFAA");
    }

    private void ClearPreview()
    {
        _previewCells.Clear();
        _lineClearPreviewCells.Clear();
        RefreshBoardUI();
    }
}
```

---

## 🔄 Полный цикл игры

### Последовательность действий

```
┌──────────────────────────────────────────────────────────────┐
│ ПОЛНЫЙ ЦИКЛ ИГРЫ Block Blast                                │
└──────────────────────────────────────────────────────────────┘

1️⃣  ЗАПУСК ПРИЛОЖЕНИЯ
    ├─ AppShell инициализирует DI контейнер (MauiProgram)
    ├─ Сервисы: AccountService, ThemeService, ScoreService
    └─ Переход на StartPage

2️⃣  StartPage - ГЛАВНОЕ МЕНЮ
    ├─ Проверка: IsLoggedIn?
    │  ├─ YES → показать меню
    │  └─ NO → переход на LoginPage
    │
    ├─ Показать:
    │  ├─ Title: "BLOCK BLAST"
    │  ├─ Best Score (из ScoreService)
    │  ├─ Button "PLAY EASY"
    │  ├─ Button "PLAY HARD"
    │  └─ Button "SETTINGS"
    │
    └─ Если нажата "PLAY EASY/HARD" → переход на GamePage

3️⃣  GamePage - ИНИЦИАЛИЗАЦИЯ ИГРЫ
    ├─ Создан объект Game(player, GameMode)
    ├─ Построен UI:
    │  ├─ 8×8 сетка (64 BoxView)
    │  ├─ Счётчик очков
    │  └─ 3 блока preview
    │
    ├─ Game.Start() вызывается
    │  ├─ Player.ResetScore()
    │  ├─ Board.Reset()
    │  ├─ Если Hard mode → Board.FillRandom(12)
    │  ├─ Генерируются 3 блока: NextBlocks = GetNext3()
    │  └─ GameStarted event → UI обновляется
    │
    └─ Подписка на события: ScoreChanged, BoardUpdated, LinesCleared, GameOver

4️⃣  ИГРОВОЙ ЦИКЛ (повторяется)

    ┌──────────────────────────────────────────┐
    │  ОЖИДАНИЕ ХОДА ИГРОКА                   │
    │                                          │
    │  Игрок видит:                           │
    │  • Доску 8×8 с размещёнными блоками   │
    │  • 3 следующих блока                   │
    │  • Текущие очки                         │
    └──────────────────────────────────────────┘
              ↓ (игрок перетаскивает блок)

    ┌──────────────────────────────────────────┐
    │  DRAG & DROP ОБРАБОТКА                  │
    │                                          │
    │  OnPointerPressed(blockIndex)           │
    │  ├─ Запомнить: _draggingIndex = blockIndex
    │  ├─ Начать отслеживание мыши
    │  └─ Создать "привидение"
    │
    │  OnPointerMoved(e)                      │
    │  ├─ Рассчитать текущую позицию (row, col)
    │  ├─ block = NextBlocks[_draggingIndex]
    │  ├─ previewCells = Board.GetPreviewCells(block, row, col)
    │  ├─ linesToClear = Board.GetWouldClearLines(...)
    │  └─ Обновить UI preview (показать полупрозрачно + жёлтый для линий)
    │
    │  OnPointerReleased(e)                   │
    │  ├─ Получить финальные координаты
    │  ├─ Вызвать: Game.TryPlaceBlock(_draggingIndex, row, col)
    │  └─ Если успешно → продолжить, если нет → отменить
    └──────────────────────────────────────────┘
              ↓

    ┌──────────────────────────────────────────┐
    │  TryPlaceBlock() - РАЗМЕЩЕНИЕ            │
    │                                          │
    │  1. Проверки (валидность):               │
    │     ├─ IsRunning && !IsOver? ✓           │
    │     ├─ blockIndex в диапазоне? ✓        │
    │     └─ Board.CanPlace()? ✓              │
    │                                          │
    │  2. Добавить очки за размещение:        │
    │     cellCount = block.GetCells().Count   │
    │     Player.AddPlacementScore(cellCount)  │
    │                                          │
    │  3. Размещение на доске:                 │
    │     Board.PlaceBlock(block, row, col)    │
    │     └─ Заполнить каждую клетку блока    │
    │     └─ Board.ClearFullLines()            │
    │        ├─ Найти полные строки/столбцы   │
    │        └─ LinesCleared event!            │
    │                                          │
    │  4. Обработка линий (если были):         │
    │     OnLinesCleared()                     │
    │     ├─ Player.AddLineScore(count, isBoardClear)
    │     │  ├─ ComboCount++
    │     │  ├─ Рассчитать бонус (10-300 +360)
    │     │  ├─ Применить комбо-мультипликатор
    │     │  └─ Добавить к Score
    │     │
    │     └─ Показать анимацию очистки
    │        └─ Через 300ms освежить BoardUI
    │                                          │
    │  5. Обновить блоки:                      │
    │     NextBlocks.RemoveAt(blockIndex)      │
    │     if NextBlocks.Count == 0:            │
    │         NextBlocks = GetNext3()          │
    │                                          │
    │  6. Events:                              │
    │     BoardUpdated?.Invoke() → RefreshUI   │
    │     ScoreChanged?.Invoke(score)          │
    │                                          │
    │  7. Проверка конца игры:                 │
    │     if Board.IsGameOver(NextBlocks):     │
    │         IsOver = true, IsRunning = false │
    │         GameOver?.Invoke()               │
    │         ↓ Переход к шагу 5️⃣ ↓
    └──────────────────────────────────────────┘

    ┌──────────────────────────────────────────┐
    │  ВЕРНУТЬСЯ К НАЧАЛУ ЦИКЛА (или конец)    │
    └──────────────────────────────────────────┘

5️⃣  КОНЕЦ ИГРЫ - GameOver
    ├─ Проверка IsGameOver() вернула true
    │  (ни один из 3 блоков не может быть размещён)
    │
    ├─ Game.GameOver event
    │  └─ OnGameOver() на GamePage
    │     ├─ ScoreService.Save(player)
    │     │  └─ Сохранить результат в топ-10
    │     │
    │     └─ ShowGameOverDialog()
    │        ├─ Показать финальные очки
    │        ├─ Показать ранг в рейтинге
    │        └─ Кнопки: "Попробовать ещё" / "Меню"
    │
    ├─ Если "Попробовать ещё" → новая игра (GamePage)
    └─ Если "Меню" → возврат на StartPage

6️⃣  ЗАВЕРШЕНИЕ / ВЫХОД
    ├─ Может быть нажата кнопка выхода
    ├─ Может быть нажата кнопка "Settings" для изменения языка/темы
    └─ Может быть нажата кнопка "Logout" для выхода из аккаунта
```

---

## 📊 Система очков (Scoring System)

### Детальный расчёт

```
ИТОГО ОЧКИ = Очки за размещение + Очки за очистку линий

1️⃣  ОЧКИ ЗА РАЗМЕЩЕНИЕ
    Formula: количество_клеток_в_блоке × 1

    Примеры:
    • Single (1×1) → 1 очко
    • LineH3 (1×3) → 3 очка
    • Square (2×2) → 4 очка
    • LineH (1×4) → 4 очка
    • LineV (4×1) → 4 очка
    • TShape (T-форма) → 5 очков
    • LShape (L-форма) → 4 очка
    • Square3 (3×3) → 9 очков (максимум!)

2️⃣  ОЧКИ ЗА ОЧИСТКУ ЛИНИЙ

    a) БАЗОВЫЙ БОНУС (зависит от количества линий):
       • 1 линия → 10 очков
       • 2 линии → 20 очков
       • 3 линии → 60 очков (скачок!)
       • 4 линии → 120 очков
       • 5 линий → 200 очков
       • 6+ линий → 300 очков (максимум)

    b) БОНУС ЗА ПОЛНУЮ ОЧИСТКУ ДОСКИ:
       • Если очищена ВСЯ ДОСКА (все 64 клетки) → +360 очков!

    c) КОМБО-МУЛЬТИПЛИКАТОР ⭐

       ComboCount = количество подряд уничтожений линий

       Мультипликатор = 1.0 + (ComboCount - 1) × 0.5

       ComboCount 1 → × 1.0  (базовое значение)
       ComboCount 2 → × 1.5  (50% больше)
       ComboCount 3 → × 2.0  (100% больше)
       ComboCount 4 → × 2.5  (150% больше)
       ComboCount 5 → × 3.0  (200% больше!)
       ComboCount 10 → × 5.5 (550% больше!!!)

       ИТОГО = BaseBonus × Multiplier

3️⃣  ПРИМЕРЫ РАСЧЁТОВ

    Пример 1: Простой ход
    ─────────────────────
    • Разместить Square (4 клетки) → +4 очка
    • Линии не очищены
    • Комбо сброшен (ComboCount = 0)
    Всего: 4 очка

    Пример 2: Первое уничтожение
    ────────────────────────────
    • Разместить блок 3×1 → +3 очка
    • Очищена 1 строка
    • Base: 10, Multiplier: 1.0x
    • Добавлено: 10 × 1.0 = 10 очков
    Всего: 3 + 10 = 13 очков

    Пример 3: Комбо на 2-м ходу
    ─────────────────────────────
    • (ComboCount уже был 1 от предыдущего хода)
    • Разместить блок 2×2 → +4 очка
    • Очищены 2 строки
    • Base: 20, Multiplier: 1.5x (ComboCount = 2)
    • Добавлено: 20 × 1.5 = 30 очков
    Всего: 4 + 30 = 34 очка

    Пример 4: БОЛЬШОЕ КОМБО! 🔥
    ────────────────────────────
    • (ComboCount = 4 от предыдущих ходов)
    • Разместить Square3 (9 клеток) → +9 очков
    • Очищены 3 строки
    • Base: 60, Multiplier: 2.5x (ComboCount = 5)
    • Добавлено: 60 × 2.5 = 150 очков
    Всего: 9 + 150 = 159 очков за один ход!!!

    Пример 5: ОЧИСТКА ВСЕЙ ДОСКИ 💥
    ───────────────────────────────
    • (ComboCount = 3)
    • Разместить блок 4×1 → +4 очка
    • Очищены 4 строки И вся доска пуста!
    • Base: 120 + 360 (доска) = 480
    • Multiplier: 2.0x (ComboCount = 4)
    • Добавлено: 480 × 2.0 = 960 очков!!!!
    Всего: 4 + 960 = 964 очка за один ход!!!!!!

4️⃣  ГРАФИК КОМБО

    Комбо #  Мультипликатор  Эффект
    ─────────────────────────────────
       0      0.0x            (комбо сброшено)
       1      1.0x            базовый
       2      1.5x            50% бонус
       3      2.0x            100% бонус
       4      2.5x            150% бонус
       5      3.0x            200% бонус ⭐
       6      3.5x            250% бонус
      10      5.5x            550% бонус 🔥
      20     10.5x           1050% бонус 🌪️
```

### Как сбрасывается комбо?

```
ComboCount сбрасывается в 0 если:
├─ Размещён блок БЕЗ уничтожения линий
└─ Вызовется Player.ResetCombo()

Комбо СОХРАНЯЕТСЯ если:
├─ Размещён блок И уничтожены линии
├─ Сразу же размещается следующий блок БЕЗ очистки
│  → тогда обнуляется
└─ (Промежуток времени не имеет значения - только физический ход)
```

---

## 💡 Примеры кода

### Пример 1: Создание новой игры

```csharp
// В MauiProgram.cs регистрируются сервисы
builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton<ThemeService>();
builder.Services.AddSingleton<ScoreService>();

// На StartPage
var player = new Player("Alice", bestScore: 1500);
var gameMode = GameMode.Easy;  // или GameMode.Hard

// Создание Game
var game = new Game(player, gameMode);

// Подписка на события
game.GameStarted += () => Console.WriteLine("Игра началась!");
game.ScoreChanged += (score) => Console.WriteLine($"Очки: {score}");
game.LinesCleared += (count, lines, combo, boardClear) => 
{
    Console.WriteLine($"Уничтожено {count} линий!");
    Console.WriteLine($"Комбо: {combo}x");
    if (boardClear) Console.WriteLine("ДОСКА ОЧИЩЕНА!");
};
game.GameOver += () => Console.WriteLine("Игра окончена!");

// Начало игры
game.Start();  // Easy: пустая доска, Hard: 12 случайных
```

### Пример 2: Размещение блока

```csharp
// Получить блок
var block = game.NextBlocks[0];  // Первый из 3 блоков

// Попытка разместить на (2, 3)
bool success = game.TryPlaceBlock(blockIndex: 0, row: 2, col: 3);

if (success)
{
    Console.WriteLine("✓ Блок размещён!");
    Console.WriteLine($"Новые очки: {game.Player.Score}");
    Console.WriteLine($"Следующих блоков: {game.NextBlocks.Count}");
}
else
{
    Console.WriteLine("✗ Не удалось разместить блок");
}
```

### Пример 3: Сохранение и загрузка рейтинга

```csharp
var scoreService = new ScoreService();
var player = new Player("Bob", 0);

// ... Игра ...

player.Score = 2500;  // После игры

// Сохранить
scoreService.Save(player);

// Позже загрузить топ-10
var topScores = scoreService.GetTopScores();

foreach (var entry in topScores.Take(10))
{
    Console.WriteLine($"{entry.Name}: {entry.Score}");
}

// Output:
// Alice: 3840
// Bob: 2500
// Charlie: 1200
// ...
```

### Пример 4: Система аккаунтов

```csharp
var accountService = new AccountService();

// Вход на LoginPage
accountService.SetCurrentName("Diana");

// На StartPage или другой странице
if (accountService.IsLoggedIn)
{
    string name = accountService.GetCurrentName();  // "Diana"
    var player = new Player(name);
    // ... запустить игру ...
}

// На SettingsPage - выход
if (userClickedLogout)
{
    accountService.Logout();
    // Перейти на LoginPage
}
```

### Пример 5: Проверка конца игры

```csharp
// В конце TryPlaceBlock():

// Получить 3 следующих блока
var nextBlocks = game.NextBlocks;

// Проверить, может ли каждый быть размещён
foreach (var block in nextBlocks)
{
    if (game.Board.CanPlaceAnywhere(block))
    {
        return false;  // Есть место! Игра продолжается
    }
}

// Если сюда дошли - ни один блок не может быть размещён
return true;  // GAME OVER!
```

---

## 🔧 DI Контейнер (MauiProgram.cs)

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()  // Использовать App как entry point
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ── РЕГИСТРАЦИЯ СЕРВИСОВ ──────────────────────────────

        // Singleton - один экземпляр на всё приложение
        builder.Services.AddSingleton<AccountService>();   // Один для аккаунта
        builder.Services.AddSingleton<ThemeService>();     // Один для темы
        builder.Services.AddSingleton<ScoreService>();     // Один для рейтингов

        // Transient - новый экземпляр каждый раз
        builder.Services.AddTransient<StartPage>();   // Новая каждый раз
        builder.Services.AddTransient<GamePage>();    // Новая для каждой игры
        builder.Services.AddTransient<SettingsPage>(); // Новая каждый раз

#if DEBUG
        builder.Logging.AddDebug();  // Логирование в Debug
#endif

        return builder.Build();  // Построить и вернуть MauiApp
    }
}
```

---

## 🎨 Визуальная иерархия UI

```
┌───────────────────────────────────────────────────────┐
│                   STATUS BAR (iOS)                    │
├───────────────────────────────────────────────────────┤
│                                                       │
│              SCORE & BEST (Заголовки)                │
│      ┌─────────────┐        ┌─────────────┐         │
│      │   SCORE     │        │    🏆 BEST  │         │
│      │   1250      │        │    3840     │         │
│      └─────────────┘        └─────────────┘         │
│                                                       │
│         ┌──────────────────────────────────┐         │
│         │     GAME BOARD (8×8)             │         │
│         │                                  │         │
│         │  ██ ██ ██ ██ ██ ██ ██ ██        │         │
│         │  ██ ██ ██ ██ ██ ██ ██ ██        │         │
│         │  ██ ██ ██ ██ ██ ██ ██ ██        │         │
│         │  ██ ██ ██ ██ ██ ██ ██ ██        │         │
│         │  ██ ██ ██ ██ ██ ██ ██ ██        │         │
│         │  ██ ██ ██ ██ ██ ██ ██ ██        │         │
│         │  ██ ██ ██ ██ ██ ██ ██ ██        │         │
│         │  ██ ██ ██ ██ ██ ██ ██ ██        │         │
│         │                                  │         │
│         └──────────────────────────────────┘         │
│                                                       │
│      NEXT BLOCKS (3 следующих)                      │
│     ┌──────┐  ┌──────┐  ┌──────┐                    │
│     │ ██   │  │██ ██ │  │██ ██ │                    │
│     │ ██   │  │██ ██ │  │██ ██ │                    │
│     │ ██   │  │      │  │██ ██ │                    │
│     │ ██   │  │      │  │██ ██ │                    │
│     └──────┘  └──────┘  └──────┘                    │
│                                                       │
└───────────────────────────────────────────────────────┘
```

---

## 🌐 Локализация (i18n)

Приложение поддерживает 3 языка:

1. **English** 🇬🇧 — английский
2. **Русский** 🇷🇺 — русский
3. **Eesti** 🇪🇪 — эстонский

**Переключение:**
```csharp
LanguageService.ChangeLanguage("en");  // English
LanguageService.ChangeLanguage("ru");  // Русский
LanguageService.ChangeLanguage("et");  // Eesti
```

Все строки хранятся в `Resources/Localization/AppResources.*.resx` файлах.

---

## 📈 Статистика и метрики

После каждой игры сохраняются:

```csharp
player.Score              // Финальные очки
player.BestScore          // Лучший результат (всех игр)
player.MaxCombo           // Максимальное комбо за эту игру
player.TotalLinesCleared  // Всего уничтожено линий
```

Эти данные используются для:
- Отображения в меню
- Рейтингов (TopScores)
- Статистики игрока

---

## 🚀 Заключение

**Block Blast** - это полноценная мобильная игра с:

✅ Классической геймеханикой (размещение + очистка)  
✅ Системой комбо для экспоненциального роста очков  
✅ Двумя режимами сложности  
✅ Рейтингом топ-10 игроков  
✅ Управлением аккаунтом  
✅ Поддержкой множества языков  
✅ Cross-platform поддержкой (iOS, Android, Windows)  
✅ Чистой архитектурой (Model-View-ViewModel)  

Все компоненты хорошо разделены и переиспользуемы! 🎮

---

**Документация подготовлена:** 2025  
**Версия:** 1.0  
**Язык:** Русский  
**Автор оригинала:** Oleksandra Ryshniak  
**GitHub:** https://github.com/OleksandraRyshniak/Block_Blast
