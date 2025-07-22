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

    // Zamanlama i�in de�i�kenler
    private float lastUpdateTime;
    private float updateInterval = 5f; // 5 saniye

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastUpdateTime = Time.time; // �lk zaman� kaydet

        TotalExtractorPollution = calculatingTotalExcPol.TotalExtractorPollution;
        TotalAssemberPollution = calculatingTotalAsembPol.TotalAssemblerPollution;
        TotalCleaning = calculatingCleaning.TotalCleaning;
        totalWorkshopPollution = CalculatingTotalWorksopPol.TotalWorkshopPol;
        totalFurnacePollution = CalculatingTotalFurnacePol.TotalFurnacePol;
    }

    // Update is called once per frame
    void Update()
    {
        // Kirlilik de�erlerini her frame g�ncelle (h�zl� de�i�ebilir)
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

            lastUpdateTime = Time.time; // Son g�ncelleme zaman�n� kaydet
        }

        // Oyun biti� kontrol� her frame'de �al��s�n (daha responsive)
        if (EcoSystemReliability <= 0)
        {
            Debug.Log("Game Over: Ecosystem Reliability has reached zero.");
            Application.Quit();
        }
    }

    // Mevcut EcoSystemReliability de�erini almak i�in public metod
    public int GetEcoSystemReliability()
    {
        return EcoSystemReliability;
    }

    // Update interval'�n� de�i�tirmek i�in metod (iste�e ba�l�)
    public void SetUpdateInterval(float newInterval)
    {
        updateInterval = newInterval;
    }
}