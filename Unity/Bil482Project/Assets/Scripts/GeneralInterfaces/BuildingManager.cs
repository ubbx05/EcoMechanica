using System;
using UnityEngine;
using Unity.Mathematics;

public class BuildingManager : MonoBehaviour
{
    public static event Action<int> OnAssemblerPlaced;
    public static event Action<int> OnExtractorPlaced;
    public static event Action<int> OnCleanerPlaced;
    public static event Action<int> OnFurnacePlaced;
    public static event Action<int> OnWorkshopPlaced;
    public static event Action<int> OnConveyorBeltPlaced;

    public static GameObject SelectedPrefab;
    private GameObject previewInstance;

    public static int SelectedTier = 1;
    [SerializeField]public RotatingBuildings rotator;
    private int yon;
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
    public void PlaceFurnace (int tier, Vector3 position, GameObject prefab)
    {
        Instantiate(prefab, position, Quaternion.identity);
        OnFurnacePlaced?.Invoke(tier); // <-- Olay tetikleniyor
    }
    public void PlaceWorkshop(int tier, Vector3 position, GameObject prefab)
    {
        Instantiate(prefab, position, Quaternion.identity);
        OnWorkshopPlaced?.Invoke(tier); // <-- Olay tetikleniyor
    }
    public void PlaceConveyorBelt(int tier, Vector3 position, GameObject prefab)
    {
        Instantiate(prefab, position, Quaternion.identity);
        OnConveyorBeltPlaced?.Invoke(tier); // <-- Olay tetikleniyor
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        

    }

    // Update is called once per frame
    void Update()
    {
        if (previewInstance != null)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider != null)
            {
                Vector3 placePos = hit.collider.transform.position;
                previewInstance.transform.position = placePos;
                
                // Q ve E tuþlarýyla preview'ý döndür
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    previewInstance.transform.Rotate(0, 0, 90f);
                    
                    rotator.SetTransferYonu(rotator.GetTransferYonu() + 1);
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    previewInstance.transform.Rotate(0, 0, -90f);
                    rotator.SetTransferYonu(rotator.GetTransferYonu() - 1);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    // WorldToTile metoduyla koordinat bul
                    Vector2Int tilePos = TileManager.Instance.WorldToTile(placePos);

                    if (!TileManager.Instance.IsOccupied(tilePos))
                    {
                        Instantiate(SelectedPrefab, placePos, previewInstance.transform.rotation);
                        TileManager.Instance.MarkOccupied(tilePos);

                        Destroy(previewInstance);
                        previewInstance = null;
                        SelectedPrefab = null;
                    }
                    else
                    {
                        Debug.Log("Bu tile zaten dolu!");
                    }
                }
            }
        }
    }

    public void SetSelectedPrefab(GameObject prefab)
    {
        SelectedPrefab = prefab;

        if (previewInstance != null)
            Destroy(previewInstance);

        previewInstance = Instantiate(prefab);
    }
}
