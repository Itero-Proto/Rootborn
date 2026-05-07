using UnityEngine;

public class HangingFlesh : MonoBehaviour
{
    float updateTimer;
    public float updateInterval = 0.05f; // 20 FPS
    public Transform startPoint;   // ветка
    public Transform endPoint;     // низ
    public float thickness = 0.1f;
    public LineRenderer lr;

    [Header("Shape")]
    public int segments = 6;
    public float swayAmount = 0.15f;
    public float swaySpeed = 2f;
    public float gravityPull = 0.5f;
    void Start()
    {
        if (!lr) lr = GetComponent<LineRenderer>();
        lr.positionCount = segments;
        lr.startWidth = thickness;
        lr.endWidth = thickness;
    }
    void Reset()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (!startPoint || !endPoint || !lr) return;

        updateTimer += Time.deltaTime;
        if (updateTimer < updateInterval) return;
        updateTimer = 0f;

        UpdateLine();
        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);

            // базовая линия
            Vector3 pos = Vector3.Lerp(startPoint.position, endPoint.position, t);

            // провисание (чем ближе к середине, тем сильнее)
            float sag = Mathf.Sin(t * Mathf.PI) * gravityPull;
            pos.y -= sag;
            float wave = Mathf.Sin(Time.time * swaySpeed + t * 5f);

            lr.SetPosition(i, pos);
        }
    }
    void UpdateLine()
    {
        lr.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);

            Vector3 pos = Vector3.Lerp(startPoint.position, endPoint.position, t);

            float sag = Mathf.Sin(t * Mathf.PI) * gravityPull;
            pos.y -= sag;

            float wave = Mathf.Sin(Time.time * swaySpeed + t * 5f);

            Vector3 offset = new Vector3(wave, 0, wave * 0.5f) * swayAmount;

            lr.SetPosition(i, pos + offset);
        }
    }
}