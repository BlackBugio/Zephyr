public class DungeonBase
{
    public string name;
    private readonly int difficulty = 3;
    public Difficulty _difficulty;
    private int duration;
    private int probability;
    private float prize;
    private bool requisite;


    public enum Difficulty
    {
        VeryEasy,
        Easy,
        Normal,
        Hard,
        VeryHard,
        Impossible
    }
    public DungeonBase(string aDungeonName, int aDifficulty)
    {
        name = aDungeonName;
        difficulty = aDifficulty;
        GetDifficulty = (Difficulty)difficulty;
        Duration = difficulty;
        Probability = difficulty;
        Prize = difficulty;
    }

    public int Duration
    {
        get { return duration; }
        set
        {
            switch (difficulty)
            {
                case 0: duration = 4; break;
                case 1: duration = 6; break;
                case 2: duration = 8; break;
                case 3: duration = 16; break;
                case 4: duration = 32; break;
                case 5: duration = 64; break;
            };
        }
    }

    public int Probability
    {
        get { return probability; }
        set
        {
            switch (difficulty)
            {
                case 0: probability = 75; break;
                case 1: probability = 60; break;
                case 2: probability = 45; break;
                case 3: probability = 25; break;
                case 4: probability = 10; break;
                case 5: probability = 1; break;
            };
        }
    }

    public float Prize
    {
        get { return prize; }
        set
        {
            switch (difficulty)
            {
                case 0: prize = 3.5f; break;
                case 1: prize = 4; break;
                case 2: prize = 7; break;
                case 3: prize = 25; break;
                case 4: prize = 100; break;
                case 5: prize = 500; break;
            };
        }
    }


    public Difficulty GetDifficulty
    {
        get { return _difficulty; }
        set { _difficulty = (Difficulty)difficulty; }
    }
}
