using System.Collections;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    private const float GunDuration = 5f;
    private const float ShieldDuration = 5f;
    private const float MagnetDuration = 6f;


    public void ActivateMagnet(PlayerManager playerManager)
    {
        if (playerManager.IsMagnetActive)
        {
            StopCoroutine(MagnetCountDown(playerManager)); // CR: Je l' ovo radi kako ocekujes? Nece da se desi da player 2 skupi svoj drugi magnet i prekine prvom igracu njegov magnet power up?
        }

        StartCoroutine(MagnetCountDown(playerManager));
    }

    private IEnumerator MagnetCountDown(PlayerManager playerManager)
    {
        playerManager.IsMagnetActive = true;
        playerManager.CollectibleCollider.enabled = true;
        yield return new WaitForSeconds(MagnetDuration);
        playerManager.CollectibleCollider.enabled = false;
        playerManager.IsMagnetActive = false;
    }

    public void ActivateGun(PlayerManager playerManager)
    {
        if (playerManager.IsGunActive)
        {
            StopCoroutine(GunCountDown(playerManager));
        }

        StartCoroutine(GunCountDown(playerManager));
    }

    private IEnumerator GunCountDown(PlayerManager playerManager)
    {
        playerManager.IsGunActive = true;
        yield return new WaitForSeconds(GunDuration);
        playerManager.IsGunActive = false;
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
