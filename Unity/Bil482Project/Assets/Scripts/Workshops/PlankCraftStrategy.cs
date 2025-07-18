using UnityEngine;

public class PlankCraftStrategy : IProductionStrategy
{   
    private Workshop workshop;
    private GameObject plankPrefab; 

    public PlankCraftStrategy(Workshop workshop, GameObject plankPrefab)
    {
        this.workshop = workshop;
        this.plankPrefab = plankPrefab;
    }

    public void Produce(GameObject inputResource, Workshop workshop)
    {
        if(inputResource == workshop.odunPrefab)
        {
            workshop.SpawnResource(plankPrefab);
        }
    }
}