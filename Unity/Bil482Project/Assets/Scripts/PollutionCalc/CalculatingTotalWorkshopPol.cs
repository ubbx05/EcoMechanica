using UnityEngine;

public class CalculatingTotalWorksopPol : MonoBehaviour
{
    public int TotalWorkshopPol = 0;
    public int Tier1Ass;
    public int Tier2Ass;
    public int Tier3Ass;

    void OnEnable()
    {
        BuildingManager.OnWorkshopPlaced += HandleMachinePlaced;
    }

    void OnDisable()
    {
        BuildingManager.OnWorkshopPlaced -= HandleMachinePlaced;
    }

    void HandleMachinePlaced(int tier)
    {
        switch (tier)
        {
            case 1:
                Tier1Ass++;
                break;
            case 2:
                Tier2Ass++;
                break;
            case 3:
                Tier3Ass++;
                break;
        }
    }

    void Update()
    {
        TotalWorkshopPol = (Tier1Ass * 1) + (Tier2Ass * 2) + (Tier3Ass * 3);
    }
}
