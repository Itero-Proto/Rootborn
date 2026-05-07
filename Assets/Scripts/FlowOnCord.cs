using UnityEngine;
using System.Collections;

public class FlowOnCord : MonoBehaviour
{
    public UmbilicalCord cord;

    public float duration = 0.5f;

    public void Play()
    {
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            transform.position = cord.GetPointOnCord(t);

            yield return null;
        }

        Destroy(gameObject);
    }
}