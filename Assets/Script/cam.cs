using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Referensi ke objek yang akan diikuti kamera.
    [SerializeField] private Transform target;

    // Jarak default kamera dari target (terutama untuk sumbu Z).
    private Vector3 offset = new Vector3(0f, 0f, -10f);

    // Waktu yang dibutuhkan untuk mencapai posisi target.
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    // --- BARU: Variabel untuk Batas Panggung ---
    [Header("Stage Boundaries")]
    [Tooltip("Koordinat X dan Y dari sudut kiri-bawah panggung.")]
    [SerializeField] private Vector2 stageBoundsMin;

    [Tooltip("Koordinat X dan Y dari sudut kanan-atas panggung.")]
    [SerializeField] private Vector2 stageBoundsMax;
    
    // Variabel untuk menyimpan komponen kamera
    private Camera cam;

    void Awake()
    {
        // Mendapatkan komponen Kamera yang ada di GameObject yang sama
        cam = GetComponent<Camera>();
    }

    // Gunakan LateUpdate untuk pergerakan kamera agar lebih mulus
    void LateUpdate()
    {
        if (target == null || cam == null)
        {
            return; // Keluar jika target atau kamera tidak ada
        }

        // 1. Hitung posisi tujuan awal (target + offset)
        Vector3 targetPosition = target.position + offset;

        // 2. Hitung posisi baru yang sudah dihaluskan (smooth)
        // Kita belum terapkan ini, hanya menghitungnya sebagai kandidat posisi.
        Vector3 newPos = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // 3. Hitung batas efektif untuk posisi kamera berdasarkan ukuran viewport
        // Ini memastikan Tepi Kamera yang berhenti di batas, bukan titik tengahnya.
        float cameraHalfHeight = cam.orthographicSize;
        float cameraHalfWidth = cameraHalfHeight * cam.aspect;

        // 4. Jepit (Clamp) posisi kamera agar tidak keluar dari batas panggung
        float clampedX = Mathf.Clamp(newPos.x, stageBoundsMin.x + cameraHalfWidth, stageBoundsMax.x - cameraHalfWidth);
        float clampedY = Mathf.Clamp(newPos.y, stageBoundsMin.y + cameraHalfHeight, stageBoundsMax.y - cameraHalfHeight);

        // 5. Terapkan posisi akhir yang sudah dijepit
        transform.position = new Vector3(clampedX, clampedY, newPos.z);
    }

    // --- BARU: Menggambar Gizmo untuk visualisasi Batas Panggung ---
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green; // Atur warna gizmo menjadi hijau

        // Hitung ukuran dan pusat dari kotak batas
        Vector3 center = new Vector3(
            stageBoundsMin.x + (stageBoundsMax.x - stageBoundsMin.x) / 2,
            stageBoundsMin.y + (stageBoundsMax.y - stageBoundsMin.y) / 2,
            0f
        );

        Vector3 size = new Vector3(
            stageBoundsMax.x - stageBoundsMin.x,
            stageBoundsMax.y - stageBoundsMin.y,
            1f
        );

        // Gambar kotak transparan (wireframe) yang merepresentasikan batas panggung
        Gizmos.DrawWireCube(center, size);
    }
}