using UnityEngine;

public class HamBakir : Resource
{
    
    protected override void InitializeResource()
    {
        resourceName = "HamBakir";
        resourceType = ResourceType.CopperOre;
        resourceIncome = 1;
    }

    /*
    private voidAwake()
    {
        resourceType = "HamBakir";
        resourceIncome = 1;
    }
    */
}
