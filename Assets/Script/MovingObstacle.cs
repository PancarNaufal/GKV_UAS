using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [Range(0,5)]
    public float speed; //kecepatan pergerakan objek.
    Vector3 targetPos; //posisi tujuan saat ini yang sedang dituju objek.

    public GameObject ways; //GameObject yang menyimpan semua titik (waypoints) sebagai child.
    public Transform[] wayPoints; //array berisi Transform dari semua child di ways.
    int pointIndex; //indeks waypoint yang sedang dituju.
    int pointCount; // jumlah total waypoint.
    int direction = 1; //arah gerak indeks (1 untuk maju, -1 untuk mundur).

    //digunakan dalam konteks kontrol pergerakan dan jeda dalam script MovingObstacle
    [Range(0,2)]
    public float waitDuration;
    //berfungsi sebagai pengatur on/off gerakan objek — semacam saklar kecepatan dalam script MovingObstacle
    int speedMultiplier = 1;


    //Dipanggil sebelum Start(), digunakan untuk menginisialisasi array wayPoints.
    //Mengambil semua child dari ways dan menyimpannya sebagai Transform dalam array wayPoints.
    //Ini memungkinkan Anda untuk mengatur waypoint cukup dengan menaruh anak-anak (child) transform di dalam GameObject ways di editor Unity
    private void Awake(){
        wayPoints = new Transform[ways.transform.childCount]; 
        for(int i =0; i<ways.gameObject.transform.childCount; i++){
            wayPoints[i] = ways.transform.GetChild(i).gameObject.transform;
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        pointCount = wayPoints.Length; //pointCount menyimpan jumlah waypoint yang tersedia.
        pointIndex = 1; //pointIndex di-set ke 1, artinya objek pertama kali menuju titik kedua (karena indeks array mulai dari 0).
        targetPos = wayPoints[pointIndex].transform.position; //targetPos diset ke posisi waypoint yang dituju.
    }

    // Update is called once per frame
    void Update()
    {
        var step = speedMultiplier * speed * Time.deltaTime; //step dihitung dari speed dan Time.deltaTime agar kecepatan gerak konsisten di semua framerate.
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step); //Vector3.MoveTowards: menggerakkan posisi objek ke targetPos dengan kecepatan step

        //ketika objek mencapai posisi target index = 1
        if (transform.position == targetPos)
        { //Jika posisi objek sudah sampai targetPos, maka panggil NextPoint() untuk menentukan tujuan berikutnya.
            NextPoint();
        }
        
    }

    void NextPoint(){
        if(pointIndex == pointCount -1){ //Jika objek sudah mencapai waypoint terakhir (pointCount - 1), ubah arah ke -1 (mundur).
            direction = -1;
        }

        if(pointIndex == 0){ //Jika sudah di waypoint pertama (index = 0), ubah arah ke 1 (maju).
            direction = 1;
        }

        pointIndex += direction; //Update pointIndex dengan arah saat ini.
        targetPos = wayPoints[pointIndex].transform.position; //Update targetPos ke posisi waypoint baru.
        StartCoroutine(WaitNextPoint());
    }

    //Fungsi IEnumerator WaitNextPoint() digunakan untuk memberi jeda waktu (delay) sebelum objek melanjutkan pergerakan ke waypoint berikutnya.
    IEnumerator WaitNextPoint(){
        //Menghentikan gerakan objek dengan mengatur speedMultiplier = 0, sehingga dalam Update(), perhitungan langkah (step) menjadi 0.
        //Akibatnya, MoveTowards tidak akan menggerakkan objek.
        speedMultiplier = 0;

        //Menunggu selama waitDuration detik sebelum melanjutkan ke baris berikutnya.
        //Ini adalah bagian dari coroutine di Unity, memungkinkan jeda tanpa membekukan seluruh game.
        yield return new WaitForSeconds(waitDuration);  

        //Setelah jeda selesai, mengembalikan kecepatan dengan mengatur speedMultiplier = 1, sehingga objek kembali bergerak di frame berikutnya.
        speedMultiplier = 1;
    }
}