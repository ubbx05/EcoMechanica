using UnityEngine;

public class Computer : Resource
{
    protected override void InitializeResource()
    {
        resourceType = ResourceType.Computer;
        resourceName = "Computer";
        resourceIncome = 5;
    }

    
}
