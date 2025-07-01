using UnityEngine;

public class CalculatingTotalExcPol : MonoBehaviour
{
    public int TotalExtractorPollution;
    public int Tier1Ext;
    public int Tier2Ext;
    public int Tier3Ext;

    void OnEnable()
    {
        BuildingManager.OnExtractorPlaced += HandleExtractorPlaced;
    }

    void OnDisable()
    {
        BuildingManager.OnExtractorPlaced -= HandleExtractorPlaced;
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
        TotalExtractorPollution = (Tier1Ext * 1) + (Tier2Ext * 2) + (Tier3Ext * 3);
    }
}
