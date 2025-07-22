using UnityEngine;

public class SteelCraftStrategy : IProductionStrategy
{
    public int neededAmount { get; set; } = 2;

    public void Produce(GameObject inputResource, Workshop workshop)
    {
        // Input resource art�k null - workshop inventory'den �al���yor
        // Do�rudan workshop'un steelPrefab'�n� kullan
        workshop.SpawnResource(workshop.steelPrefab);
        Debug.Log("Steel crafted and sent to conveyor belt!");
    }
}