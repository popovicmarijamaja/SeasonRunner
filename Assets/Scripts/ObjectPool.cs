using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
	public static ObjectPool Instance;

    private const int PoolSizeEnvironment = 3;
    private const int PoolSizeFireParticle = 10;

    [SerializeField] private GameObject environment;
    [SerializeField] private GameObject fireParticle;
    
    private readonly List<GameObject> pooledObjectsFire = new();
    public List<GameObject> pooledObjectsEnvironment = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < PoolSizeEnvironment; i++)
        {
            GameObject obj = Instantiate(environment, transform.parent);
            obj.SetActive(false);
            pooledObjectsEnvironment.Add(obj);
        }

        for (int i = 0; i < PoolSizeFireParticle; i++)
        {
            GameObject obj = Instantiate(fireParticle, transform.parent);
            obj.SetActive(false);
            pooledObjectsFire.Add(obj);
        }
    }
    public GameObject GetPooledObjectEnvironment()
    {
        for (int i = 0; i < pooledObjectsEnvironment.Count; i++)
        {
            if (!pooledObjectsEnvironment[i].activeInHierarchy)
            {
                return pooledObjectsEnvironment[i];
            }
        }
        return null;
    }
    public GameObject GetPooledObjectFire()
    {
        foreach (GameObject obj in pooledObjectsFire)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }
}

