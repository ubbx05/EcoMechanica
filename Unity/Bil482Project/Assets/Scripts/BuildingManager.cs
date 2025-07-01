using System;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static event Action<int> OnAssemblerPlaced;
    public static event Action<int> OnExtractorPlaced;
    public static event Action<int> OnCleanerPlaced; 

    public void PlaceAssembler(int tier, Vector3 position, GameObject prefab)
    {
        Instantiate(prefab, position, Quaternion.identity);
        OnAssemblerPlaced?.Invoke(tier);
    }

    public void PlaceExtractor(int tier, Vector3 position, GameObject prefab)
    {
        Instantiate(prefab, position, Quaternion.identity);
        OnExtractorPlaced?.Invoke(tier);
    }

    public void PlaceCleaner(int tier, Vector3 position, GameObject prefab)
    {
        Instantiate(prefab, position, Quaternion.identity);
        OnCleanerPlaced?.Invoke(tier); // <-- Olay tetikleniyor
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
