using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource MusicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource LoopSFXSource;

    [Header("Audio Clips")]
    public AudioClip background;
    public AudioClip death;
    public AudioClip checkpoint;
    public AudioClip PogoAttack;
    public AudioClip jump;
    public AudioClip portalIn;
    public AudioClip portalOut;
    public AudioClip dash;
    public AudioClip land;
    public AudioClip walk; // Untuk suara langkah
    public AudioClip wallJump; // Untuk suara lompat dinding
    public AudioClip wallSlide; // Untuk suara meluncur di dinding
    public AudioClip playerHurt;


    public void Start()
    {
        MusicSource.clip = background;
        MusicSource.loop = true;
        MusicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    // Metode baru untuk memutar SFX yang berulang
    public void PlayLoopingSFX(AudioClip clip)
    {
        if (LoopSFXSource.isPlaying && LoopSFXSource.clip == clip)
        {
            // Sudah berputar dan klipnya sama, tidak perlu memutar ulang
            return;
        }
        LoopSFXSource.clip = clip;
        LoopSFXSource.loop = true; // Penting: atur agar klip berulang
        LoopSFXSource.Play();
    }

    // Metode untuk menghentikan SFX yang berulang
    public void StopLoopingSFX()
    {
        if (LoopSFXSource.isPlaying)
        {
            LoopSFXSource.Stop();
            LoopSFXSource.loop = false; // Reset loop agar tidak memengaruhi klip lain
            LoopSFXSource.clip = null; // Hapus klip agar tidak memutar klip sebelumnya
        }
    }
}    