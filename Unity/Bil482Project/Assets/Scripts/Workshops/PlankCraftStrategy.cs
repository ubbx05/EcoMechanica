using UnityEngine;

public class PlankCraftStrategy : IProductionStrategy
{
    public int neededAmount { get; set; } = 1;

    public void Produce(GameObject inputResource, Workshop workshop)
    {
        // Input resource art�k null - workshop inventory'den �al���yor
        // Do�rudan workshop'un plankPrefab'�n� kullan
        workshop.SpawnResource(workshop.plankPrefab);
        Debug.Log(" Plank crafted and sent to conveyor belt!");
    }
}