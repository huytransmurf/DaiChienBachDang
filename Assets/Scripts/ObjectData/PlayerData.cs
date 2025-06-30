using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public Dictionary<string, bool> unlockedSkills = new Dictionary<string, bool>();
    public Dictionary<string, int> upgradedSkills = new Dictionary<string, int>();
    public int gold;
    public float playerHealth;
}
