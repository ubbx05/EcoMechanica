using UnityEngine;

public class CopperWireCraftStrategy : IProductionStrategy
{
    private Workshop workshop;
    private GameObject copperWirePrefab;

    public int neededAmount { get; set; } = 2;

    public void Produce(GameObject inputResource, Workshop workshop)
    {
        if(inputResource == workshop.bakirIngotPrefab)
        {
            workshop.SpawnResource(copperWirePrefab);
        }
    }
}
