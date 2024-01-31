using UnityEngine;

// CR: Ova klasa treba da drzi info o ponasanju neprijatelja
[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string Name;
    public float Speed;
    public string AliveParameter;
    public string FireTag; // CR: Fire tag ne pripada u EnemyData
}
