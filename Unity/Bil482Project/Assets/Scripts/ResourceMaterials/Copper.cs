using UnityEngine;

public class Copper : Resource
{
    protected override void InitializeResource()
    {
        resourceName = "Copper";
        resourceType = ResourceType.Copper;
        resourceIncome = 3;
    }

    /*
    private void Awake()
    {
        resourceType = "Copper";
        resourceIncome = 3;
    }
    */
}

