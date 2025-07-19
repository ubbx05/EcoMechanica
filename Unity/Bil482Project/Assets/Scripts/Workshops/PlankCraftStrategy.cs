using UnityEngine;

public class PlankCraftStrategy : IProductionStrategy
{   
    private Workshop workshop;
    private GameObject plankPrefab;

    public int neededAmount { get; set; } = 1;

    public void Produce(GameObject inputResource, Workshop workshop)
    {
        if(inputResource == workshop.odunPrefab)
        {
            workshop.SpawnResource(plankPrefab);
        }
    }
}