using UnityEngine;
using System.Collections;

// Pastikan nama class ini sama dengan nama file Anda, yaitu 'Trap2'
public class Trap2 : MonoBehaviour 
{
    [Header("Trap Damage Settings")]
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] private Vector2 _knockbackForce = new Vector2(5f, 5f);

    [Header("Movement Settings")]
    [SerializeField] private Transform _targetPoint;
    [SerializeField] private float _moveSpeed = 3f;
    [Tooltip("Berapa lama trap diam di posisi target sebelum menghilang")]
    [SerializeField] private float _activeTime = 0.5f;

    private Vector3 _startPoint;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;

    private void Awake()
    {
        _startPoint = transform.position;
        // Ambil komponen saat awal
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();

        // Pastikan komponen ada untuk menghindari error
        if (_spriteRenderer == null || _collider == null)
        {
            Debug.LogError("SpriteRenderer atau Collider2D tidak ditemukan di trap!", this.gameObject);
            return;
        }

        // Sembunyikan saat awal
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
    }

    public void ActivateTrap(Transform triggerTransform)
    {
        // --- BAGIAN YANG HILANG & SEKARANG DIPERBAIKI ---
        // 1. Hitung arah dari titik awal trap ke posisi trigger
        Vector2 directionToTrigger = triggerTransform.position - _startPoint;
        
        // 2. Ubah arah tersebut menjadi sudut rotasi (dalam derajat)
        float angle = Mathf.Atan2(directionToTrigger.y, directionToTrigger.x) * Mathf.Rad2Deg;
        // --- AKHIR PERBAIKAN ---

        // Terapkan rotasi
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        // Posisikan trap & aktifkan komponennya
        transform.position = _startPoint;
        _spriteRenderer.enabled = true;
        _collider.enabled = true;

        StartCoroutine(MoveAndDisappear());
    }

    private IEnumerator MoveAndDisappear()
    {
        yield return StartCoroutine(MoveToPosition(_targetPoint.position));
        yield return new WaitForSeconds(_activeTime);
        
        // Sembunyikan lagi sprite dan collider
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
    }
    
    private IEnumerator MoveToPosition(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Coba dapatkan komponen PlayerHealthAndCombat dari player
            // Ganti 'PlayerHealthAndCombat' jika nama skrip player Anda berbeda
            PlayerHealthAndCombat playerHealth = collision.GetComponent<PlayerHealthAndCombat>();
            if (playerHealth != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                if (knockbackDirection == Vector2.zero)
                {
                    knockbackDirection = Vector2.up;
                }
                playerHealth.TakeDamage(_damageAmount, knockbackDirection * _knockbackForce);
            }
        }
    }
}