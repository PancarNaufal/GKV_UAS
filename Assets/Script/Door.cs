// DoorController.cs

using UnityEngine;

public class DoorController : MonoBehaviour
{
    // Opsional: untuk efek visual atau suara saat pintu terbuka
    public GameObject openEffectPrefab; 
    public AudioClip openSound;

    // Kita menggunakan OnCollisionEnter2D karena pintu kita solid (bukan trigger)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Cek apakah yang menabrak adalah Player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player touched the door.");

            // 2. Dapatkan komponen PlayerMovement dari player
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();

            // 3. Cek apakah player ada dan memiliki kunci
            if (player != null && player.hasKey)
            {
                // Jika punya kunci, buka pintunya
                OpenDoor(player);
            }
            else
            {
                // Jika tidak punya kunci, beri feedback (opsional)
                Debug.Log("The door is locked. You need a key!");
                // Opsional: Mainkan suara pintu terkunci di sini
            }
        }
    }

    private void OpenDoor(PlayerMovement player)
    {
        Debug.Log("Key used. Door is opening!");

        // 4. Set status kunci player kembali ke false
        player.hasKey = false;

        // 5. Cari dan hancurkan objek kunci kecil yang mengikuti player
        //    (Pastikan prefab kunci kecil punya tag "SmallKey")
        GameObject smallKeyVisual = GameObject.FindWithTag("SmallKey");
        if (smallKeyVisual != null)
        {
            Destroy(smallKeyVisual);
        }

        // Opsional: mainkan efek dan suara
        if (openEffectPrefab != null)
        {
            Instantiate(openEffectPrefab, transform.position, Quaternion.identity);
        }
        // AudioManager.Instance.PlaySound(openSound); // Contoh jika pakai AudioManager

        // 6. Hancurkan atau nonaktifkan pintu
        // gameObject.SetActive(false); // Cara sederhana
        Destroy(gameObject); // Cara permanen
    }
}