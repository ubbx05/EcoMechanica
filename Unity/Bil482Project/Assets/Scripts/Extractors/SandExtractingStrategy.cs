using UnityEngine;

public class SandExtractingStrategy : ExtractingStrategy
{
    private Extractor extractor;
    private GameObject sandPrefab;

    public SandExtractingStrategy(Extractor extractor, GameObject prefab)
    {
        this.extractor = extractor;
        this.sandPrefab = prefab;

        if (sandPrefab == null)
        {
            Debug.LogError("Sand prefab is not assigned!");
        }
    }

    public void extract()
    {
        extractor.SpawnResource(sandPrefab);
    }
}