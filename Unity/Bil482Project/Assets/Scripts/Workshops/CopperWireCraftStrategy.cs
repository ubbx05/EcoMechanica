using UnityEngine;

public class CopperWireCraftStrategy : IProductionStrategy
{
    public int neededAmount { get; set; } = 2;

    public void Produce(GameObject inputResource, Workshop workshop)
    {
        // Input resource artýk null - workshop inventory'den çalýþýyor
        // Doðrudan workshop'un copperWirePrefab'ýný kullan
        workshop.SpawnResource(workshop.copperWirePrefab);
        Debug.Log(" Copper Wire crafted and sent to conveyor belt!");
    }
}