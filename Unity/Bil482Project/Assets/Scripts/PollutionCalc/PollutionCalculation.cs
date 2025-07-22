using UnityEngine;

public class PollutionCalculation : MonoBehaviour
{
    public CalculatingTotalExcPol calculatingTotalExcPol;
    public CalculatingTotalAsembPol calculatingTotalAsembPol;
    public CalculatingCleaning calculatingCleaning;
    public CalculatingTotalWorksopPol CalculatingTotalWorksopPol;
    public CalculatingTotalFurnacePol CalculatingTotalFurnacePol;

    int TotalExtractorPollution;
    int TotalAssemberPollution;
    int TotalCleaning;
    int totalWorkshopPollution;
    int totalFurnacePollution;
    private int EcoSystemReliability = 100000;

    // Zamanlama için deðiþkenler
    private float lastUpdateTime;
    private float updateInterval = 5f; // 5 saniye

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastUpdateTime = Time.time; // Ýlk zamaný kaydet

        TotalExtractorPollution = calculatingTotalExcPol.TotalExtractorPollution;
        TotalAssemberPollution = calculatingTotalAsembPol.TotalAssemblerPollution;
        TotalCleaning = calculatingCleaning.TotalCleaning;
        totalWorkshopPollution = CalculatingTotalWorksopPol.TotalWorkshopPol;
        totalFurnacePollution = CalculatingTotalFurnacePol.TotalFurnacePol;
    }

    // Update is called once per frame
    void Update()
    {
        // Kirlilik deðerlerini her frame güncelle (hýzlý deðiþebilir)
        TotalExtractorPollution = calculatingTotalExcPol.TotalExtractorPollution;
        TotalAssemberPollution = calculatingTotalAsembPol.TotalAssemblerPollution;
        TotalCleaning = calculatingCleaning.TotalCleaning;
        totalWorkshopPollution = CalculatingTotalWorksopPol.TotalWorkshopPol;
        totalFurnacePollution = CalculatingTotalFurnacePol.TotalFurnacePol;

        // Sadece 5 saniyede bir EcoSystemReliability'yi azalt
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            int pollutionDifference = (TotalExtractorPollution + totalFurnacePollution + totalWorkshopPollution + TotalAssemberPollution - TotalCleaning);
            EcoSystemReliability -= pollutionDifference;

            // Debug bilgisi
            Debug.Log($"Ecosystem Reliability Updated: {EcoSystemReliability} (Changed by: -{pollutionDifference})");
            Debug.Log($"Pollution Breakdown - Extractor: {TotalExtractorPollution}, Assembler: {TotalAssemberPollution}, Workshop: {totalWorkshopPollution}, Furnace: {totalFurnacePollution}, Cleaning: {TotalCleaning}");

            lastUpdateTime = Time.time; // Son güncelleme zamanýný kaydet
        }

        // Oyun bitiþ kontrolü her frame'de çalýþsýn (daha responsive)
        if (EcoSystemReliability <= 0)
        {
            Debug.Log("Game Over: Ecosystem Reliability has reached zero.");
            Application.Quit();
        }
    }

    // Mevcut EcoSystemReliability deðerini almak için public metod
    public int GetEcoSystemReliability()
    {
        return EcoSystemReliability;
    }

    // Update interval'ýný deðiþtirmek için metod (isteðe baðlý)
    public void SetUpdateInterval(float newInterval)
    {
        updateInterval = newInterval;
    }
}