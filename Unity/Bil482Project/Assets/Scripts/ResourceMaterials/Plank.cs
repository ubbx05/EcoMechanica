using UnityEngine;

public class Plank : Resource
{
    protected override void InitializeResource()
    {
        resourceName = "Plank";
        resourceType = ResourceType.Plank;
        resourceIncome = 2;
    }

    /*
    private void Awake()
    {
        resourceType = "Plank";
        resourceIncome = 3;
    }
    */
}

