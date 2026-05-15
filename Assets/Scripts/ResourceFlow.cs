using UnityEngine;

public class ResourceFlow : MonoBehaviour
{
    public UmbilicalCord cord;

    [Header("Flow Prefabs")]
    public GameObject organicFlowPrefab;
    public GameObject inorganicFlowPrefab;

    public void PlayFlow(DropType type)
    {
        GameObject prefab = null;

        switch (type)
        {
            case DropType.Organic:
                prefab = organicFlowPrefab;
                break;

            case DropType.Inorganic:
                prefab = inorganicFlowPrefab;
                break;
        }

        if (prefab == null)
            return;

        GameObject obj = Instantiate(prefab);

        FlowOnCord flow = obj.GetComponent<FlowOnCord>();

        if (flow != null)
        {
            flow.cord = cord;
            flow.Play();
        }
    }
}