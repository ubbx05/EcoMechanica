using UnityEngine;

public class HamDemir : Resource
{
    protected override void InitializeResource()
    {
        resourceName = "HamDemir";
        resourceType = ResourceType.IronOre;
        resourceIncome = 1;
    }
    
    /*
    void Awake()
    {
        resourceType = "HamDemir";
        resourceIncome = 1;
    }
    */

}
