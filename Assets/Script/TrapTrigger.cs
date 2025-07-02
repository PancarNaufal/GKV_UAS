using UnityEngine;
using System.Collections;

public class TrapTriggerArea : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Masukkan objek Spike Trap yang ingin diaktifkan")]
    [SerializeField] private Trap2 _spikeTrapToActivate;

    [Tooltip("Setelah aktif, berapa lama jeda sebelum trigger ini bisa digunakan lagi")]
    [SerializeField] private float _cooldown = 3f;

    private Collider2D _triggerCollider;
    private bool _isReady = true;

    private void Awake()
    {
        _triggerCollider = GetComponent<Collider2D>();
        if (_triggerCollider == null)
        {
            Debug.LogError("Tidak ada Collider2D pada Trigger Area!", this.gameObject);
            return;
        }

        // Pastikan collider ini adalah trigger
        _triggerCollider.isTrigger = true;

        if (_spikeTrapToActivate == null)
        {
            Debug.LogError("Spike Trap belum di-set pada Trigger Area!", this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Hanya aktif jika trigger siap dan yang masuk adalah player
        if (_isReady && collision.CompareTag("Player"))
        {
            // Panggil fungsi untuk mengaktifkan trap DAN kirim informasi transform trigger ini
            _spikeTrapToActivate.ActivateTrap(transform); // <-- UBAH BARIS INI

            // Mulai proses cooldown
            StartCoroutine(StartCooldown());
        }
    }

    private IEnumerator StartCooldown()
    {
        // Matikan trigger sementara agar tidak bisa diaktifkan berulang kali
        _isReady = false;

        // Tunggu sesuai durasi cooldown
        yield return new WaitForSeconds(_cooldown);

        // Siapkan kembali trigger untuk digunakan lagi
        _isReady = true;
    }

    // Visualisasi di editor agar mudah dilihat
    private void OnDrawGizmos()
    {
        if (GetComponent<Collider2D>() is BoxCollider2D boxCollider)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f); // Warna hijau transparan
            Gizmos.DrawCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
        }
    }
}