using UnityEngine;

public class IronOre : Resource
{
    protected override void InitializeResource()
    {
        resourceName = "IronOre";
        resourceType = ResourceType.IronOre;
        resourceIncome = 1;
    }

    /*
    private void Awake()
    {
        resourceType = "IronOre";
        resourceIncome = 1;
    }
    */
}

