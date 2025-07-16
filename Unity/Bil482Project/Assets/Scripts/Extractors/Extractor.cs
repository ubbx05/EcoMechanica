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

    private ExtractingStrategy extractingStrategy;
    private GameObject currentResource; // Üzerinde bulunduðu kaynak
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

        // Ýlk baþta hangi kaynaðýn üzerindeyse onu belirle
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
        // Extractor'ýn altýnda hangi resource olduðunu kontrol et
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, GetComponent<BoxCollider2D>().size, 0f);

        foreach (Collider2D collider in colliders)
        {
            // Bronze (Bakýr) madeni kontrolü
            if (collider.CompareTag("Bakir"))
            {
                if (extractingStrategy == null || !(extractingStrategy is BronzeExtractingStrategy))
                {
                    SetNewStrategy(new BronzeExtractingStrategy(this, hamBakirPrefab), collider.gameObject);
                    if (!flag)
                    {
                        flag = true;
                        Debug.Log("Bronze extraction strategy set");
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

        // Hiçbir kaynak bulunamadýysa
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

    // Resource spawn metodu - strategy'ler tarafýndan çaðrýlacak
    public void SpawnResource(GameObject resourcePrefab)
    {
        if (resourcePrefab != null)
        {
            // Yöne göre kontrol pozisyonunu hesapla
            Vector3 checkPosition = GetCheckPosition();

            // O pozisyonda conveyor belt var mý kontrol et
            Collider2D conveyorBelt = GetConveyorBeltAtPosition(checkPosition);

            Vector3 spawnPosition;

            if (conveyorBelt != null)
            {
                // Conveyor belt varsa onun üzerinde spawn et
                spawnPosition = new Vector3(
                    conveyorBelt.transform.position.x,
                    conveyorBelt.transform.position.y,
                    conveyorBelt.transform.position.z - 0.1f
                );
            }
            else
            {
                // Conveyor belt yoksa normal pozisyonda spawn et
                spawnPosition = checkPosition;
            }

            // Resource'ý spawn et
            GameObject spawnedResource = Instantiate(resourcePrefab, spawnPosition, Quaternion.identity);

            // Rigidbody2D ekle
            Rigidbody2D rb = spawnedResource.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = spawnedResource.AddComponent<Rigidbody2D>();
            }

            // Conveyor belt üzerindeyse kinematic yap
            if (conveyorBelt != null)
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    // Yöne göre kontrol edilecek pozisyonu hesapla
    private Vector3 GetCheckPosition()
    {
        Vector3 offset = Vector3.zero;

        switch (yon)
        {
            case 0: // Sað
                offset = new Vector3(0.75f, 0f, 0f);
                break;
            case 1: // Yukarý
                offset = new Vector3(0f, 0.75f, 0f);
                break;
            case 2: // Sol
                offset = new Vector3(-0.75f, 0f, 0f);
                break;
            case 3: // Aþaðý
                offset = new Vector3(0f, -0.75f, 0f);
                break;
        }

        return transform.position + offset;
    }

    // Belirli pozisyonda conveyor belt var mý kontrol et
    private Collider2D GetConveyorBeltAtPosition(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(position, new Vector2(0.5f, 0.5f), 0f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("ConveyorBelt"))
            {
                return collider;
            }
        }

        return null;
    }

    // Trigger based detection alternatifi (2D)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bakir"))
        {
            SetNewStrategy(new BronzeExtractingStrategy(this, hamBakirPrefab), other.gameObject);
        }
        else if (other.CompareTag("Demir"))
        {
            SetNewStrategy(new IronExtractingStrategy(this, hamDemirPrefab), other.gameObject);
        }
        else if (other.CompareTag("Agac"))
        {
            SetNewStrategy(new WoodExtractingStrategy(this, odunPrefab), other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == currentResource)
        {
            StopExtraction();
            extractingStrategy = null;
            currentResource = null;
            flag = false;
        }
    }

    // Manual strategy set (GameController'dan kullanýlabilir)
    public void SetExtractingStrategy(ExtractingStrategy strategy)
    {
        StopExtraction();
        this.extractingStrategy = strategy;
        StartExtraction();
    }

    // Extraction interval'ini deðiþtirmek için public metod
    public void SetExtractionInterval(float newInterval)
    {
        extractionInterval = newInterval;
        // Eðer extraction devam ediyorsa, yeni interval ile yeniden baþlat
        if (extractingStrategy != null)
        {
            StartExtraction();
        }
    }

    void OnDestroy()
    {
        StopExtraction();
    }
}