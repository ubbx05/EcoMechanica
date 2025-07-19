using UnityEngine;

public class CopperWireCraftStrategy : IProductionStrategy
{
    private Workshop workshop;
    private GameObject copperPrefab;

    public CopperWireCraftStrategy(Workshop workshop, GameObject copperPrefab)
    {
        this.workshop = workshop;
        this.copperPrefab = copperPrefab;

    }
    public void Produce(GameObject inputResource, Workshop workshop)
    {
        if (inputResource == copperPrefab)
        {
            workshop.SpawnResource(workshop.copperWirePrefab);
            //Debug.Log("Steel produced from iron");
        }
    }
}
