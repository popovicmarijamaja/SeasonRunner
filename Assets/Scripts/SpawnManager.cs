using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private const int NumberOfCoinsPerChunk = 11;
    private const int NumberOfObstaclesPerChunk = 11;
    private const float DistanceBetweenObstacles = -7.7f;
    private const float DistanceBetweenCoins = -7.7f;
    private const float FirstDistanceObstacle = -51.5f;
    private const float FirstDistanceCoin = -47.8f;

    [SerializeField] private EnemyData shootingEnemy;
    [SerializeField] private EnemyData walkingEnemy;

    private readonly List<GameObject> ObjectsInEnvironment = new();

    private void SpawnObstacle()
    {
        float posX;
        float randomPosZ;
        int randomIndex;
        float[] posForZ = { -0.5f, 0.5f, 1.6f };
        float neutralPosZ = posForZ[1];


        for (int i = 0; i < NumberOfObstaclesPerChunk; i++)
        {
            posX = FirstDistanceObstacle + (i * DistanceBetweenObstacles);
            randomIndex = Random.Range(0, 3);
            randomPosZ = posForZ[randomIndex];

            switch (ChanceManager.Instance.ChooseObstacleType())
            {
                case 1:
                    SpawnOneRock(posX, randomPosZ);
                    break;
                case 2:
                    SpawnTwoRocks(posX, randomPosZ, posForZ);
                    break;
                case 3:
                    SpawnWoodenObstacleWithStar(posX, neutralPosZ);
                    break;
                case 4:
                    SpawnWoodenObstacleWithMushrooms(posX, neutralPosZ);
                    break;
                case 5:
                    SpawnWoodenObstacle(posX, neutralPosZ);
                    break;
                case 6:
                    SpawnMushrooms(posX, randomPosZ);
                    break;
                case 7:
                    SpawnWalkingSoldier(posX, neutralPosZ);
                    break;
                case 8:
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
        walkingSoldier.GetComponentInChildren<EnemyController>().enemyData = walkingEnemy;
        walkingSoldier.GetComponentInChildren<EnemyController>().SetEnemy();
        walkingSoldier.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void SpawnShootingSoldier(float posX, float randomPosZ)
    {
        GameObject shootingSoldier = ObjectPool.Instance.GetSoldier();
        SpawnObject(shootingSoldier, posX, randomPosZ);
        shootingSoldier.GetComponentInChildren<EnemyController>().enemyData = shootingEnemy;
        shootingSoldier.GetComponentInChildren<EnemyController>().SetEnemy();
        shootingSoldier.transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    private void SpawnCoins()
    {
        float randomPosZ;
        float posX;
        int randomIndex;
        float[] posForZ = { -1f, 0f, 1f };

        for (int i = 0; i < NumberOfCoinsPerChunk; i++)
        {
            randomIndex = Random.Range(0, 3);
            randomPosZ = posForZ[randomIndex];
            posX = FirstDistanceCoin + (i * DistanceBetweenCoins);

            SpawnObject(ObjectPool.Instance.GetCoin(), posX, randomPosZ);
        }
    }

    private void SpawnPowerUp()
    {
        float[] posForZ = { -1.2f, 0f, 1.2f };
        float[] posForX = { -49f, -57f };
        int randomIndexZ = Random.Range(0, 3);
        int randomIndexX = Random.Range(0, 2);
        float randomPosZ = posForZ[randomIndexZ];
        float randomPosX = posForX[randomIndexX];

        switch (ChanceManager.Instance.ChoosePowerUp())
        {
            case 1:
                SpawnObject(ObjectPool.Instance.GetMagnet(), randomPosX, randomPosZ);
                break;
            case 2:
                SpawnObject(ObjectPool.Instance.GetHealthPack(), randomPosX, randomPosZ);
                break;
            case 3:
                SpawnObject(ObjectPool.Instance.GetShield(), randomPosX, randomPosZ);
                break;
            case 4:
                SpawnObject(ObjectPool.Instance.GetGun(), randomPosX, randomPosZ);
                break;
        }
    }

    private void SpawnObject(GameObject obj, float posX, float posZ)
    {
        if (obj == null)
            return;
        
        obj.SetActive(true);
        obj.transform.parent = transform.parent;
        obj.transform.SetPositionAndRotation(new Vector3(posX, obj.transform.position.y, posZ), obj.transform.rotation);
        ObjectsInEnvironment.Add(obj);
    }

    private void ReturnObjectsInEnvironmentToPool()
    {
        if (ObjectsInEnvironment == null)
            return;
        for (int i = 0; i < ObjectsInEnvironment.Count; i++)
        {
            GameObject obj = ObjectsInEnvironment[i];
            ObjectPool.Instance.ReturnToPool(obj);
        }
    }

    private void ClearListOfGameObjects(List<GameObject> list)
    {
        list.Clear();
    }

    private void OnEnable()
    {
        if (!ObjectPool.Instance.isInitialized)
            return;
        ReturnObjectsInEnvironmentToPool();
        ClearListOfGameObjects(ObjectsInEnvironment);
        SpawnObstacle();
        SpawnCoins();
        SpawnPowerUp();
    }
}
