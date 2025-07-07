using System;
using UnityEngine;

public class Extractor : MonoBehaviour // Context of Strategy Pattern
{
    // Inspector'dan atanacak prefab'lar
    [Header("Resource Prefabs")]
    public GameObject hamBakirPrefab;
    public GameObject hamDemirPrefab;
    public GameObject odunPrefab;

    private ExtractingStrategy extractingStrategy;
    private GameObject currentResource; // Üzerinde bulunduðu kaynak

    // Spawn offset - extractor'ýn saðýna spawn etmek için
    private Vector3 spawnOffset = new Vector3(1.5f, 0f, 0f);
    Boolean flag = false;

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
                extractingStrategy = new BronzeExtractingStrategy(this, hamBakirPrefab);
                currentResource = collider.gameObject;
                if(flag == false)
                {
                    flag = true;
                    Debug.Log("Bronze extraction strategy set");
                }

                return;
            }
            // Iron (Demir) madeni kontrolü
            else if (collider.CompareTag("Demir"))
            {
                extractingStrategy = new IronExtractingStrategy(this, hamDemirPrefab);
                currentResource = collider.gameObject;
                if (flag == false)
                {
                    flag = true;
                    Debug.Log("Iron extraction strategy set");
                }
                return;
            }
            // Wood (Odun) kontrolü
            else if (collider.CompareTag("Agac"))
            {
                extractingStrategy = new WoodExtractingStrategy(this, odunPrefab);
                currentResource = collider.gameObject;
                if (flag == false)
                {
                    flag = true;
                    Debug.Log("Wood extraction strategy set");
                }
                return;
            }
        }

        // Hiçbir kaynak bulunamadýysa
        extractingStrategy = null;
        currentResource = null;
    }

    void Start()
    {
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
        // Her frame'de pozisyon kontrolü (performans için belirli aralýklarla yapýlabilir)
        DetermineExtractionStrategy();
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
            Vector3 spawnPosition = transform.position + spawnOffset;
            GameObject spawnedResource = Instantiate(resourcePrefab, spawnPosition, Quaternion.identity);

            // Spawn edilen resource'a fizik ekle (düþmesi için)
            Rigidbody2D rb = spawnedResource.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = spawnedResource.AddComponent<Rigidbody2D>();
            }
        }
    }

    // Trigger based detection alternatifi (2D)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bakir") )
        {
            extractingStrategy = new BronzeExtractingStrategy(this, hamBakirPrefab);
            currentResource = other.gameObject;
        }
        else if (other.CompareTag("Demir") )
        {
            extractingStrategy = new IronExtractingStrategy(this, hamDemirPrefab);
            currentResource = other.gameObject;
        }
        else if (other.CompareTag("Agac") )
        {
            extractingStrategy = new WoodExtractingStrategy(this, odunPrefab);
            currentResource = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == currentResource)
        {
            extractingStrategy = null;
            currentResource = null;
        }
    }

    // Manual strategy set (GameController'dan kullanýlabilir)
    public void SetExtractingStrategy(ExtractingStrategy strategy)
    {
        this.extractingStrategy = strategy;
    }
}