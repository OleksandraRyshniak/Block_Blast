namespace Block_Blast.Models;

public class Player
{
    
    public string Name { get; set; }
    public int Score { get; private set; }
    public int BestScore { get; private set; }

   
    public int ComboCount { get; private set; }

    
    public int MaxCombo { get; private set; }

    public int TotalLinesCleared { get; private set; }

    public Player(string name, int bestScore = 0)
    {
        Name = name;
        Score = 0;
        BestScore = bestScore;
        ComboCount = 0;
        MaxCombo = 0;
        TotalLinesCleared = 0;
    }

    public void AddPlacementScore(int cellsPlaced)
    {
        AddPoints(cellsPlaced);
    }

    public void AddLineScore(int linesCleared, bool isBoardClear = false)
    {
        ComboCount++;
        if (ComboCount > MaxCombo) MaxCombo = ComboCount;

        TotalLinesCleared += linesCleared;

        int lineBonus = linesCleared switch
        {
            1 => 10,
            2 => 20,
            3 => 60,
            4 => 120,
            5 => 200,
            _ => 300   
        };

    
        if (isBoardClear) lineBonus += 360;

        double multiplier = 1.0 + (ComboCount - 1) * 0.5;
        int total = (int)Math.Round(lineBonus * multiplier);

        AddPoints(total);
    }

    public void ResetCombo()
    {
        ComboCount = 0;
    }

    public void ResetScore()
    {
        Score = 0;
        ComboCount = 0;
        MaxCombo = 0;
        TotalLinesCleared = 0;
    }

    private void AddPoints(int points)
    {
        Score += points;
        if (Score > BestScore)
            BestScore = Score;
    }
}