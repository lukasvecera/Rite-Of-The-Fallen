using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    public AudioSource walkingSource;

    public AudioClip background;
    public AudioClip walkingOnConcrete;
    public AudioClip walkingOnDirtGravel;
    public AudioClip runningOnConcrete;
    public AudioClip runningOnDirtGravel;
    public AudioClip deathSound;
    public AudioClip walkingCathedral;
    public AudioClip runningCathedral;
    public AudioClip jumpSound;
    public AudioClip fallGrunt;
    public AudioClip buttonSound;
    public AudioClip paperSound;
    public AudioClip bookOpenSound;
    public AudioClip bookCloseSound;
    public AudioClip pickupItemSound;
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;
    public AudioClip matchCandleSound;
    public AudioClip potionPouringSound;
    public AudioClip openChestSound;
    public AudioClip closeChestSound;
    public AudioClip altarSound;
    public AudioClip questCompleteSound;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        SFXSource.PlayOneShot(clip, volume);
    }

    public void PlaySFXDialogue(AudioClip clip)
    {
        SFXSource.clip = clip;
        SFXSource.Play();
    }

    public void StopSFX()
    {
        SFXSource.Stop();
    }

public void PlayWalking(AudioClip clip)
{
    if (walkingSource.isPlaying && walkingSource.clip == clip) return;
    walkingSource.clip = clip;
    walkingSource.loop = true;
    walkingSource.Play();
}

    public void StopWalking()
    {
        walkingSource.Stop();
    }
}
