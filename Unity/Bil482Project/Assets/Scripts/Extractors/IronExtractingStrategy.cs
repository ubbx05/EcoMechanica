using UnityEngine;

public class IronExtractingStrategy : ExtractingStrategy
{
    private Extractor extractor;
    private GameObject hamDemirPrefab;

    public IronExtractingStrategy(Extractor extractor, GameObject prefab)
    {
        this.extractor = extractor;
        this.hamDemirPrefab = prefab;

        if (hamDemirPrefab == null)
        {
           // Debug.LogError("HamDemir prefab is not assigned!");
        }
    }

    public void extract()
    {
        //Debug.Log("1 Iron extracted");
        extractor.SpawnResource(hamDemirPrefab);
    }
}