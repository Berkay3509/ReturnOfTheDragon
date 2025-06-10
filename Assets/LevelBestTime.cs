[System.Serializable]
public class LevelBestTime
{
    public string levelName; // Seviyenin adý (Scene adý olabilir)
    public float bestTime;

    public LevelBestTime(string name, float time)
    {
        levelName = name;
        bestTime = time;
    }
}