using UnityEngine;

public class SteelCraftStrategy : IProductionStrategy
{
    private Workshop workshop;
    private GameObject steelPrefab;

    public int neededAmount { get; set; } = 2;
   
    public void Produce(GameObject inputResource, Workshop workshop)
    {
        if(inputResource == workshop.demirIngotPrefab)
        {
            workshop.SpawnResource(steelPrefab);
        }
        
    }

    
}
