using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
	public static ObjectPool Instance;

    private const int EnvironmentPoolSize = 3;
    private const int FireballPoolSize = 10;
    private const int BulletPoolSize = 20;
    private const int CoinPoolSize = 11;
    private const int ObstaclesPoolSize = 11;
    private const int NumberOfRockInLine = 2;

    [SerializeField] private GameObject environment;
    [SerializeField] private GameObject fireball;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject rock;
    [SerializeField] private GameObject woodenObstacle;
    [SerializeField] private GameObject mushrooms;
    [SerializeField] private GameObject star;
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject magnet;
    [SerializeField] private GameObject healthPack;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject shootingEnemy;

    public List<GameObject> EnvirontmentPool = new();
    private readonly List<GameObject> fireballPool = new();
    private readonly List<GameObject> bulletPool = new();
    private readonly List<GameObject> rockPool = new();
    private readonly List<GameObject> woodenObstaclePool = new();
    private readonly List<GameObject> mushroomsPool = new();
    private readonly List<GameObject> starPool = new();
    private readonly List<GameObject> coinPool = new();
    private readonly List<GameObject> enemyPool = new();
    private readonly List<GameObject> shootingEnemyPool = new();
    private readonly List<GameObject> magnetPool = new();
    private readonly List<GameObject> healthPackPool = new();
    private readonly List<GameObject> shieldPool = new();
    private readonly List<GameObject> gunPool = new();

    public bool isInitialized;

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

        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < EnvironmentPoolSize; i++)
        {
            InstantiateGameObject(environment, EnvirontmentPool);
            for (int j = 0; j < ObstaclesPoolSize; j++)
            {
                for (int e = 0; e < NumberOfRockInLine; e++)
                {
                    InstantiateGameObject(rock, rockPool);
                }
                InstantiateGameObject(woodenObstacle, woodenObstaclePool);
                InstantiateGameObject(mushrooms, mushroomsPool);
                InstantiateGameObject(star, starPool);
                InstantiateGameObject(enemy, enemyPool);
                InstantiateGameObject(shootingEnemy, shootingEnemyPool);
            }
            for (int f = 0; f < CoinPoolSize; f++)
            {
                InstantiateGameObject(coin, coinPool);
            }
            InstantiateGameObject(magnet, magnetPool);
            InstantiateGameObject(healthPack, healthPackPool);
            InstantiateGameObject(shield, shieldPool);
            InstantiateGameObject(gun, gunPool);
        }

        for (int i = 0; i < FireballPoolSize; i++)
        {
            InstantiateGameObject(fireball, fireballPool);
        }

        for (int i = 0; i < BulletPoolSize; i++)
        {
            InstantiateGameObject(bullet, bulletPool);
        }

        isInitialized = true;
    }

    private void InstantiateGameObject(GameObject prefab, List<GameObject> list)
    {
        GameObject obj = Instantiate(prefab, transform.parent);
        obj.SetActive(false);
        list.Add(obj);
    }

    public GameObject GetEnvironment()
    {
        for (int i = 0; i < EnvirontmentPool.Count; i++)
        {
            if (!EnvirontmentPool[i].activeInHierarchy)
            {
                return EnvirontmentPool[i];
            }
        }
        return null;
    }

    public GameObject GetFireball()
    {
        foreach (GameObject obj in fireballPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }

    public GameObject GetBullet()
    {
        foreach (GameObject obj in bulletPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }

    public GameObject GetRock()
    {
        foreach (GameObject obj in rockPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }

    public GameObject GetWoodenObstacle()
    {
        foreach (GameObject obj in woodenObstaclePool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }

    public GameObject GetMushrooms()
    {
        foreach (GameObject obj in mushroomsPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }

    public GameObject GetStar()
    {
        foreach (GameObject obj in starPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }

    public GameObject GetCoin()
    {
        foreach (GameObject obj in coinPool)
        {
            if (!obj.activeInHierarchy && obj.transform.parent == transform.parent)
            {
                return obj;
            }
        }
        return null;
    }

    public GameObject GetEnemy()
    {
        foreach (GameObject obj in enemyPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }

    public GameObject GetShootingEnemy()
    {
        foreach (GameObject obj in shootingEnemyPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }

    public GameObject GetMagnet()
    {
        foreach (GameObject obj in magnetPool)
        {
            if (!obj.activeInHierarchy && obj.transform.parent == transform.parent)
            {
                return obj;
            }
        }
        return null;
    }

    public GameObject GetHealthPack()
    {
        foreach (GameObject obj in healthPackPool)
        {
            if (!obj.activeInHierarchy && obj.transform.parent == transform.parent)
            {
                return obj;
            }
        }
        return null;
    }

    public GameObject GetShield()
    {
        foreach (GameObject obj in shieldPool)
        {
            if (!obj.activeInHierarchy && obj.transform.parent == transform.parent)
            {
                return obj;
            }
        }
        return null;
    }

    public GameObject GetGun()
    {
        foreach (GameObject obj in gunPool)
        {
            if (!obj.activeInHierarchy && obj.transform.parent == transform.parent)
            {
                return obj;
            }
        }
        return null;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.transform.parent = transform.parent;
        obj.SetActive(false);
    }
}

