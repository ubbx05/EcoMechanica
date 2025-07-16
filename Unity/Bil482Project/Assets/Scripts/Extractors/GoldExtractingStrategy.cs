using UnityEngine;

public class GoldExtractingStrategy : ExtractingStrategy
{
    private Extractor extractor;
    private GameObject goldPrefab;

    public GoldExtractingStrategy(Extractor extractor, GameObject prefab)
    {
        this.extractor = extractor;
        this.goldPrefab = prefab;

        if (goldPrefab == null)
        {
            Debug.LogError("Gold prefab is not assigned!");
        }
    }

    public void extract()
    {
        extractor.SpawnResource(goldPrefab);
    }
}