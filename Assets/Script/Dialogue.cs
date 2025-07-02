using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // 1. DITAMBAHKAN: Untuk mengakses fungsi manajemen scene

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    private PlayerMovement _playerMovement;
    private int index;

    // Awake dijalankan sebelum Start. Baik untuk inisialisasi referensi.
    void Awake()
    {
        // 2. DIPERBAIKI: Cari komponen PlayerMovement yang aktif di scene saat ini.
        // Skrip Anda sebelumnya tidak pernah mengisi variabel _playerMovement, yang akan menyebabkan error.
        _playerMovement = FindObjectOfType<PlayerMovement>();
    }

    void Start()
    {
        textComponent.text = string.Empty;
        
        // Nonaktifkan gerakan player jika referensinya ditemukan
        if (_playerMovement != null)
        {
            _playerMovement.enabled = false;
        }
        else
        {
            // Beri peringatan jika skrip player tidak ditemukan, untuk memudahkan debugging.
            Debug.LogWarning("PlayerMovement script not found in the scene!");
        }

        StartDialogue();
    }

    void Update()
    {
        // Menggunakan Input.GetMouseButtonDown(0) sudah bagus, tidak perlu diubah.
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        // 3. INTI PERUBAHAN ADA DI SINI
        // Dapatkan build index dari scene yang sedang berjalan
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Dapatkan jumlah total scene yang ada di Build Settings
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        // Cek apakah scene saat ini adalah scene terakhir (misal: total ada 3 scene, indexnya 0, 1, 2. Maka yg terakhir adalah index 2)
        if (currentSceneIndex == totalScenes - 1)
        {
            // Jika ini adalah scene terakhir, muat Main Menu (yang biasanya ada di index 0)
            Debug.Log("Final scene finished. Returning to Main Menu.");
            SceneManager.LoadScene(0); // Memuat scene dengan build index 0
        }
        else
        {
            // Jika BUKAN scene terakhir, lanjutkan seperti biasa:
            // Sembunyikan dialog box dan aktifkan kembali player.
            gameObject.SetActive(false);
            if (_playerMovement != null)
            {
                _playerMovement.enabled = true;
            }
        }
    }
}
