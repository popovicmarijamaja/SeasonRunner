using System.Collections;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    private const float GunDuration = 5f;
    private const float ShieldDuration = 5f;
    private const float MagnetDuration = 6f;

    private PlayerManager playerManager;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    public void ActivateMagnet()
    {
        if (playerManager.isMagnetActive)
        {
            StopCoroutine(MagnetCountDown());
        }

        StartCoroutine(MagnetCountDown());
    }

    private IEnumerator MagnetCountDown()
    {
        playerManager.isMagnetActive = true;
        playerManager.CollectibleCollider.enabled = true;
        yield return new WaitForSeconds(MagnetDuration);
        playerManager.CollectibleCollider.enabled = false;
        playerManager.isMagnetActive = false;
    }

    public void ActivateGun()
    {
        if (playerManager.isGunActive)
        {
            StopCoroutine(GunCountDown());
        }

        StartCoroutine(GunCountDown());
    }

    private IEnumerator GunCountDown()
    {
        playerManager.isGunActive = true;
        yield return new WaitForSeconds(GunDuration);
        playerManager.isGunActive = false;
    }

    public void ActivateShield()
    {
        if (playerManager.IsShieldActive)
        {
            StopCoroutine(ShieldCountDown());
        }

        StartCoroutine(ShieldCountDown());
    }

    private IEnumerator ShieldCountDown()
    {
        playerManager.IsShieldActive = true;
        yield return new WaitForSeconds(ShieldDuration);
        playerManager.IsShieldActive = false;
    }
}
