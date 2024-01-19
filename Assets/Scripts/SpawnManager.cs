using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    public const int NumberOfCoin = 11;
    private const int NumberOfObstacles = 11;
    private const int NumberOfRockInLine = 2;
    private const float RCSGPosY = 0.5f; //for rock, coin, shield, star and gun
    private const float WEMPosY = 0f; //for wooden obstacle, enemy and mushrooms
    private const float DistanceBetweenObstacles = -7.6f;
    private const float DistanceBetweenCoin = -7.6f;
    private const float FirstDistanceObstacle = -51.5f;
    private const float FirstDistanceCoin = 31.3f;
    private const string AliveParametar = "alive";

    private readonly List<GameObject> SpawnedRocks = new();
    private readonly List<GameObject> SpawnedWoodenObstacle = new();
    private readonly List<GameObject> SpawnedMushrooms = new();
    private readonly List<GameObject> SpawnedStars = new();
    public readonly List<GameObject> SpawnedCoins = new();
    public readonly List<GameObject> SpawnedEnemy = new();

    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private GameObject woodenObstaclePrefab;
    [SerializeField] private GameObject mushroomPrefab;
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject magnetPrefab;
    [SerializeField] private GameObject healthPackPrefab;
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject gunPrefab;

    private bool isCoinSpawned;
    private GameObject magnet;
    private GameObject healthPack;
    private GameObject shield;
    private GameObject gun;

    private void Start()
    {
        SpawnObstacle();
        RespawnObstacle();
        SpawnCoin();
        SpawnPower();
        RespawnPower();
    }

    public void SpawnObstacle()
    {
        float posX;
        float posZ = 0;
        GameObject obj;
        
        for (int i = 0; i < NumberOfObstacles; i++)
        {
            posX = FirstDistanceObstacle + (i * DistanceBetweenObstacles);

            //Rock spawn
            for (int j = 0; j < NumberOfRockInLine; j++)
            {
                obj = Instantiate(rockPrefab, new Vector3(posX, RCSGPosY, posZ), Quaternion.Euler(-90, 0, 0), transform.parent);
                SpawnedRocks.Add(obj);
            }

            //Wooden obstacle spawn
            obj = Instantiate(woodenObstaclePrefab, new Vector3(posX, WEMPosY, posZ), Quaternion.Euler(0, 0, 0), transform.parent);
            SpawnedWoodenObstacle.Add(obj);

            //Star spawn (wooden obstacle part)
            obj = Instantiate(starPrefab, new Vector3(posX, RCSGPosY, posZ), Quaternion.Euler(0, 0, 0), transform.parent);
            SpawnedStars.Add(obj);

            //Mushrooms spawn
            obj = Instantiate(mushroomPrefab, new Vector3(posX, WEMPosY, posZ), Quaternion.Euler(0, 0, 0), transform.parent);
            SpawnedMushrooms.Add(obj);

            //Enemy spawn
            obj = Instantiate(enemyPrefab, new Vector3(posX, WEMPosY, posZ), Quaternion.Euler(0, 0, 0), transform.parent);
            SpawnedEnemy.Add(obj);
        }
    }

    public void RespawnObstacle()
    {
        float randomPosZ;
        int randomIndex;
        float[] pos = { -0.5f, 0.5f, 1.6f };
        float[] posForMushrooms = { -1, 0, 1 };
        float chance;
        float neutralPosZ = 0f;
        int j = 1;
        for (int i = 0; i < NumberOfObstacles; i++)
        {
            chance = Random.value;
            randomIndex = Random.Range(0, 3);
            randomPosZ = pos[randomIndex];
            // Get the x position of the previous spawned rock
            float prevX = j > 0 ? SpawnedRocks[j - 1].transform.localPosition.x : float.MinValue;

            SpawnedRocks[j].SetActive(false);
            SpawnedRocks[j - 1].SetActive(false);
            SpawnedWoodenObstacle[i].SetActive(false);
            SpawnedMushrooms[i].SetActive(false);
            SpawnedStars[i].SetActive(false);
            SpawnedEnemy[i].SetActive(false);

            // 50% chance for 1 rock in line
            if (SpawnedRocks[j].transform.localPosition.x == prevX && chance <= 0.5f)
             {
                SpawnedRocks[j - 1].SetActive(true);
                SpawnedRocks[j - 1].transform.localPosition = new Vector3(SpawnedRocks[j].transform.localPosition.x, RCSGPosY, randomPosZ);
            }
             // 20% chance for 2 rocks in line
             else if(SpawnedRocks[j].transform.localPosition.x == prevX && chance > 0.5f && chance <= 0.7f)
             {
                SpawnedRocks[j].SetActive(true);
                SpawnedRocks[j - 1].SetActive(true);
                
                // Making sure that second rock does not have the same z position as first
                float prevZ = randomPosZ;
                do
                {
                    randomIndex = Random.Range(0, 3);
                    randomPosZ = pos[randomIndex];
                } while (randomPosZ == prevZ);

                SpawnedRocks[j].transform.localPosition = new Vector3(SpawnedRocks[j].transform.localPosition.x, RCSGPosY, randomPosZ);
                SpawnedRocks[j - 1].transform.localPosition = new Vector3(SpawnedRocks[j - 1].transform.localPosition.x, RCSGPosY, prevZ);
            }
             // 10% chance for wooden obsticle
             else if (SpawnedRocks[j].transform.localPosition.x == prevX && chance > 0.7f && chance <= 0.8f)
             {
                SpawnedWoodenObstacle[i].SetActive(true);

                chance = Random.value;
                // 4% chance for star
                if (chance <= 0.4f)
                {
                    SpawnedStars[i].SetActive(true);
                }
                // 4% chance for mushrooms
                else if (chance>0.4f && chance <= 0.8)
                {
                    SpawnedMushrooms[i].SetActive(true);
                    SpawnedMushrooms[i].transform.localPosition = new Vector3(SpawnedMushrooms[i].transform.localPosition.x, WEMPosY, neutralPosZ);
                }
                // 2% chance for empty wooden obsticle
                else if(chance > 0.8f)
                {
                    SpawnedStars[i].SetActive(false);
                    SpawnedMushrooms[i].SetActive(false);
                }
             }
             // 10% chance for mushrooms
             else if(SpawnedRocks[j].transform.localPosition.x == prevX && chance > 0.8f && chance <= 0.9f)
             {
                SpawnedMushrooms[i].SetActive(true);
                randomPosZ = posForMushrooms[randomIndex];
                SpawnedMushrooms[i].transform.localPosition = new Vector3(SpawnedMushrooms[i].transform.localPosition.x, WEMPosY, randomPosZ);
             }
             // 10% chance for enemy
             else if (SpawnedRocks[j].transform.localPosition.x == prevX && chance > 0.9f)
             {
                 SpawnedEnemy[i].SetActive(true);
                 SpawnedEnemy[i].transform.localPosition = new Vector3(SpawnedEnemy[i].transform.localPosition.x, WEMPosY, neutralPosZ);
                SpawnedEnemy[i].GetComponentInChildren<Animator>().SetBool(AliveParametar, true);
             }
            j += 2;
        }
    }

    public void SpawnCoin()
    {
        float randomPosZ;
        float posX;
        int randomIndex;
        float[] pos = { -1f, 0f, 1f };
        for (int i = 0; i < NumberOfCoin; i++)
        {
            randomIndex = Random.Range(0, 3);
            randomPosZ = pos[randomIndex];
            if (isCoinSpawned)
            {
                SpawnedCoins[i].SetActive(true);
                posX = FirstDistanceCoin + (i * DistanceBetweenCoin);
                SpawnedCoins[i].transform.localPosition = new Vector3(posX, RCSGPosY, randomPosZ);
            }
            else
            {
                posX = FirstDistanceCoin + (i * DistanceBetweenCoin);
                GameObject obj = Instantiate(coinPrefab, Vector3.zero, Quaternion.Euler(-90, 0, 0), transform.parent);
                obj.transform.localPosition = new Vector3(posX, RCSGPosY, randomPosZ);
                SpawnedCoins.Add(obj);
            }
        }
        isCoinSpawned = true;

    }

    public void SpawnPower()
    {
        magnet = Instantiate(magnetPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0), transform.parent);
        healthPack = Instantiate(healthPackPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0), transform.parent);
        shield = Instantiate(shieldPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0), transform.parent);
        gun = Instantiate(gunPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0), transform.parent);
    }

    public void RespawnPower()
    {
        float[] posForZ = { -1.2f, 0f, 1.2f };
        float[] posForX = { 10f, -16.5f };
        int randomIndexZ = Random.Range(0, 3);
        int randomIndexX = Random.Range(0, 2);
        float randomPosZ = posForZ[randomIndexZ];
        float randomPosX = posForX[randomIndexX];
        float chance = Random.value;

        // Rest of chance fo nothing --> 20%
        magnet.SetActive(false);
        healthPack.SetActive(false);
        shield.SetActive(false);
        gun.SetActive(false);

        // 20% chance for magnet
        if (chance <= 0.2f)
        {
            magnet.SetActive(true);
            magnet.transform.localPosition = new Vector3(randomPosX, RCSGPosY, randomPosZ);
        }
        // 20% chance for health pack
        else if(chance > 0.2f && chance <= 0.4f)
        {
            healthPack.SetActive(true);
            healthPack.transform.localPosition = new Vector3(randomPosX, RCSGPosY, randomPosZ);
        }
        // 20% chance for shield
        else if(chance > 0.4f && chance <= 0.6f)
        {
            shield.SetActive(true);
            shield.transform.localPosition = new Vector3(randomPosX, RCSGPosY, randomPosZ);
        }
        // 20% chance for gun
        else if (chance > 0.6f && chance <= 0.8f)
        {
            gun.SetActive(true);
            gun.transform.localPosition = new Vector3(randomPosX, RCSGPosY, randomPosZ);
        }
    }
}
