using UnityEngine;
using System.Collections; // Diperlukan untuk menggunakan Coroutine

public class SpikeTrapMove : MonoBehaviour
{
    [Header("Trap Damage Settings")]
    [SerializeField] private int _damageAmount = 1; // Jumlah damage yang diberikan trap
    [SerializeField] private Vector2 _knockbackForce = new Vector2(5f, 5f); // Kekuatan knockback saat player terkena trap

    [Header("Movement Settings")]
    [SerializeField] private Transform _targetPoint; // Titik tujuan pergerakan trap
    [SerializeField] private float _moveSpeed = 3f; // Kecepatan pergerakan trap
    [SerializeField] private float _activeTime = 1f; // Berapa lama trap diam di posisi target
    [SerializeField] private float _inactiveDelay = 2f; // Berapa lama trap diam di posisi awal sebelum bergerak

    private Vector3 _startPoint; // Posisi awal trap
    private bool _isMoving = false;

    private void Start()
    {
        // Simpan posisi awal trap
        _startPoint = transform.position;

        // Pastikan target point sudah di-set untuk menghindari error
        if (_targetPoint != null)
        {
            // Mulai siklus pergerakan trap
            StartCoroutine(MoveCycle());
        }
        else
        {
            Debug.LogError("Target Point untuk Spike Trap belum di-set!", this.gameObject);
        }
    }

    private IEnumerator MoveCycle()
    {
        // Loop ini akan berjalan selamanya, membuat trap terus bergerak dalam siklus
        while (true)
        {
            // 1. Tunggu di posisi awal (inactive)
            yield return new WaitForSeconds(_inactiveDelay);

            // 2. Bergerak ke posisi target (aktif)
            yield return StartCoroutine(MoveToPosition(_targetPoint.position));

            // 3. Tunggu di posisi target (aktif)
            yield return new WaitForSeconds(_activeTime);

            // 4. Bergerak kembali ke posisi awal
            yield return StartCoroutine(MoveToPosition(_startPoint));
        }
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        // Selama posisi trap belum mencapai target
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            // Gerakkan trap menuju target dengan kecepatan _moveSpeed
            transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed * Time.deltaTime);
            // Tunggu frame berikutnya sebelum melanjutkan loop
            yield return null;
        }
        // Pastikan posisi trap sama persis dengan target di akhir pergerakan
        transform.position = target;
    }


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

                // Jika knockbackDirection adalah (0,0) (misal player tepat di tengah), berikan arah default ke atas
                if (knockbackDirection == Vector2.zero)
                {
                    knockbackDirection = Vector2.up;
                }
                
                // Panggil metode TakeDamage pada player
                playerHealth.TakeDamage(_damageAmount, knockbackDirection * _knockbackForce);

                // Anda bisa menambahkan SFX trap di sini
                // Contoh: FindObjectOfType<AudioManager>()?.PlaySFX(FindObjectOfType<AudioManager>().trapHitSFX);
            }
        }
    }

    // Fungsi ini tidak wajib, hanya untuk visualisasi di Editor
    private void OnDrawGizmos()
    {
        // Gambar collider trap
        Gizmos.color = Color.red;
        if (GetComponent<Collider2D>() is BoxCollider2D boxCollider)
        {
            // Jika ada pergerakan, gambar wireframe di posisi awal juga
            if (_targetPoint != null && Application.isPlaying)
                Gizmos.DrawWireCube(_startPoint + (Vector3)boxCollider.offset, boxCollider.size);
            
            // Gambar di posisi sekarang
            Gizmos.DrawWireCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
        }
        else if (GetComponent<Collider2D>() is CircleCollider2D circleCollider)
        {
             // Jika ada pergerakan, gambar wireframe di posisi awal juga
            if (_targetPoint != null && Application.isPlaying)
                 Gizmos.DrawWireSphere(_startPoint + (Vector3)circleCollider.offset, circleCollider.radius);
            
            // Gambar di posisi sekarang
            Gizmos.DrawWireSphere(transform.position + (Vector3)circleCollider.offset, circleCollider.radius);
        }

        // Gambar garis jalur pergerakan jika targetPoint sudah di-set
        if (_targetPoint != null)
        {
            Gizmos.color = Color.cyan;
            // Gunakan posisi awal yang disimpan jika aplikasi berjalan, atau posisi transform saat ini jika di editor
            Vector3 startPos = Application.isPlaying ? _startPoint : transform.position;
            Gizmos.DrawLine(startPos, _targetPoint.position);
            Gizmos.DrawWireSphere(_targetPoint.position, 0.2f); // Tandai titik target
        }
    }
}