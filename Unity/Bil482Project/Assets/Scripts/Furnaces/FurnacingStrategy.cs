using UnityEngine;

public interface FurnacingStrategy 
{
    int neededAmount { get; set; }
    void furnace(GameObject inputResource, Furnace furnace); 
}
