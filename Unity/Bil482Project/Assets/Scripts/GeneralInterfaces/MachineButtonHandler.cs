using UnityEngine;

public class MachineButtonHandler : MonoBehaviour
{
    public CalculatingCleaning calculatingCleaning;
    public CalculatingTotalAsembPol calculatingTotalAsembPol;
    public CalculatingTotalExcPol calculatingTotalExcPol;
    public GameObject Assemblertier1Prefab;
    public GameObject Assemblertier2Prefab;
    public GameObject Assemblertier3Prefab;
    public GameObject extractorTier1Prefab;
    public GameObject extractorTier2Prefab;
    public GameObject extractorTier3Prefab;
    public GameObject cleanerTier1Prefab;
    public GameObject cleanerTier2Prefab;
    public GameObject cleanerTier3Prefab;
    public BuildingManager buildingManager;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    public void OnClickPlaceCleanerTier1()
    {
        buildingManager.PlaceCleaner(1, new Vector3(10, 0, 0), cleanerTier1Prefab);
    }

    public void OnClickPlaceCleanerTier2()
    {
        buildingManager.PlaceCleaner(2, new Vector3(11, 0, 0), cleanerTier2Prefab);
    }

    public void OnClickPlaceCleanerTier3()
    {
        buildingManager.PlaceCleaner(3, new Vector3(12, 0, 0), cleanerTier3Prefab);
    }
    public void OnClickPlaceExtractorTier1()
    {
        buildingManager.PlaceExtractor(1, new Vector3(5, 0, 0), extractorTier1Prefab);
    }

    public void OnClickPlaceExtractorTier2()
    {
        buildingManager.PlaceExtractor(2, new Vector3(6, 0, 0), extractorTier2Prefab);
    }

    public void OnClickPlaceExtractorTier3()
    {
        buildingManager.PlaceExtractor(3, new Vector3(7, 0, 0), extractorTier3Prefab);
    }

    public void OnClickPlaceTier1()
    {
        buildingManager.PlaceAssembler(1, new Vector3(0, 0, 0), Assemblertier1Prefab);
    }

    public void OnClickPlaceTier2()
    {
        buildingManager.PlaceAssembler(2, new Vector3(1, 0, 0), Assemblertier2Prefab);
    }

    public void OnClickPlaceTier3()
    {
        buildingManager.PlaceAssembler(3, new Vector3(2, 0, 0), Assemblertier3Prefab);
    }
}
