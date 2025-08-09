using UnityEngine;

[System.Serializable]
public class Upgrade
{
    public int id;
    public string upgradeName;
    public Sprite icon;
    [TextArea] public string description;
    public int cost;
    public int prerequisiteId; 
}
