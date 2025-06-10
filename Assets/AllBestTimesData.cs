using System.Collections.Generic;

[System.Serializable]
public class AllBestTimesData
{
    public List<LevelBestTime> levelTimes;

    public AllBestTimesData()
    {
        levelTimes = new List<LevelBestTime>();
    }
}