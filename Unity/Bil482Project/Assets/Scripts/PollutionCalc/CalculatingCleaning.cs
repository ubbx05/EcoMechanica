using UnityEngine;

public class CalculatingCleaning : MonoBehaviour
{
    public  int TotalCleaning = 0;
    public int Tier1Cleaner;
    public int Tier2Cleaner;
    public int Tier3Cleaner;

    void OnEnable()
    {
        BuildingManager.OnCleanerPlaced += HandleCleanerPlaced;
    }

    void OnDisable()
    {
        BuildingManager.OnCleanerPlaced -= HandleCleanerPlaced;
    }

    void HandleCleanerPlaced(int tier)
    {
        switch (tier)
        {
            case 1:
                Tier1Cleaner++;
                break;
            case 2:
                Tier2Cleaner++;
                break;
            case 3:
                Tier3Cleaner++;
                break;
        }
    }

    void Update()
    {
        TotalCleaning = (Tier1Cleaner * 1) + (Tier2Cleaner * 2) + (Tier3Cleaner * 3);
    }
}
