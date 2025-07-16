using UnityEngine;
using System.Collections;

public class BuildingController : MonoBehaviour
{
    [Header("Building Information")]
    public string buildingName;
    public BuildingType buildingType;
    public string description;

    [Header("Building Stats")]
    public float productionSpeed = 1f;
    public bool isOperational = true;

    [Header("Visual Components")]
    public SpriteRenderer spriteRenderer;
    public Sprite normalSprite;
    public Sprite selectedSprite;

    [Header("Building States")]
    public bool isSelected = false;
    public bool isWorking = false;

    [Header("Effects")]
    public GameObject workingEffect;

    // Events
    public System.Action<BuildingController> OnBuildingSelected;
    public System.Action<BuildingController> OnBuildingDeselected;
    public System.Action<BuildingController> OnProductionComplete;

    // Private variables
    private Color originalColor;
    private Coroutine workingCoroutine;

    void Start()
    {
        InitializeBuilding();
    }

    void InitializeBuilding()
    {
        // Sprite renderer bul
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Orijinal rengi kaydet
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        // Baþlangýç sprite'ýný ayarla
        if (normalSprite != null && spriteRenderer != null)
            spriteRenderer.sprite = normalSprite;

        // Bina türüne göre özel ayarlar
        SetupBuildingType();

        Debug.Log($"{buildingName} binasý baþlatýldý!");
    }

    void SetupBuildingType()
    {
        switch (buildingType)
        {
            case BuildingType.Extractor:
                SetupExtractor();
                break;
            case BuildingType.Furnace:
                SetupFurnace();
                break;
            case BuildingType.Assembler:
                SetupAssembler();
                break;
            case BuildingType.Cleaner:
                SetupCleaner();
                break;
            case BuildingType.ConveyorBelt:
                SetupConveyorBelt();
                break;
            default:
                SetupDefaultBuilding();
                break;
        }
    }

    void SetupExtractor()
    {
        buildingName = "Extractor";
        description = "Hammadde çýkarýr";
        productionSpeed = 2f;
    }

    void SetupFurnace()
    {
        buildingName = "Furnace";
        description = "Malzemeleri iþler";
        productionSpeed = 1.5f;
    }

    void SetupAssembler()
    {
        buildingName = "Assembler";
        description = "Parçalarý birleþtirir";
        productionSpeed = 1f;
    }

    void SetupCleaner()
    {
        buildingName = "Cleaner";
        description = "Malzemeleri temizler";
        productionSpeed = 3f;
    }

    void SetupConveyorBelt()
    {
        buildingName = "Conveyor Belt";
        description = "Malzemeleri taþýr";
        productionSpeed = 5f;
    }

    void SetupDefaultBuilding()
    {
        buildingName = "Unknown Building";
        description = "Bilinmeyen bina";
        productionSpeed = 1f;
    }

    // Bina seçimi
    public void SelectBuilding()
    {
        if (isSelected) return;

        isSelected = true;

        // Seçili sprite'ý göster
        if (selectedSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = selectedSprite;
        }
        else if (spriteRenderer != null)
        {
            // Sprite yoksa renk deðiþtir
            spriteRenderer.color = Color.yellow;
        }

        // Event tetikle
        OnBuildingSelected?.Invoke(this);

        Debug.Log($"{buildingName} seçildi!");
    }

    public void DeselectBuilding()
    {
        if (!isSelected) return;

        isSelected = false;

        // Normal sprite'a döndür
        if (normalSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = normalSprite;
        }

        // Orijinal renge döndür
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        // Event tetikle
        OnBuildingDeselected?.Invoke(this);

        Debug.Log($"{buildingName} seçimi kaldýrýldý!");
    }

    // Üretim baþlat
    public void StartProduction()
    {
        if (!isOperational || isWorking) return;

        isWorking = true;

        // Çalýþma efekti
        if (workingEffect != null)
            workingEffect.SetActive(true);

        // Üretim coroutine baþlat
        workingCoroutine = StartCoroutine(ProductionCoroutine());

        Debug.Log($"{buildingName} üretim baþladý!");
    }

    public void StopProduction()
    {
        if (!isWorking) return;

        isWorking = false;

        // Coroutine durdur
        if (workingCoroutine != null)
        {
            StopCoroutine(workingCoroutine);
            workingCoroutine = null;
        }

        // Çalýþma efektini kaldýr
        if (workingEffect != null)
            workingEffect.SetActive(false);

        Debug.Log($"{buildingName} üretim durduruldu!");
    }

    IEnumerator ProductionCoroutine()
    {
        while (isWorking && isOperational)
        {
            // Üretim süresi (hýz ile ters orantýlý)
            float productionTime = 1f / productionSpeed;

            yield return new WaitForSeconds(productionTime);

            if (isWorking)
            {
                CompleteProduction();
            }
        }
    }

    void CompleteProduction()
    {
        // Üretim tamamlandý
        Debug.Log($"{buildingName} üretim tamamlandý!");

        // Event tetikle
        OnProductionComplete?.Invoke(this);

        // Burada spesifik bina türüne göre üretim mantýðý eklenebilir
        HandleProductionByType();
    }

    void HandleProductionByType()
    {
        switch (buildingType)
        {
            case BuildingType.Extractor:
                // Hammadde çýkar
                Debug.Log("Hammadde çýkarýldý!");
                break;
            case BuildingType.Furnace:
                // Malzeme iþle
                Debug.Log("Malzeme iþlendi!");
                break;
            case BuildingType.Assembler:
                // Parça birleþtir
                Debug.Log("Parça birleþtirildi!");
                break;
            case BuildingType.Cleaner:
                // Malzeme temizle
                Debug.Log("Malzeme temizlendi!");
                break;
            case BuildingType.ConveyorBelt:
                // Malzeme taþý
                Debug.Log("Malzeme taþýndý!");
                break;
        }
    }

    // Bina bilgilerini al
    public BuildingInfo GetBuildingInfo()
    {
        return new BuildingInfo
        {
            name = buildingName,
            type = buildingType,
            description = description,
            productionSpeed = productionSpeed,
            isOperational = isOperational,
            isWorking = isWorking,
            isSelected = isSelected
        };
    }

    public void ToggleOperational()
    {
        isOperational = !isOperational;

        if (!isOperational)
        {
            StopProduction();
        }

        Debug.Log($"{buildingName} çalýþma durumu: {isOperational}");
    }
}

// Bina türleri enum
public enum BuildingType
{
    Extractor,
    Furnace,
    Assembler,
    Cleaner,
    ConveyorBelt,
    Workshop,
    Factory,
    Storage,
    Other
}

// Bina bilgisi struct
[System.Serializable]
public struct BuildingInfo
{
    public string name;
    public BuildingType type;
    public string description;
    public float productionSpeed;
    public bool isOperational;
    public bool isWorking;
    public bool isSelected;
}