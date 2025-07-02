using UnityEngine;
using System.Collections;

public class PlayerHealthAndCombat : MonoBehaviour
{
    public PlayerDataWithDash Data; 

    public Rigidbody2D RB { get; private set; }
    AudioManager audioManager; // Dapatkan referensi ke AudioManager

    private Collider2D[] _pogoHitResults = new Collider2D[10]; // Sesuaikan ukuran array
    private ContactFilter2D _pogoContactFilter; // Deklarasikan ContactFilter2D

    // Variabel health dihilangkan karena sudah one-hit kill

    [Header("Pogo Attack")]
    [SerializeField] private Transform _pogoAttackPoint; 
    [SerializeField] private Vector2 _pogoAttackRange = new Vector2(0.5f, 0.2f); 
    [SerializeField] private LayerMask _hittableLayer; 
    [SerializeField] private float _pogoJumpForce = 15f; 
    [SerializeField] private float _pogoAttackCooldown = 0.2f; 
    private float _lastPogoAttackTime;

    [Header("Damage & Death")]
    [SerializeField] private float _hurtKnockbackForce = 3f;
    [SerializeField] private float _invincibilityDuration = 1f; 
    private bool _isInvincible = false; 
    
    [Header("Respawn")]
    [SerializeField] private Transform _spawnPoint; 
    [SerializeField] private float _deathRespawnDelay = 2f;

    private PlayerMovement _playerMovement;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        _playerMovement = GetComponent<PlayerMovement>();

        _pogoContactFilter = new ContactFilter2D();
        _pogoContactFilter.SetLayerMask(_hittableLayer); // Atur LayerMask dari variabel Anda
        _pogoContactFilter.useTriggers = true;
    }

    private void Start()
    {
        if (_spawnPoint == null)
        {
            _spawnPoint = new GameObject("InitialSpawnPoint").transform;
            _spawnPoint.position = transform.position;
            _spawnPoint.SetParent(transform.parent);
        }
        _isInvincible = false; // Pastikan tidak kebal di awal game
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.Mouse0)) && RB.linearVelocity.y < 0)
        {
            PerformPogoAttack();
        }
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (_isInvincible) return;

        // Gunakan playerHurt jika ada di AudioManager
        // Jika tidak, Anda bisa tambahkan atau gunakan SFX umum
        if (audioManager.playerHurt != null) // Cek apakah playerHurt ada
            audioManager.PlaySFX(audioManager.playerHurt); 
        else
            Debug.LogWarning("Player Hurt SFX is not assigned in AudioManager!");


        RB.AddForce(knockbackDirection.normalized * _hurtKnockbackForce, ForceMode2D.Impulse);

        Die(); 
    }

    private void PerformPogoAttack()
    {
        if (Time.time >= _lastPogoAttackTime + _pogoAttackCooldown)
        {
            _lastPogoAttackTime = Time.time;
            
            // Gunakan Physics2D.OverlapBox yang direkomendasikan dengan ContactFilter2D
            // Ini akan mengembalikan jumlah collider yang terdeteksi dan mengisi _pogoHitResults
            int numHit = Physics2D.OverlapBox(_pogoAttackPoint.position, _pogoAttackRange, 0, _pogoContactFilter, _pogoHitResults);

            for (int i = 0; i < numHit; i++)
            {
                Collider2D obj = _pogoHitResults[i];

                if (obj == null) continue; 

                Debug.Log($"Pogo attack hit {obj.name}!");

                // Beri damage ke objek jika memungkinkan (contoh: musuh)
                // Jika musuh memiliki script yang bisa menerima damage, panggil metodenya
                // Contoh: obj.GetComponent<EnemyHealth>()?.TakeDamage(_pogoDamage); // Gunakan _pogoDamage di sini!

                RB.linearVelocity = new Vector2(RB.linearVelocity.x, 0); 
                RB.AddForce(Vector2.up * _pogoJumpForce, ForceMode2D.Impulse);
                audioManager.PlaySFX(audioManager.PogoAttack); 

                return; 
            }
        }
    }


    private void Die()
    {
        Debug.Log("Player Died!");
        audioManager.PlaySFX(audioManager.death); // Menggunakan audioManager.death

        if (_playerMovement != null)
        {
            _playerMovement.enabled = false;
            RB.linearVelocity = Vector2.zero; 
            RB.gravityScale = 0; // Matikan gravitasi untuk mencegah gerakan lebih lanjut
        }

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(_deathRespawnDelay);

        transform.position = _spawnPoint.position;

        if (_playerMovement != null)
        {
            _playerMovement.enabled = true;
        }

        StartCoroutine(InvincibilityRoutine());

        Debug.Log("Player Respawned!");
    }

    private IEnumerator InvincibilityRoutine()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(_invincibilityDuration);
        _isInvincible = false;
    }

    public void SetSpawnPoint(Transform newSpawnPoint)
    {
        _spawnPoint = newSpawnPoint;
        Debug.Log($"Spawn point updated to: {_spawnPoint.name}");
    }

    #region DEBUG METHODS
    private void OnDrawGizmosSelected()
    {
        if (_pogoAttackPoint == null) return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(_pogoAttackPoint.position, _pogoAttackRange);
    }
    #endregion
}