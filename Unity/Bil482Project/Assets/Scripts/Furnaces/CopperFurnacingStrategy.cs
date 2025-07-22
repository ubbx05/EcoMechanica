using UnityEngine;

public class CopperFurnacingStrategy : FurnacingStrategy
{
    public int neededAmount { get; set; } = 2;

    public void furnace(GameObject inputResource, Furnace furnace)
    {
        // Input resource artýk null - furnace inventory'den çalýþýyor
        // Doðrudan furnace'ýn copperore_prefab'ýný kullan
        furnace.SpawnResource(furnace.copperore_prefab);
        Debug.Log("Copper Bar crafted and sent to conveyor belt!");
    }
}