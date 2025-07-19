using UnityEngine;

public class SteelCraftStrategy : IProductionStrategy
{
    private Workshop workshop;
    private GameObject ironPrefab;

    public SteelCraftStrategy(Workshop workshop, GameObject ironPrefab)
    {
        this.workshop = workshop;
        this.ironPrefab = ironPrefab;
        
    }
    public void Produce(GameObject inputResource, Workshop workshop)
    {
        if(inputResource == ironPrefab)
        {
            workshop.SpawnResource(workshop.steelPrefab);
            //Debug.Log("Steel produced from iron");
        }
        
    }

    
}
