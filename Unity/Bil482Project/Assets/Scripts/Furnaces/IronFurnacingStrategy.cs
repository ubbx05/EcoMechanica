using UnityEngine;

public class IronFurnacing : FurnacingStrategy
{
    private Furnace Furnace;
    private GameObject Ironore;

    public IronFurnacing(GameObject Ironore , Furnace furnace)
    {
        this.Ironore =Ironore;
        this.Furnace = furnace;
        if (Ironore == null)
        {
            //Debug.LogError("Ironore prefab is not assigned!");
        }
    }

    public void furnace()
    {
        //gelecek
    }
}
