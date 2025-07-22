using UnityEngine;

public class SteelCraftStrategy : IProductionStrategy
{
    public int neededAmount { get; set; } = 2;

    public void Produce(GameObject inputResource, Workshop workshop)
    {
        // Input resource artýk null - workshop inventory'den çalýþýyor
        // Doðrudan workshop'un steelPrefab'ýný kullan
        workshop.SpawnResource(workshop.steelPrefab);
        Debug.Log("Steel crafted and sent to conveyor belt!");
    }
}