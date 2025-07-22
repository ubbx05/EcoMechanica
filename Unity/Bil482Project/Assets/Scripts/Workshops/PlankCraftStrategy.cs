using UnityEngine;

public class PlankCraftStrategy : IProductionStrategy
{
    public int neededAmount { get; set; } = 1;

    public void Produce(GameObject inputResource, Workshop workshop)
    {
        // Input resource artýk null - workshop inventory'den çalýþýyor
        // Doðrudan workshop'un plankPrefab'ýný kullan
        workshop.SpawnResource(workshop.plankPrefab);
        Debug.Log(" Plank crafted and sent to conveyor belt!");
    }
}