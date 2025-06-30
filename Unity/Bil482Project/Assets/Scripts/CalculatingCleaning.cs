using UnityEngine;

public class CalculatingCleaning : MonoBehaviour
{
    public int TotalCleaning;
    public int Tier1Ext;
    public int Tier2Ext;
    public int Tier3Ext;

    void OnEnable()
    {
        //MachineManager.OnCleanerPlaced += HandleCleanerPlaced;
    }

    void OnDisable()
    {
        //MachineManager.OnCleanerPlaced -= HandleCleanerPlaced;
    }

    void HandleCleanerPlaced(int tier)
    {
        switch (tier)
        {
            case 1:
                Tier1Ext++;
                break;
            case 2:
                Tier2Ext++;
                break;
            case 3:
                Tier3Ext++;
                break;
        }
    }

    void Update()
    {
        TotalCleaning = (Tier1Ext * 1) + (Tier2Ext * 2) + (Tier3Ext * 3);
    }
}
