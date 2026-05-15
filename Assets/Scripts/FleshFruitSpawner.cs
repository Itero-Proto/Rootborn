using UnityEngine;

public class FleshFruitSpawner : MonoBehaviour
{
    public GameObject healingFruitPrefab;

    [Header("Settings")]
    public float checkInterval = 10f;
    [Range(0f, 1f)] public float spawnChance = 0.1f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= checkInterval)
        {
            timer = 0f;
            TrySpawnFruit();
        }
    }

    void TrySpawnFruit()
    {
        if (Random.value <= spawnChance)
        {
            Instantiate(
                healingFruitPrefab,
                transform.position,
                Quaternion.identity
            );
        }
    }
}