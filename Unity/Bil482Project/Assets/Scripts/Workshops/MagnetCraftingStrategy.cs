using UnityEngine;

public class MagnetCraftingStrategy : IProductionStrategy
{
    public int neededAmount { get; set; } = 2;

    public void Produce(GameObject inputResource, Workshop workshop)
    {
        // Input resource artýk null - workshop inventory'den çalýþýyor
        // Doðrudan workshop'un copperWirePrefab'ýný kullan
        workshop.SpawnResource(workshop.magnetPrefab);
        Debug.Log(" Copper Wire crafted and sent to conveyor belt!");
    }
}
