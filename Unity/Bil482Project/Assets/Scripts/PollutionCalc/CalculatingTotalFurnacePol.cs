using UnityEngine;

public class CalculatingTotalFurnacePol : MonoBehaviour
{
    public int TotalFurnacePol = 0;
    public int Tier1Ext;
    public int Tier2Ext;
    public int Tier3Ext;

    void OnEnable()
    {
        BuildingManager.OnFurnacePlaced += HandleExtractorPlaced;
    }

    void OnDisable()
    {
        BuildingManager.OnFurnacePlaced -= HandleExtractorPlaced;
    }

    void HandleExtractorPlaced(int tier)
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
        TotalFurnacePol = (Tier1Ext * 1) + (Tier2Ext * 2) + (Tier3Ext * 3);
    }
}
