[System.Serializable]
public class LevelBestTime
{
    public string levelName; // Seviyenin ad� (Scene ad� olabilir)
    public float bestTime;

    public LevelBestTime(string name, float time)
    {
        levelName = name;
        bestTime = time;
    }
}