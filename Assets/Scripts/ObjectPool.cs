using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
	public static ObjectPool Instance;

    private const int EnvironmentPoolSize = 6;
    private const int FireballPoolSize = 30;
    private const int BulletPoolSize = 40;
    private const int CoinPoolSize = 22;
    private const int ObstaclesPoolSize = 22;
    private const int NumberOfRockInLine = 2;

    [SerializeField] private GameObject environmentPrefab;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private GameObject woodenObstaclePrefab;
    [SerializeField] private GameObject mushroomsPrefab;
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject magnetPrefab;
    [SerializeField] private GameObject healthPackPrefab;
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private GameObject soldierPrefab;
    [SerializeField] private GameObject gunPrefab;

    public List<GameObject> EnvirontmentPool = new();
    private readonly List<GameObject> fireballPool = new();
    private readonly List<GameObject> bulletPool = new();
    private readonly List<GameObject> rockPool = new();
    private readonly List<GameObject> woodenObstaclePool = new();
    private readonly List<GameObject> mushroomsPool = new();
    private readonly List<GameObject> starPool = new();
    private readonly List<GameObject> coinPool = new();
    private readonly List<GameObject> soldierPool = new();
    private readonly List<GameObject> magnetPool = new();
    private readonly List<GameObject> healthPackPool = new();
    private readonly List<GameObject> shieldPool = new();
    private readonly List<GameObject> gunPool = new();

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
            InstantiateGameObject(environmentPrefab, EnvirontmentPool);
            for (int j = 0; j < ObstaclesPoolSize; j++)
            {
                for (int e = 0; e < NumberOfRockInLine; e++)
                {
                    InstantiateGameObject(rockPrefab, rockPool);
                }
                InstantiateGameObject(woodenObstaclePrefab, woodenObstaclePool);
                InstantiateGameObject(mushroomsPrefab, mushroomsPool);
                InstantiateGameObject(starPrefab, starPool);
                InstantiateGameObject(soldierPrefab, soldierPool);
            }
            for (int f = 0; f < CoinPoolSize; f++)
            {
                InstantiateGameObject(coinPrefab, coinPool);
            }
            InstantiateGameObject(magnetPrefab, magnetPool);
            InstantiateGameObject(healthPackPrefab, healthPackPool);
            InstantiateGameObject(shieldPrefab, shieldPool);
            InstantiateGameObject(gunPrefab, gunPool);
        }

        for (int i = 0; i < FireballPoolSize; i++)
        {
            InstantiateGameObject(fireballPrefab, fireballPool);
        }

        for (int i = 0; i < BulletPoolSize; i++)
        {
            InstantiateGameObject(bulletPrefab, bulletPool);
        }
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
            if (!obj.activeInHierarchy && obj.transform.parent == transform.parent)
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

    public GameObject GetSoldier()
    {
        foreach (GameObject obj in soldierPool)
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

