using System.Collections;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    private const float GunDuration = 5f;
    private const float ShieldDuration = 5f;
    private const float MagnetDuration = 6f;


    public void ActivateMagnet(PlayerManager playerManager)
    {
        if (playerManager.isMagnetActive)
        {
            StopCoroutine(MagnetCountDown(playerManager));
        }

        StartCoroutine(MagnetCountDown(playerManager));
    }

    private IEnumerator MagnetCountDown(PlayerManager playerManager)
    {
        playerManager.isMagnetActive = true;
        playerManager.CollectibleCollider.enabled = true;
        yield return new WaitForSeconds(MagnetDuration);
        playerManager.CollectibleCollider.enabled = false;
        playerManager.isMagnetActive = false;
    }

    public void ActivateGun(PlayerManager playerManager)
    {
        if (playerManager.isGunActive)
        {
            StopCoroutine(GunCountDown(playerManager));
        }

        StartCoroutine(GunCountDown(playerManager));
    }

    private IEnumerator GunCountDown(PlayerManager playerManager)
    {
        playerManager.isGunActive = true;
        yield return new WaitForSeconds(GunDuration);
        playerManager.isGunActive = false;
    }

    public void ActivateShield(PlayerManager playerManager)
    {
        if (playerManager.IsShieldActive)
        {
            StopCoroutine(ShieldCountDown(playerManager));
        }

        StartCoroutine(ShieldCountDown(playerManager));
    }

    private IEnumerator ShieldCountDown(PlayerManager playerManager)
    {
        playerManager.IsShieldActive = true;
        yield return new WaitForSeconds(ShieldDuration);
        playerManager.IsShieldActive = false;
    }
}
