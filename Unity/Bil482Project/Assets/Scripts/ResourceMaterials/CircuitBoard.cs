using UnityEngine;

public class CircuitBoard : Resource
{
    protected override void InitializeResource()
    {
        resourceName = "CircuitBoard";
        resourceType = ResourceType.CircuitBoard;
        resourceIncome = 4;
    }

    /*
    private void Awake()
    {
        resourceType = "CircuitBoard";
        resourceIncome = 100;
    }
    */
}

