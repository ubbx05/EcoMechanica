using System;
using System.Collections;
using UnityEngine;

public class Extractor : MonoBehaviour
{
    // Inspector'dan atanacak prefab'lar  
    [Header("Resource Prefabs")]
    public GameObject hamBakirPrefab;
    public GameObject hamDemirPrefab;
    public GameObject odunPrefab;

    [Header("Extraction Settings")]
    [SerializeField] private float extractionInterval = 4f; // 4 saniyede bir

    // Conveyor Belt'e bildirim göndermek için Action - Resource type bilgisi ile
    // Action parametreleri: (ResourcePrefab, BeltPosition, ResourceType)
    public static Action<GameObject, Vector3, ResourceType> OnResourceSpawned;

    private ExtractingStrategy extractingStrategy;
    private GameObject currentResource; // Üzerinde bulunduğu kaynak
    private Coroutine extractionCoroutine;
    private RotatingBuildings rotator;
    private int yon;
    private bool flag = false;

    void Start()
    {
        rotator = GetComponent<RotatingBuildings>();
        if (rotator != null)
        {
            yon = rotator.GetTransferYonu();
        }

        // BoxCollider2D kontrolü
        if (GetComponent<BoxCollider2D>() == null)
        {
            Debug.LogWarning("Extractor needs a BoxCollider2D component!");
        }

        // Prefab kontrolü
        if (hamBakirPrefab == null || hamDemirPrefab == null || odunPrefab == null)
        {
            Debug.LogError("Please assign all resource prefabs in the Inspector!");
        }

        // İlk başta hangi kaynağın üzerindeyse onu belirle
        DetermineExtractionStrategy();
    }

    void Update()
    {
        // Her frame'de pozisyon ve yön kontrolü
        DetermineExtractionStrategy();
        if (rotator != null)
        {
            yon = rotator.GetTransferYonu();
        }
    }

    // Tag'lere göre strategy mapping
    private void DetermineExtractionStrategy()
    {
        // Extractor'ın altında hangi resource olduğunu kontrol et
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, GetComponent<BoxCollider2D>().size, 0f);

        foreach (Collider2D collider in colliders)
        {
            // Bronze (Bakır) madeni kontrolü
            if (collider.CompareTag("Bakir"))
            {
                if (extractingStrategy == null || !(extractingStrategy is BronzeExtractingStrategy))
                {
                    SetNewStrategy(new BronzeExtractingStrategy(this, hamBakirPrefab), collider.gameObject);
                    if (!flag)
                    {
                        flag = true;
                        Debug.Log("Copper extraction strategy set");
                    }
                }
                return;
            }
            // Iron (Demir) madeni kontrolü
            else if (collider.CompareTag("Demir"))
            {
                if (extractingStrategy == null || !(extractingStrategy is IronExtractingStrategy))
                {
                    SetNewStrategy(new IronExtractingStrategy(this, hamDemirPrefab), collider.gameObject);
                    if (!flag)
                    {
                        flag = true;
                        Debug.Log("Iron extraction strategy set");
                    }
                }
                return;
            }
            // Wood (Odun) kontrolü
            else if (collider.CompareTag("Agac"))
            {
                if (extractingStrategy == null || !(extractingStrategy is WoodExtractingStrategy))
                {
                    SetNewStrategy(new WoodExtractingStrategy(this, odunPrefab), collider.gameObject);
                    if (!flag)
                    {
                        flag = true;
                        Debug.Log("Wood extraction strategy set");
                    }
                }
                return;
            }
        }

        // Hiçbir kaynak bulunamadıysa
        if (extractingStrategy != null)
        {
            StopExtraction();
            extractingStrategy = null;
            currentResource = null;
            flag = false;
        }
    }

    private void SetNewStrategy(ExtractingStrategy newStrategy, GameObject resource)
    {
        StopExtraction();
        extractingStrategy = newStrategy;
        currentResource = resource;
        StartExtraction();
    }

    private void StartExtraction()
    {
        if (extractionCoroutine != null)
        {
            StopCoroutine(extractionCoroutine);
        }
        extractionCoroutine = StartCoroutine(ExtractionRoutine());
    }

    private void StopExtraction()
    {
        if (extractionCoroutine != null)
        {
            StopCoroutine(extractionCoroutine);
            extractionCoroutine = null;
        }
    }

    private IEnumerator ExtractionRoutine()
    {
        while (extractingStrategy != null)
        {
            yield return new WaitForSeconds(extractionInterval);
            ExtractResource();
        }
    }

    public void ExtractResource()
    {
        if (extractingStrategy != null)
        {
            extractingStrategy.extract();
        }
        else
        {
            Debug.LogWarning("No resource found to extract!");
        }
    }

    // Resource spawn metodu - strategy'ler tarafından çağrılacak
    // Sadece conveyor belt'i bulur ve Action ile bildirim gönderir, spawn işlemini ConveyorBelt yapar
    public void SpawnResource(GameObject resourcePrefab)
    {
        if (resourcePrefab != null)
        {
            // Yöne göre kontrol pozisyonunu hesapla
            Vector3 checkPosition = GetCheckPosition();

            // DEBUG: Kontrol pozisyonunu yazdır
            Debug.Log($"🔍 Extractor at: {transform.position}, Direction: {yon}, Check Position: {checkPosition}");

            // O pozisyonda conveyor belt var mı kontrol et
            Collider2D conveyorBelt = GetConveyorBeltAtPosition(checkPosition);

            if (conveyorBelt != null)
            {
                // Conveyor belt bulundu! Action ile bildirim gönder
                Vector3 targetPosition = conveyorBelt.transform.position;
                ResourceType resourceType = DetermineResourceType(resourcePrefab);

                // Action'ı tetikle - ConveyorBelt bu bildirimle spawn işlemini yapacak
                OnResourceSpawned?.Invoke(resourcePrefab, targetPosition, resourceType);

                Debug.Log($"📡 Extractor found conveyor belt! Sending spawn request...");
                Debug.Log($"📦 Resource Type: {resourceType}, Target Belt Position: {targetPosition}");
                Debug.Log($"🎯 Extractor: {gameObject.name}, Direction: {yon}");
            }
            else
            {
                // Conveyor belt bulunamadı - DEBUG bilgilerini artır
                Debug.LogWarning($"⚠️ No conveyor belt found at direction {yon}! Resource not spawned.");
                Debug.LogWarning($"🔍 Extractor position: {transform.position}");
                Debug.LogWarning($"🔍 Checked position: {checkPosition}");
                Debug.LogWarning($"🔍 Search area: {new Vector2(0.5f, 0.5f)}");

                // Çevredeki tüm collider'ları listele
                Collider2D[] allColliders = Physics2D.OverlapBoxAll(checkPosition, new Vector2(1f, 1f), 0f);
                Debug.LogWarning($"🔍 Found {allColliders.Length} colliders in larger area:");
                foreach (Collider2D col in allColliders)
                {
                    Debug.LogWarning($"   - {col.name} (Tag: {col.tag}) at {col.transform.position}");
                }
            }
        }
        else
        {
            Debug.LogError("❌ ResourcePrefab is null! Cannot send spawn request.");
        }
    }

    // Resource tipini prefab'e göre belirle
    private ResourceType DetermineResourceType(GameObject prefab)
    {
        ResourceType resourceType = ResourceType.hamDemir;
        if (prefab == hamBakirPrefab)
            resourceType = ResourceType.hamBakir;
        if (prefab == hamDemirPrefab)
            resourceType = ResourceType.hamDemir;
        if (prefab == odunPrefab)
            resourceType = ResourceType.Wood;
        return resourceType;
    }

    // Yöne göre kontrol edilecek pozisyonu hesapla
    private Vector3 GetCheckPosition()
    {
        Vector3 offset = Vector3.zero;

        switch (yon)
        {
            case 0: // Sağ
                offset = new Vector3(0.75f, 0f, 0f);
                break;
            case 1: // Yukarı
                offset = new Vector3(0f, 0.75f, 0f);
                break;
            case 2: // Sol
                offset = new Vector3(-0.75f, 0f, 0f);
                break;
            case 3: // Aşağı
                offset = new Vector3(0f, -0.75f, 0f);
                break;
            default:
                Debug.LogError("Wrong direction input");
                break;
        }

        return transform.position + offset;
    }

    // Belirli pozisyonda conveyor belt var mı kontrol et
    private Collider2D GetConveyorBeltAtPosition(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(position, new Vector2(0.5f, 0.5f), 0f);

        foreach (Collider2D collider in colliders)
        {
            // Tag kontrolü - eğer tag yoksa isim kontrolü yap
            if (collider.CompareTag("ConveyorBelt") ||
                collider.name.Contains("ConveyorBelt") ||
                collider.name.Contains("ConveyorBeltPng"))
            {
                Debug.Log($"✅ Found conveyor belt: {collider.name} (Tag: {collider.tag})");
                return collider;
            }
        }

        return null;
    }

    // Trigger based detection alternatifi (2D)
    void OnTriggerEnter2D(Collider2D other)
    {
        // Bu metod gerekirse kullanılabilir
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Bu metod gerekirse kullanılabilir
    }
}