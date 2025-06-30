using UnityEngine;

public class CalculatingTotalAsembPol : MonoBehaviour
{
    public int TotalAssemblerPollution;
    public int Tier1Ext;
    public int Tier2Ext;
    public int Tier3Ext;

    void OnEnable()
    {
        //MachineManager.OnMachinePlaced += HandleMachinePlaced;
    }

    void OnDisable()
    {
        //MachineManager.OnMachinePlaced -= HandleMachinePlaced;
    }

    void HandleMachinePlaced(int tier)
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
        TotalAssemblerPollution = (Tier1Ext * 1) + (Tier2Ext * 2) + (Tier3Ext * 3);
    }
}
