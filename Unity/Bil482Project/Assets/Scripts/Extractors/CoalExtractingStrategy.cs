using UnityEngine;

public class CoalExtractingStrategy : ExtractingStrategy
{
    private Extractor extractor;
    private GameObject coalPrefab;

    public CoalExtractingStrategy(Extractor extractor, GameObject prefab)
    {
        this.extractor = extractor;
        this.coalPrefab = prefab;

        if (coalPrefab == null)
        {
            Debug.LogError("Coal prefab is not assigned!");
        }
    }

    public void extract()
    {
        extractor.SpawnResource(coalPrefab);
    }
}