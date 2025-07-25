using UnityEngine;

public class MagnetCraftingStrategy : IProductionStrategy
{
    public int neededAmount { get; set; } = 2;

    public void Produce(GameObject inputResource, Workshop workshop)
    {
        // Input resource art�k null - workshop inventory'den �al���yor
        // Do�rudan workshop'un copperWirePrefab'�n� kullan
        workshop.SpawnResource(workshop.magnetPrefab);
        Debug.Log(" Copper Wire crafted and sent to conveyor belt!");
    }
}
