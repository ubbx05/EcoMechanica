using UnityEngine;

public class CopperOre : Resource
{
    protected override void InitializeResource()
    {
        resourceName = "CopperOre";
        resourceType = ResourceType.CopperOre;
        resourceIncome = 1;
    }

    /*
    private void Awake()
    {
        resourceType = "CopperOre";
        resourceIncome = 1;
    }
    */
}

