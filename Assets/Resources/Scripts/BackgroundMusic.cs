using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    public AudioClip intro;
    public AudioClip loop;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        if (intro != null)
        {
            audioSource.PlayOneShot(intro);

            audioSource.clip = loop;
            audioSource.loop = true;
            audioSource.PlayScheduled(AudioSettings.dspTime + intro.length);
        }
        else
        {
            audioSource.clip = loop;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}

