using UnityEngine;

public class Odun : Resource
{
    protected override void InitializeResource()
    {
        resourceName = "Odun";
        resourceType = ResourceType.Wood;
        resourceIncome = 1;
    }
    
    /*
    void Awake()
    {
        resourceType = "Odun";
        resourceIncome = 1; 
    }
    */
}
