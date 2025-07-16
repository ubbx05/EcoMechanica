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

        // Ba�lang�� sprite'�n� ayarla
        if (normalSprite != null && spriteRenderer != null)
            spriteRenderer.sprite = normalSprite;

        // Bina t�r�ne g�re �zel ayarlar
        SetupBuildingType();

        Debug.Log($"{buildingName} binas� ba�lat�ld�!");
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
        description = "Hammadde ��kar�r";
        productionSpeed = 2f;
    }

    void SetupFurnace()
    {
        buildingName = "Furnace";
        description = "Malzemeleri i�ler";
        productionSpeed = 1.5f;
    }

    void SetupAssembler()
    {
        buildingName = "Assembler";
        description = "Par�alar� birle�tirir";
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
        description = "Malzemeleri ta��r";
        productionSpeed = 5f;
    }

    void SetupDefaultBuilding()
    {
        buildingName = "Unknown Building";
        description = "Bilinmeyen bina";
        productionSpeed = 1f;
    }

    // Bina se�imi
    public void SelectBuilding()
    {
        if (isSelected) return;

        isSelected = true;

        // Se�ili sprite'� g�ster
        if (selectedSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = selectedSprite;
        }
        else if (spriteRenderer != null)
        {
            // Sprite yoksa renk de�i�tir
            spriteRenderer.color = Color.yellow;
        }

        // Event tetikle
        OnBuildingSelected?.Invoke(this);

        Debug.Log($"{buildingName} se�ildi!");
    }

    public void DeselectBuilding()
    {
        if (!isSelected) return;

        isSelected = false;

        // Normal sprite'a d�nd�r
        if (normalSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = normalSprite;
        }

        // Orijinal renge d�nd�r
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        // Event tetikle
        OnBuildingDeselected?.Invoke(this);

        Debug.Log($"{buildingName} se�imi kald�r�ld�!");
    }

    // �retim ba�lat
    public void StartProduction()
    {
        if (!isOperational || isWorking) return;

        isWorking = true;

        // �al��ma efekti
        if (workingEffect != null)
            workingEffect.SetActive(true);

        // �retim coroutine ba�lat
        workingCoroutine = StartCoroutine(ProductionCoroutine());

        Debug.Log($"{buildingName} �retim ba�lad�!");
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

        // �al��ma efektini kald�r
        if (workingEffect != null)
            workingEffect.SetActive(false);

        Debug.Log($"{buildingName} �retim durduruldu!");
    }

    IEnumerator ProductionCoroutine()
    {
        while (isWorking && isOperational)
        {
            // �retim s�resi (h�z ile ters orant�l�)
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
        // �retim tamamland�
        Debug.Log($"{buildingName} �retim tamamland�!");

        // Event tetikle
        OnProductionComplete?.Invoke(this);

        // Burada spesifik bina t�r�ne g�re �retim mant��� eklenebilir
        HandleProductionByType();
    }

    void HandleProductionByType()
    {
        switch (buildingType)
        {
            case BuildingType.Extractor:
                // Hammadde ��kar
                Debug.Log("Hammadde ��kar�ld�!");
                break;
            case BuildingType.Furnace:
                // Malzeme i�le
                Debug.Log("Malzeme i�lendi!");
                break;
            case BuildingType.Assembler:
                // Par�a birle�tir
                Debug.Log("Par�a birle�tirildi!");
                break;
            case BuildingType.Cleaner:
                // Malzeme temizle
                Debug.Log("Malzeme temizlendi!");
                break;
            case BuildingType.ConveyorBelt:
                // Malzeme ta��
                Debug.Log("Malzeme ta��nd�!");
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

        Debug.Log($"{buildingName} �al��ma durumu: {isOperational}");
    }
}

// Bina t�rleri enum
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