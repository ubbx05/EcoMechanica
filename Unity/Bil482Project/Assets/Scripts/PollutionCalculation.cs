using UnityEngine;

public class PollutionCalculation : MonoBehaviour
{
    public CalculatingTotalExcPol calculatingTotalExcPol;
    public CalculatingTotalAsembPol calculatingTotalAsembPol;
    public CalculatingCleaning calculatingCleaning;
    int TotalExtractorPollution;
    int TotalAssemberPollution;
    int TotalCleaning;
    private int EcoSystemReliability = 100000;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        int TotalExtractorPollution = calculatingTotalExcPol.TotalExtractorPollution;
        int TotalAssemberPollution = calculatingTotalAsembPol.TotalAssemblerPollution;
        int TotalCleaning = calculatingCleaning.TotalCleaning;
    }

    // Update is called once per frame
    void Update()
    {
        TotalExtractorPollution = calculatingTotalExcPol.TotalExtractorPollution;
        TotalAssemberPollution = calculatingTotalAsembPol.TotalAssemblerPollution;
        TotalCleaning = calculatingCleaning.TotalCleaning;
        EcoSystemReliability -= (TotalExtractorPollution + TotalAssemberPollution - TotalCleaning);
        if(EcoSystemReliability <= 0)
        {
            //oyunu bitir
            Debug.Log("Game Over: Ecosystem Reliability has reached zero.");
            Application.Quit();
        }
        else
        {
            //Debug.Log("Ecosystem Reliability: " + EcoSystemReliability);
        }
    }
}
