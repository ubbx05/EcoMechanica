using UnityEngine;

public class StoneExtractingStrategy : ExtractingStrategy
{
    private Extractor extractor;
    private GameObject stonePrefab;

    public StoneExtractingStrategy(Extractor extractor, GameObject prefab)
    {
        this.extractor = extractor;
        this.stonePrefab = prefab;

        if (stonePrefab == null)
        {
            Debug.LogError("Stone prefab is not assigned!");
        }
    }

    public void extract()
    {
        extractor.SpawnResource(stonePrefab);
    }
}