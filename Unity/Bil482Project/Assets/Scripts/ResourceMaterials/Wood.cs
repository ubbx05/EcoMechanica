using UnityEngine;


public class Wood : Resource
{
    protected override void InitializeResource()
    {
        resourceName = "Wood";
        resourceType = ResourceType.Wood;
        resourceIncome = 1;
    }
    
    /*
    void Awake()
    {
        resourceType = "Wood";
        resourceIncome = 1; 
    }
    */
}
