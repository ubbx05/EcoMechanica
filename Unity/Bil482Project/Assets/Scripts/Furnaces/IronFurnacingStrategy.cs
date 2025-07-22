using UnityEngine;

public class IronFurnacingStrategy : FurnacingStrategy
{
    public int neededAmount { get; set; } = 2;

    public void furnace(GameObject inputResource, Furnace furnace)
    {
        // Input resource artýk null - furnace inventory'den çalýþýyor
        // Doðrudan furnace'ýn ironore_prefab'ýný kullan
        furnace.SpawnResource(furnace.ironore_prefab);
        Debug.Log("Iron Bar crafted and sent to conveyor belt!");
    }
}