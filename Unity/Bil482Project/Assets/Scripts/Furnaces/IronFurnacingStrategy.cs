using UnityEngine;

public class IronFurnacingStrategy : FurnacingStrategy
{
    public int neededAmount { get; set; } = 2;

    public void furnace(GameObject inputResource, Furnace furnace)
    {
        // Input resource art�k null - furnace inventory'den �al���yor
        // Do�rudan furnace'�n ironore_prefab'�n� kullan
        furnace.SpawnResource(furnace.ironore_prefab);
        Debug.Log("Iron Bar crafted and sent to conveyor belt!");
    }
}