using UnityEngine;

public class CopperWire : Resource
{
    protected override void InitializeResource()
    {
        resourceName = "CopperWire";
        resourceType = ResourceType.CopperWire;
        resourceIncome = 3;
    }

    /*
    private void Awake()
    {
        resourceType = "CopperWire";
        resourceIncome = 10;
    }
    */
}

