using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "RotaAchievements/Achievement/New Achievement")]
public class Achievement : ScriptableObject
{
     public string id;
    public string title;
    public string description;
    public int rewardAmount;
    public bool isRepeatable;

}
