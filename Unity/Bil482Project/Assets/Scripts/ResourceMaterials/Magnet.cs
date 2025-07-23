using UnityEngine;

public class Magnet : Resource
{
    protected override void InitializeResource()
    {
        resourceName = "Magnet";
        resourceType = ResourceType.Magnet;
        resourceIncome = 3;
    }

    /*
    private void Awake()
    {
        resourceType = "Magnet";
        resourceIncome = 10;
    }
    */
}

