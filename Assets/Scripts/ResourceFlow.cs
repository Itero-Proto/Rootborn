using UnityEngine;

public class ResourceFlow : MonoBehaviour
{
    public UmbilicalCord cord;
    public GameObject flowPrefab;

    public void PlayFlow()
    {
        GameObject obj = Instantiate(flowPrefab);

        FlowOnCord flow = obj.GetComponent<FlowOnCord>();
        flow.cord = cord;

        flow.Play();
    }
}