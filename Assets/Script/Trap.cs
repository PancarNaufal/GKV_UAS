using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] private int _damageAmount = 1; // Jumlah damage yang diberikan trap
    [SerializeField] private Vector2 _knockbackForce = new Vector2(5f, 5f); // Kekuatan knockback saat player terkena trap

    // Pastikan collider pada GameObject trap adalah Trigger (isTrigger = true)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cek apakah yang masuk adalah Player
        if (collision.CompareTag("Player"))
        {
            // Coba dapatkan komponen PlayerHealthAndCombat dari player
            PlayerHealthAndCombat playerHealth = collision.GetComponent<PlayerHealthAndCombat>();

            if (playerHealth != null)
            {
                // Hitung arah knockback (dari trap ke player)
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                
                // Panggil metode TakeDamage pada player
                playerHealth.TakeDamage(_damageAmount, knockbackDirection * _knockbackForce.x); // Menggunakan x untuk kekuatan horizontal

                // Anda bisa menambahkan SFX trap di sini
                // Contoh: FindObjectOfType<AudioManager>()?.PlaySFX(FindObjectOfType<AudioManager>().trapHitSFX);
            }
        }
    }

    // Fungsi ini tidak wajib, hanya untuk visualisasi di Editor
    private void OnDrawGizmos()
    {
        // Gambar kotak hijau di posisi trap untuk debugging
        Gizmos.color = Color.red;
        if (GetComponent<Collider2D>() is BoxCollider2D boxCollider)
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
        }
        else if (GetComponent<Collider2D>() is CircleCollider2D circleCollider)
        {
            Gizmos.DrawWireSphere(transform.position + (Vector3)circleCollider.offset, circleCollider.radius);
        }
    }
}