using UnityEngine;

public class BronzeExtractingStrategy : ExtractingStrategy
{
    private Extractor extractor;
    private GameObject hamBakirPrefab;

    public BronzeExtractingStrategy(Extractor extractor, GameObject prefab)
    {
        this.extractor = extractor;
        this.hamBakirPrefab = prefab;

        if (hamBakirPrefab == null)
        {
            Debug.LogError("HamBakir prefab is not assigned!");
        }
    }

    public void extract()
    {
        Debug.Log("1 Bronze extracted");
        extractor.SpawnResource(hamBakirPrefab);
    }

    
}