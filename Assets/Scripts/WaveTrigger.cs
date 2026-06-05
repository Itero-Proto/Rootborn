using UnityEngine;

public class WaveTrigger : MonoBehaviour
{
    public WaveSpawner waveSpawner;
    [Header("Audio")]
    public AudioClip waveTriggerSound;
    private AudioSource audioSource;
    private bool triggered = false;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (triggered)
            return;

        triggered = true;

        waveSpawner.StartWaves();
        
        if (waveTriggerSound != null)
        {
            GameObject temp = new GameObject("waveTriggerSound");
            temp.transform.position = transform.position;
            AudioSource a = temp.AddComponent<AudioSource>();
            a.PlayOneShot(waveTriggerSound, 0.9f);

            Destroy(temp, waveTriggerSound.length);
        }
        Destroy(gameObject);
    }
}