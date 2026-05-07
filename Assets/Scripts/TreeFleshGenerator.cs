using System.Collections.Generic;
using UnityEngine;

public class TreeFleshGenerator : MonoBehaviour
{
    public List<FleshPoint> points = new List<FleshPoint>();
    public GameObject fleshPrefab;
    [Header("Thickness")]
    public float minThickness = 0.05f;
    public float maxThickness = 0.2f;
    [Header("Length")]
    public float minLength = 0.5f;
    public float maxLength = 2f;

    private List<FleshPoint> freePoints = new List<FleshPoint>();

    void Awake()
    {
        // кэшируем свободные точки
        foreach (var p in points)
        {
            if (!p.isUsed)
                freePoints.Add(p);
        }
    }

    public void AddOrganic(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (freePoints.Count == 0)
                return;

            SpawnFlesh();
        }
    }
    void SpawnFlesh()
    {
        int index = Random.Range(0, freePoints.Count);
        FleshPoint point = freePoints[index];

        freePoints.RemoveAt(index);
        point.isUsed = true;

        // создаём кишку
        GameObject obj = Instantiate(
            fleshPrefab,
            point.transform.position,
            Quaternion.identity,
            transform
        );

        HangingFlesh flesh = obj.GetComponent<HangingFlesh>();

        // создаём конец
        GameObject end = new GameObject("EndPoint");

        float length = Random.Range(minLength, maxLength);
        float thickness = Random.Range(minThickness, maxThickness);

        end.transform.position = point.transform.position + Vector3.down * length;
        end.transform.parent = obj.transform;

        flesh.startPoint = point.transform;
        flesh.endPoint = end.transform;

        // ❗ ВАЖНО: передаём толщину сюда
        flesh.thickness = thickness;

        // 🎯 рандом параметров
        flesh.swaySpeed = Random.Range(1.5f, 3f);
        flesh.swayAmount = Random.Range(0.1f, 0.25f);
        flesh.gravityPull = Random.Range(0.3f, 0.8f);

        // небольшой разброс позиции
        obj.transform.position += Random.insideUnitSphere * 0.05f;
    }
}