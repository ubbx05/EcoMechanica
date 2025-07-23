using UnityEngine;

public class Steel : Resource
{
    protected override void InitializeResource()
    {
        resourceName = "Steel";
        resourceType = ResourceType.Steel;
        resourceIncome = 3;
    }

    /*
    private void Awake()
    {
        resourceType = "Steel";
        resourceIncome = 10;
    }
    */
}

