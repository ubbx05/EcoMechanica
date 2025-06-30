using UnityEngine;

public class PollutionCalculation : MonoBehaviour
{
    CalculatingTotalExcPol calculatingTotalExcPol;
    CalculatingTotalAsembPol calculatingTotalAsembPol;
    CalculatingCleaning calculatingCleaning;
    public int TotalExtractorPollution;
    public int TotalAssemberPollution;
    public int TotalCleaning;
    private int EcoSystemReliability = 100000;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Use GetComponent to reference the CalculatingTotalExcPol component attached to the same GameObject
        
    }

    // Update is called once per frame
    void Update()
    {
        TotalExtractorPollution = calculatingTotalExcPol.TotalExtractorPollution;
        TotalAssemberPollution = calculatingTotalAsembPol.TotalAssemblerPollution;
        TotalCleaning = calculatingCleaning.TotalCleaning;
        EcoSystemReliability -= (TotalExtractorPollution + TotalAssemberPollution - TotalCleaning);
    }
}
