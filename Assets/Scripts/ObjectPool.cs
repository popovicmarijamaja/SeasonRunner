using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
	public static ObjectPool Instance;

    private const int PoolSizeEnvironment = 3; // CR: U imenu treba zadnja rec da predstavlja ono sto objekat jeste, a prethodne reci ga blize opisuju. Ovako zvuci kao da ovo predstavlja environment, a u stvari predstavlja size. EnvironmentPoolSize je bolje ime.
    private const int PoolSizeFireParticle = 10; // CR: FireballPoolSize, FireProjectilePoolSize ili slicno

    [SerializeField] private GameObject environment;
    [SerializeField] private GameObject fireParticle; // CR: Koju god nimenklaturu usvojis umesto particle za ovo, stavi je i ovde, npr. fireball.
    
    private readonly List<GameObject> pooledObjectsFire = new(); // CR: Opet, zadnja rec treba da opisuje objekat. fireballPool.
    public List<GameObject> pooledObjectsEnvironment = new(); // CR: environmentPool

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
		// CR: Fali ti else sa Destroy ovde, kao sto imas u game manageru.
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
    public GameObject GetPooledObjectEnvironment() // CR: Posto se objekat vec zove ObjectPool, reci PooledObject su ti ovde suvisne jer se podrazumevaju. GetEnvironment ili GetEnvironmentChunk je ovde dovoljno.
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
    public GameObject GetPooledObjectFire() // CR: Po istoj logici, GetFireball je dovoljno.
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

