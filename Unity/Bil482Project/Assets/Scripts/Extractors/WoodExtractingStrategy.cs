using UnityEngine;

public class WoodExtractingStrategy : ExtractingStrategy
{
    private Extractor extractor;
    private GameObject odunPrefab;

    public WoodExtractingStrategy(Extractor extractor, GameObject prefab)
    {
        this.extractor = extractor;
        this.odunPrefab = prefab;

        if (odunPrefab == null)
        {
            //Debug.LogError("Odun prefab is not assigned!");
        }
    }

    public void extract()
    {
        //Debug.Log("1 Wood extracted");
        extractor.SpawnResource(odunPrefab);
    }
}