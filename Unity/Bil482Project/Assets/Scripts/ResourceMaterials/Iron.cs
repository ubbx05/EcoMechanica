using UnityEngine;

public class Iron : Resource
{
    protected override void InitializeResource()
    {
        resourceName = "Iron";
        resourceType = ResourceType.Iron;
        resourceIncome = 2;
    }

    /*
    private void Awake()
    {
        resourceType = "Iron";
        resourceIncome = 3;
    }
    */
}

