using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class UmbilicalCord : MonoBehaviour
{
    public Transform tree;
    public Transform player;
    private LineRenderer line;
    [Header("Length")]
    public float baseMaxDistance = 5f;
    public float currentMaxDistance;
    public float maxSafeLength = 15f;
    [Header("Shape")]
    public int segments = 15;
    public float waveAmplitude = 0.3f;
    public float waveFrequency = 3f;

    public GameObject breakEffect;

    private bool isBroken = false;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = segments;
        currentMaxDistance = baseMaxDistance;
    }
    void Update()
    {
        if (tree == null || player == null || isBroken) return;

        DrawCord();

        float distance = Vector3.Distance(player.position, tree.position);

        // 🔥 ФИНАЛЬНЫЙ РАЗРЫВ
        if (!isBroken && distance >= maxSafeLength)
        {
            StartCoroutine(BreakCord());
        }
    }
    public void IncreaseLength(float amount)
    {
        currentMaxDistance += amount;

    }

    void DrawCord()
    {
        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);
            Vector3 pos = GetPointOnCord(t);
            line.SetPosition(i, pos);
        }
    }

    public Vector3 GetPointOnCord(float t)
    {
        Vector3 pos = Vector3.Lerp(player.position, tree.position, t);

        float wave = Mathf.Sin(Time.time * waveFrequency + t * 10f) * waveAmplitude;
        float sag = Mathf.Sin(t * Mathf.PI) * -0.5f;

        pos += new Vector3(0, wave + sag, 0);

        return pos;
    }

    IEnumerator BreakCord()
    {
        if (isBroken) yield break;

        isBroken = true;

        // отключаем линию
        if (line != null)
            line.enabled = false;

        // эффект разрыва
        if (breakEffect != null)
        {
            Vector3 mid = (player.position + tree.position) / 2f;
            Instantiate(breakEffect, mid, Quaternion.identity);
        }

        // небольшая пауза
        yield return new WaitForSeconds(2f);

        // вызываем концовку
        yield return StartCoroutine(GameManager.Instance.FadeToBlack(1.5f));
        GameManager.Instance.EndGameCord();
    }
}