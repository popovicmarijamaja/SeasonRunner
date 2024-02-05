using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string Name;
    public float Speed;
    public bool isShooting;
    public string AliveParameter;
    public RuntimeAnimatorController AnimatorController;
}
