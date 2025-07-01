using UnityEngine;

public class CalculatingTotalAsembPol : MonoBehaviour
{
    public int TotalAssemblerPollution;
    public int Tier1Ass;
    public int Tier2Ass;
    public int Tier3Ass;

    void OnEnable()
    {
        BuildingManager.OnAssemblerPlaced += HandleMachinePlaced;
    }

    void OnDisable()
    {
        BuildingManager.OnAssemblerPlaced -= HandleMachinePlaced;
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
        TotalAssemblerPollution = (Tier1Ass * 1) + (Tier2Ass * 2) + (Tier3Ass * 3);
    }
}
