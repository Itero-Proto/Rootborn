using UnityEngine;

public class HangingFlesh : MonoBehaviour
{
    public Transform startPoint;   // ветка
    public Transform endPoint;     // низ
    public float thickness = 0.1f;
    public LineRenderer lr;

    [Header("Shape")]
    public int segments = 12;
    public float swayAmount = 0.15f;
    public float swaySpeed = 2f;
    public float gravityPull = 0.5f;
    void Start()
    {
        if (!lr) lr = GetComponent<LineRenderer>();

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

        lr.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);

            // базовая линия
            Vector3 pos = Vector3.Lerp(startPoint.position, endPoint.position, t);

            // провисание (чем ближе к середине, тем сильнее)
            float sag = Mathf.Sin(t * Mathf.PI) * gravityPull;
            pos.y -= sag;

            // органическое покачивание
            float noiseX = Mathf.PerlinNoise(Time.time * swaySpeed, t * 2f) - 0.5f;
            float noiseZ = Mathf.PerlinNoise(t * 2f, Time.time * swaySpeed) - 0.5f;

            Vector3 offset = new Vector3(noiseX, 0, noiseZ) * swayAmount;

            pos += offset;

            lr.SetPosition(i, pos);
        }
    }
}