using UnityEngine;

public class ChanceManager : MonoBehaviour
{
    public static ChanceManager Instance { get; private set; }

    private const int OneRock = 1;
    private const int TwoRocks = 2;
    private const int WoodenObstacle = 3;
    private const int WoodenObstacleWithStar = 4;
    private const int WoodenObstacleWithMushrooms = 5;
    private const int Mushrooms = 6;
    private const int WalkingSoldier = 7;
    private const int ShootingSoldier = 8;
    private const int Magnet = 1;
    private const int HealthPack = 2;
    private const int Shield = 3;
    private const int Gun = 4;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public int ChooseObstacleType()
    {
        float obstacleChance = Random.value;

        // 70% chance for rock --> 50% for one rock, 20% for two rocks
        if (obstacleChance <= 0.7f)
        {
            if (obstacleChance < 0.5f)
                return OneRock;
            else
                return TwoRocks;
        }

        // 10% chance for wooden obstacle --> 4% with star, 4% with mushrooms, 2% empty
        else if (obstacleChance <= 0.8f)
        {
            if (obstacleChance <= 0.74f)
                return WoodenObstacleWithStar;
            else if (obstacleChance <= 0.78f)
                return WoodenObstacleWithMushrooms;
            else
                return WoodenObstacle;
        }

        // 10% for mushrooms
        else if (obstacleChance <= 0.9f)
        {
            return Mushrooms;
        }

        // 10% for enemy --> 5% for walking soldier, 5% for shooting soldier
        else
        {
            if (obstacleChance <= 0.95f)
                return WalkingSoldier;
            else
                return ShootingSoldier;
        }
    }

    public int ChoosePowerUp()
    {
        float powerUpChance = Random.value;
        
        // 20% chance for magnet
        if (powerUpChance <= 0.2f)
            return Magnet;

        // 20% chance for health pack
        else if (powerUpChance <= 0.4f)
            return HealthPack;

        // 20% chance for shield
        else if (powerUpChance <= 0.6f)
            return Shield;

        // 20% chance for gun
        else
            return Gun;
    }
}
