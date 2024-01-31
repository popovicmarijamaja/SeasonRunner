using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private const int NumberOfCoin = 11; // CR: Broj novcica po cemu? Je l' ovo predstavlja maksimalni broj novcica na ekranu? Maksimalni broj novcica po chunk-u? Ili je broj novcica po chunk-u fiksno 11? Koji god da je slucaj, ime treba nedvosmisleno da ilustruje to.
    private const int NumberOfObstacles = 11; // CR: Ista stvar.
	// CR: Ovo podesavanje pozicija ti je hak. Nije velika stvar, ali definitivno na neki nacin zaobilazis problem. Bilo bi bolje da u samim assetima nekako resis ovo neslaganje po Y i ucinis ih konzistentnim.
    private const float RCSGPosY = 0.5f; //for rock, coin, shield, star and gun
    private const float WEMPosY = 0f; //for wooden obstacle, enemy and mushrooms
    private const float DistanceBetweenObstacles = -7.7f;
    private const float DistanceBetweenCoin = -7.7f;
    private const float FirstDistanceObstacle = -51.5f;
    private const float FirstDistanceCoin = -47.8f;

    private readonly List<GameObject> ObjectsInEnvironment = new();

    private void SpawnObstacle()
    {
        float posX;
        float randomPosZ;
        int randomIndex;
        float chance;
        float prevZ = 0f;
        float neutralPosZ = 0f;
		// CR: I ovde bi bilo dobro da se ove nekonzistentnosti u pozicijama rese na nivou samih asseta.
		// Takodje, u pitanju su z-pozicije, sto treba da se odrazi u nazivu ova dva polja
        float[] pos = { -0.5f, 0.5f, 1.6f };
        float[] posME = { -1, 0, 1 }; // Pos for mushrooms and shooting enemy
        

        for (int i = 0; i < NumberOfObstacles; i++)
        {
            posX = FirstDistanceObstacle + (i * DistanceBetweenObstacles);
            chance = Random.value;
            randomIndex = Random.Range(0, 3);
            randomPosZ = pos[randomIndex];

			// CR: Ovu sansu mozemo elegantnije da resimo
            // 50% chance for 1 rock in line
            if (chance <= 0.5f)
            {
                SpawnObject(ObjectPool.Instance.GetRock(), posX, RCSGPosY, randomPosZ, Quaternion.Euler(-90, 0, 0));
            }
            // 20% chance for 2 rocks in line
            else if (chance > 0.5f && chance <= 0.7f)
            {
                // Making sure that second rock does not have the same z position as first
                (float newFirstPos, float newSecondPos) = MakeDiferentPos(pos, prevZ, randomPosZ);

                SpawnObject(ObjectPool.Instance.GetRock(), posX, RCSGPosY, newFirstPos, Quaternion.Euler(-90, 0, 0));
                SpawnObject(ObjectPool.Instance.GetRock(), posX, RCSGPosY, newSecondPos, Quaternion.Euler(-90, 0, 0));
            }
            // 10% chance for wooden obsticle
            else if (chance > 0.7f && chance <= 0.8f)
            {
                SpawnObject(ObjectPool.Instance.GetWoodenObstacle(), posX, WEMPosY, neutralPosZ, Quaternion.Euler(0, 0, 0));

                // Rest of chance for empty wooden obsticle => 2%
                chance = Random.value;
                // 4% chance for star
                if (chance <= 0.4f)
                {
                    SpawnObject(ObjectPool.Instance.GetStar(), posX, RCSGPosY, neutralPosZ, Quaternion.Euler(0, 0, 0));
                }
                // 4% chance for mushrooms
                else if (chance > 0.4f && chance <= 0.8)
                {
                    SpawnObject(ObjectPool.Instance.GetMushrooms(), posX, WEMPosY, neutralPosZ, Quaternion.Euler(0, 0, 0));
                }
            }
            // 10% chance for mushrooms
            else if (chance > 0.8f && chance <= 0.9f)
            {
                randomPosZ = posME[randomIndex];
                SpawnObject(ObjectPool.Instance.GetMushrooms(), posX, WEMPosY, randomPosZ, Quaternion.Euler(0, 0, 0));
            }
			// CR: Ovo je dobro mesto za jos jedan nivo logike
            // 5% chance for enemy
            else if (chance > 0.9f && chance <= 0.95f)
            {
                SpawnObject(ObjectPool.Instance.GetEnemy(), posX, WEMPosY, neutralPosZ, Quaternion.Euler(0, 0, 0));
            }
            // 5% chance for shooting enemy
            else if (chance > 0.95f)
            {
                randomPosZ = posME[randomIndex];
                SpawnObject(ObjectPool.Instance.GetShootingEnemy(), posX, WEMPosY, randomPosZ, Quaternion.Euler(0, 90, 0));
            }
        }
    }

    private void SpawnCoin() // CR: Ako funkcija spawnuje vise od jednog novcica, treba da se zove SpawnCoins
    {
        float randomPosZ;
        float posX;
        int randomIndex;
        float[] pos = { -1f, 0f, 1f };

        for (int i = 0; i < NumberOfCoin; i++)
        {
            randomIndex = Random.Range(0, 3);
            randomPosZ = pos[randomIndex];
            posX = FirstDistanceCoin + (i * DistanceBetweenCoin);

            SpawnObject(ObjectPool.Instance.GetCoin(), posX, RCSGPosY, randomPosZ, Quaternion.Euler(-90, 0, 0));
        }
    }

    private void SpawnPower() // CR: SpawnPowerUp bi bilo bolje, posto tu terminologiju koristimo
    {
        float[] posForZ = { -1.2f, 0f, 1.2f };
        float[] posForX = { -49f, -57f };
        int randomIndexZ = Random.Range(0, 3);
        int randomIndexX = Random.Range(0, 2);
        float randomPosZ = posForZ[randomIndexZ];
        float randomPosX = posForX[randomIndexX];
        float chance = Random.value;

		// CR: I ovde sanse mozemo elegantnije
        // Rest of chance fo nothing --> 20%

        // 20% chance for magnet
        if (chance <= 0.2f)
        {
            SpawnObject(ObjectPool.Instance.GetMagnet(), randomPosX, RCSGPosY, randomPosZ, Quaternion.Euler(0, 0, 0));
        }
        // 20% chance for health pack
        else if (chance > 0.2f && chance <= 0.4f)
        {
            SpawnObject(ObjectPool.Instance.GetHealthPack(), randomPosX, RCSGPosY, randomPosZ, Quaternion.Euler(0, 0, 0));
        }
        // 20% chance for shield
        else if (chance > 0.4f && chance <= 0.6f)
        {
            SpawnObject(ObjectPool.Instance.GetShield(), randomPosX, RCSGPosY, randomPosZ, Quaternion.Euler(0, 0, 0));
        }
        // 20% chance for gun
        else if (chance > 0.6f && chance <= 0.8f)
        {
            SpawnObject(ObjectPool.Instance.GetGun(), randomPosX, RCSGPosY, randomPosZ, Quaternion.Euler(0, 0, 0));
        }
    }

    private void SpawnObject(GameObject obj, float posX, float posY, float posZ, Quaternion rotation)
    {
        if (obj != null)
        {
            obj.SetActive(true);
            obj.transform.parent = transform.parent;
            obj.transform.SetPositionAndRotation(new Vector3(posX, posY, posZ), rotation);
            ObjectsInEnvironment.Add(obj);
        }
    }

    private (float,float) MakeDiferentPos(float[] pos, float firstPos, float secondPos)
    {
        firstPos = secondPos; // CR: Ovo nicemu ne sluzi, posto imas do-while a ne while. Takodje, firstPos ne treba da bude parametar nego lokalna promenljiva
        do
        {
            int randomIndex = Random.Range(0, 3);
            firstPos = pos[randomIndex];
        } while (secondPos == firstPos);
        return (firstPos, secondPos);
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
        SpawnCoin();
        SpawnPower();
    }
}
