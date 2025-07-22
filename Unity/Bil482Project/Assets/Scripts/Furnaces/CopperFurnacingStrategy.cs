using UnityEngine;

public class CopperFurnacingStrategy : FurnacingStrategy
{
    public int neededAmount { get; set; } = 2;

    public void furnace(GameObject inputResource, Furnace furnace)
    {
        // Input resource art�k null - furnace inventory'den �al���yor
        // Do�rudan furnace'�n copperore_prefab'�n� kullan
        furnace.SpawnResource(furnace.copperore_prefab);
        Debug.Log("Copper Bar crafted and sent to conveyor belt!");
    }
}