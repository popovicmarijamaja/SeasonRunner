using Fusion;
using UnityEngine;

public enum ObstacleType
{
    OneRock,
    TwoRocks,
    WoodenObstacle,
    WoodenObstacleWithStar,
    WoodenObstacleWithMushrooms,
    Mushrooms,
    WalkingSoldier,
    ShootingSoldier
}

public enum PowerUpType
{
    Magnet,
    HealthPack,
    Shield,
    Gun
}

public class ChanceManager : MonoBehaviour
{
    public static ChanceManager Instance { get; private set; }

    //[Networked]
    public static int peed { get; set; }


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
    private void Start()
    {
        peed++;
        Random.InitState(peed);
    }

    /*public void SetSeed(int seed)
    {
        Random.InitState(seed);
        peed = seed;
    }*/

    public ObstacleType ChooseObstacleType()
    {
        float obstacleTypeChance = Random.value;

        // 70% chance for rock
        if (obstacleTypeChance <= 0.7f)
        {
            return ChooseRockType();
        }

        // 10% chance for wooden obstacle
        else if (obstacleTypeChance <= 0.8f)
        {
            return ChooseWoodenObstacleType();
        }

        // 10% for mushrooms
        else if (obstacleTypeChance <= 0.9f)
        {
            return ChooseMushroomsType();
        }

        // 10% for enemy
        else
        {
            return ChooseEnemyType();
        }
    }

    private ObstacleType ChooseRockType()
    {
        float rockTypeChance = Random.value;

        // 70% chance for one rock
        if (rockTypeChance <= 0.7f)
            return ObstacleType.OneRock;

        // 30% chance for two rocks
        else
            return ObstacleType.TwoRocks;
    }

    private ObstacleType ChooseWoodenObstacleType()
    {
        float woodenObstacleTypeChance = Random.value;

        // 40% chance for wooden obstacle with star
        if (woodenObstacleTypeChance <= 0.4f)
            return ObstacleType.WoodenObstacleWithStar;

        // 40% chance for wooden obstacle with mushrooms
        else if (woodenObstacleTypeChance <= 0.8f)
            return ObstacleType.WoodenObstacleWithMushrooms;

        // 20% chance for empty wooden obstacle
        else
            return ObstacleType.WoodenObstacle;
    }

    private ObstacleType ChooseMushroomsType()
    {
        return ObstacleType.Mushrooms;
    }

    private ObstacleType ChooseEnemyType()
    {
        float enemyTypeChance = Random.value;

        // 50% chance for walking soldier
        if (enemyTypeChance <= 0.5f)
            return ObstacleType.WalkingSoldier;

        // 50% chance for shooting soldier
        else
            return ObstacleType.ShootingSoldier;
    }

    public PowerUpType ChoosePowerUp()
    {
        float powerUpChance = Random.value;
        
        // 20% chance for magnet
        if (powerUpChance <= 0.2f)
            return PowerUpType.Magnet;

        // 20% chance for health pack
        else if (powerUpChance <= 0.4f)
            return PowerUpType.HealthPack;

        // 20% chance for shield
        else if (powerUpChance <= 0.6f)
            return PowerUpType.Shield;

        // 20% chance for gun
        else
            return PowerUpType.Gun;
    }
}
