using UnityEngine;

public class DynamicPlatform : MonoBehaviour
{
    public Collider2D solidCollider; // Collider yang aktif saat player di atas
    private bool playerOnTop = false;

    void Update()
    {
        solidCollider.enabled = playerOnTop;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Pastikan pemain berada di atas platform
            if (other.transform.position.y > transform.position.y + 0.1f)
            {
                playerOnTop = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnTop = false;
        }
    }
}
