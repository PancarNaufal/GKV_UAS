using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform destinationPortal; // Portal tujuan
    public float teleportCooldown = 1f;

    private bool canTeleport = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canTeleport)
        {
            StartCoroutine(Teleport(other));
        }
    }

    private System.Collections.IEnumerator Teleport(Collider2D player)
    {
        // Teleport player ke portal tujuan
        player.transform.position = destinationPortal.position;

        // Nonaktifkan sementara portal tujuan agar tidak langsung teleport balik
        Portal destPortalScript = destinationPortal.GetComponent<Portal>();
        if (destPortalScript != null)
        {
            destPortalScript.canTeleport = false;
        }

        // Cooldown di portal ini juga
        canTeleport = false;

        yield return new WaitForSeconds(teleportCooldown);

        // Aktifkan teleport kembali
        if (destPortalScript != null)
        {
            destPortalScript.canTeleport = true;
        }
        canTeleport = true;
    }
}
