using UnityEngine;

public class WaveTrigger : MonoBehaviour
{
    public WaveSpawner waveSpawner;

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (triggered)
            return;

        triggered = true;

        waveSpawner.StartWaves();

        Destroy(gameObject);
    }
}