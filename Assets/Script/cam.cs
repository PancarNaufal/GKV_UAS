using System.Collections;
// using System.Collections.Generic;
using UnityEngine; //penting karena semua komponen Unity seperti Transform, MonoBehaviour, Vector3, dll, ada di namespace ini

//Ini adalah deklarasi kelas yang turunan dari MonoBehaviour, 
//artinya ini adalah script yang bisa ditempel ke GameObject di Unity.
public class CameraFollow : MonoBehaviour 
{
    // Ini adalah jarak antara kamera dan target (misalnya karakter pemain). 
    // Kamera akan mengikuti target tetapi tetap berada di belakangnya sejauh -10 di sumbu Z (umumnya untuk 2D game).
    private Vector3 offset = new Vector3(0f,0f,-10f); 

    //Waktu yang dibutuhkan untuk mencapai posisi target. 
    //Nilai ini menentukan seberapa halus perpindahan kamera.
    private float smoothTime = 0.25f;

    // Digunakan oleh fungsi SmoothDamp untuk menghitung gerakan halus.
    private Vector3 velocity = Vector3.zero;

    //Referensi ke objek yang akan diikuti kamera. 
    //Menggunakan [SerializeField] agar tetap private tapi bisa di-assign dari Unity Inspector.
    [SerializeField] private Transform target;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Setiap frame, kamera menghitung posisi target + offset (targetPosition).
        Vector3 targetPosition = target. position + offset;

        //SmoothDamp digunakan untuk membuat gerakan kamera menuju targetPosition secara halus, tanpa gerakan mendadak.
        //ref velocity menyimpan kecepatan agar transisi tetap konsisten.
        //smoothTime menentukan seberapa cepat atau lambat transisi itu terjadi.
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}