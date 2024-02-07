using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    private const int NumberOfCoinsPerChunk = 11;
    private const int NumberOfObstaclesPerChunk = 11;
    private const float DistanceBetweenObstacles = -7.7f;
    private const float DistanceBetweenCoins = -7.7f;
    private const float FirstDistanceObstacle = -32.5f;
    private const float FirstDistanceCoin = -28.8f;
    private const float LeftLane = -1f;
    private const float CentreLane = 0f;
    private const float RightLane = 1f;

    private float randomPosZ;
    private float posX;

    [SerializeField] private Transform environmentSpawnPos;
    [SerializeField] private EnemyData shootingEnemy;
    [SerializeField] private EnemyData walkingEnemy;
    private Transform parentEnvironment;

    private readonly List<GameObject> ObjectsInEnvironment = new();
    private readonly float[] posForZ = { LeftLane, CentreLane, RightLane };

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

    public void SpawnNewSection()
    {
        GameObject environment = ObjectPool.Instance.GetEnvironment();
        if (environment == null)
            return;
        SetEnvironment(environment);
        SpawnObstacle();
        SpawnCoins();
        SpawnPowerUp();
    }

    private void SetEnvironment(GameObject environment)
    {
        environment.transform.position = environmentSpawnPos.position;
        environment.SetActive(true);
        parentEnvironment = environment.transform;
    }

    private void SpawnObstacle()
    {
        for (int i = 0; i < NumberOfObstaclesPerChunk; i++)
        {
            GeneratePosX(i, FirstDistanceObstacle, DistanceBetweenObstacles);
            GenerateRandomPosZ();

            switch (ChanceManager.Instance.ChooseObstacleType())
            {
                case ObstacleType.OneRock:
                    SpawnOneRock(posX, randomPosZ);
                    break;
                case ObstacleType.TwoRocks:
                    SpawnTwoRocks(posX, randomPosZ, posForZ);
                    break;
                case ObstacleType.WoodenObstacleWithStar:
                    SpawnWoodenObstacleWithStar(posX, CentreLane);
                    break;
                case ObstacleType.WoodenObstacleWithMushrooms:
                    SpawnWoodenObstacleWithMushrooms(posX, CentreLane);
                    break;
                case ObstacleType.WoodenObstacle:
                    SpawnWoodenObstacle(posX, CentreLane);
                    break;
                case ObstacleType.Mushrooms:
                    SpawnMushrooms(posX, randomPosZ);
                    break;
                case ObstacleType.WalkingSoldier:
                    SpawnWalkingSoldier(posX, CentreLane);
                    break;
                case ObstacleType.ShootingSoldier:
                    SpawnShootingSoldier(posX, randomPosZ);
                    break;
            }
        }
    }

    private void SpawnOneRock(float posX, float randomPosZ)
    {
        SpawnObject(ObjectPool.Instance.GetRock(), posX, randomPosZ);
    }

    private void SpawnTwoRocks(float posX, float randomPosZ, float[] posForZ)
    {
        // Making sure that second rock does not have the same z position as first
        (float newFirstPos, float newSecondPos) = MakeDiferentPos(posForZ, randomPosZ);

        SpawnObject(ObjectPool.Instance.GetRock(), posX, newFirstPos);
        SpawnObject(ObjectPool.Instance.GetRock(), posX, newSecondPos);
    }

    private (float, float) MakeDiferentPos(float[] pos, float secondPos)
    {
        float firstPos;
        do
        {
            int randomIndex = Random.Range(0, 3);
            firstPos = pos[randomIndex];
        } while (secondPos == firstPos);
        return (firstPos, secondPos);
    }

    private void SpawnWoodenObstacleWithStar(float posX, float neutralPosZ)
    {
        SpawnObject(ObjectPool.Instance.GetWoodenObstacle(), posX, neutralPosZ);
        SpawnObject(ObjectPool.Instance.GetStar(), posX, neutralPosZ);
    }

    private void SpawnWoodenObstacleWithMushrooms(float posX, float neutralPosZ)
    {
        SpawnObject(ObjectPool.Instance.GetWoodenObstacle(), posX, neutralPosZ);
        SpawnObject(ObjectPool.Instance.GetMushrooms(), posX, neutralPosZ);
    }

    private void SpawnWoodenObstacle(float posX,float neutralPosZ)
    {
        SpawnObject(ObjectPool.Instance.GetWoodenObstacle(), posX, neutralPosZ);
    }

    private void SpawnMushrooms(float posX, float randomPosZ)
    {
        SpawnObject(ObjectPool.Instance.GetMushrooms(), posX, randomPosZ);
    }

    private void SpawnWalkingSoldier(float posX, float neutralPosZ)
    {
        GameObject walkingSoldier = ObjectPool.Instance.GetSoldier();
        SpawnObject(walkingSoldier, posX, neutralPosZ);
        walkingSoldier.GetComponentInChildren<EnemyController>().InnateEnemy(walkingEnemy);
        walkingSoldier.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void SpawnShootingSoldier(float posX, float posZ)
    {
        GameObject shootingSoldier = ObjectPool.Instance.GetSoldier();
        SpawnObject(shootingSoldier, posX, posZ);
        shootingSoldier.GetComponentInChildren<EnemyController>().InnateEnemy(shootingEnemy);
        shootingSoldier.transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    private void SpawnCoins()
    {
        for (int i = 0; i < NumberOfCoinsPerChunk; i++)
        {
            GenerateRandomPosZ();
            GeneratePosX(i, FirstDistanceCoin, DistanceBetweenCoins);
            SpawnObject(ObjectPool.Instance.GetCoin(), posX, randomPosZ);
        }
    }

    private void GeneratePosX(int i, float firstDistance, float distanceBetweenObjects)
    {
        posX = firstDistance + (i * distanceBetweenObjects);
    }

    private void SpawnPowerUp()
    {
        ChooseFromTwoPosX(-30f, -38f);
        GenerateRandomPosZ();

        switch (ChanceManager.Instance.ChoosePowerUp())
        {
            case PowerUpType.Magnet:
                SpawnObject(ObjectPool.Instance.GetMagnet(), posX, randomPosZ);
                break;
            case PowerUpType.HealthPack:
                SpawnObject(ObjectPool.Instance.GetHealthPack(), posX, randomPosZ);
                break;
            case PowerUpType.Shield:
                SpawnObject(ObjectPool.Instance.GetShield(), posX, randomPosZ);
                break;
            case PowerUpType.Gun:
                SpawnObject(ObjectPool.Instance.GetGun(), posX, randomPosZ);
                break;
        }
    }

    private void ChooseFromTwoPosX(float firstPos, float secondPos)
    {
        float[] posForX = { firstPos, secondPos };
        int randomIndexX = Random.Range(0, 2);
        posX = posForX[randomIndexX];
    }

    private void GenerateRandomPosZ()
    {
        int randomIndexZ = Random.Range(0, 3);
        randomPosZ = posForZ[randomIndexZ];
    }

    private void SpawnObject(GameObject obj, float posX, float posZ)
    {
        if (obj == null)
            return;
        
        obj.SetActive(true);
        obj.transform.parent = parentEnvironment;
        obj.transform.SetPositionAndRotation(new Vector3(posX, obj.transform.position.y, posZ), obj.transform.rotation);
        ObjectsInEnvironment.Add(obj);
    }

    public void ReturnObjectsInEnvironmentToPool(GameObject environment)
    {
        if (ObjectsInEnvironment == null)
            return;
        for (int i = ObjectsInEnvironment.Count - 1; i >= 0; i--)
        {
            GameObject obj = ObjectsInEnvironment[i];

            if (obj.transform.parent == environment.transform)
            {
                ObjectPool.Instance.ReturnToPool(obj);
                RemoveGameObjectFromList(ObjectsInEnvironment, obj);
            }
        }
    }

    private void RemoveGameObjectFromList(List<GameObject> list, GameObject obj)
    {
        if (list.Contains(obj))
            list.Remove(obj);
    }

}
