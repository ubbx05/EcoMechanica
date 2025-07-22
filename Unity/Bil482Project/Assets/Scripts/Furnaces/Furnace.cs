using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour
{
    private float furnaceTimer = 0f;
    [SerializeField] private float furnacingInterval = 4f;

    private FurnacingStrategy currentStrategy;
    private RotatingBuildings rotator;
    private int yon;
    private bool isFurnacing = false;
    public int envanter = 0;

    [Header("Input Resource Prefabs")]
    [SerializeField] public GameObject hamDemirPrefab;
    [SerializeField] public GameObject hamBakirPrefab;

    [Header("Output Resource Prefabs")]
    [SerializeField] public GameObject ironore_prefab;
    [SerializeField] public GameObject copperore_prefab;

    [Header("Furnace Output Buffer")]
    public Queue<GameObject> outputBuffer = new Queue<GameObject>();
    private float retryTimer = 0f;
    private float retryDelay = 1f; // 1 saniyede bir tekrar dene

    void Start()
    {
        rotator = GetComponent<RotatingBuildings>();
        if (rotator != null)
        {
            yon = rotator.GetTransferYonu();
        }

        // DEBUG: Prefab'ları kontrol et
        Debug.Log($"Furnace Start - ironore_prefab: {ironore_prefab?.name ?? "NULL"}");
        Debug.Log($"Furnace Start - copperore_prefab: {copperore_prefab?.name ?? "NULL"}");
    }

    void Update()
    {
        // Her frame'de pozisyon ve yön kontrolü
        if (rotator != null)
        {
            yon = rotator.GetTransferYonu();
        }

        // Üretim kontrolü
        if (isFurnacing)
        {
            furnaceTimer += Time.deltaTime;

            if (furnaceTimer >= furnacingInterval)
            {
                isFurnacing = false;
                furnaceTimer = 0f;

                Debug.Log($"Furnacing completed! Strategy: {currentStrategy?.GetType().Name}");
                Debug.Log($"Envanter before production: {envanter}");

                // Üretimi yap - ürünü buffer'a ekle
                currentStrategy?.furnace(null, this);

                // Envanterden resource'ları düş
                envanter -= currentStrategy.neededAmount;

                Debug.Log($"Envanter after production: {envanter}");
                Debug.Log("Furnacing cycle finished");
            }
        }

        // Buffer'daki ürünleri ConveyorBelt'e göndermeye çalış
        retryTimer += Time.deltaTime;
        if (retryTimer >= retryDelay && outputBuffer.Count > 0)
        {
            retryTimer = 0f;
            TryToSendBufferedProducts();
        }
    }

    public void SetStrategy(FurnacingStrategy strategy)
    {
        currentStrategy = strategy;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null || collision.gameObject == null)
        {
            return;
        }

        GameObject incomingResource = collision.gameObject;
        Debug.Log($"Furnace received: {incomingResource.name}");

        // ConveyorBelt'ten teleport edilen resource'lar için özel kontrol
        TeleportedResource teleportMarker = incomingResource.GetComponent<TeleportedResource>();
        if (teleportMarker != null && teleportMarker.isTeleportedFromConveyorBelt)
        {
            Debug.Log("Teleported resource detected - skipping direction check");
        }
        else
        {
            // Normal resource'lar için yön kontrolü yap
            Vector3 expectedEntryDirection = GetExpectedEntryDirection();
            Vector3 actualDirection = (incomingResource.transform.position - transform.position).normalized;

            if (Vector3.Dot(expectedEntryDirection, actualDirection) < 0.7f)
            {
                Debug.Log($"Wrong direction - Expected: {expectedEntryDirection}, Actual: {actualDirection}");
                Destroy(incomingResource);
                return;
            }
            Debug.Log("Direction check passed for normal resource");
        }

        bool resourceAccepted = false;

        // Resource türüne göre stratejiyi belirle - Workshop'taki gibi
        if (incomingResource.name.Contains("Demir") || incomingResource.name.Contains("Iron") || incomingResource.name.Contains("HamDemir"))
        {
            resourceAccepted = true;
            if (currentStrategy is IronFurnacingStrategy)
            {
                envanter++;
                Debug.Log($"Iron ore added to inventory. Total: {envanter}");
            }
            else
            {
                envanter = 1;
                Debug.Log($"Setting Iron strategy - ironore_prefab: {ironore_prefab?.name ?? "NULL"}");
                SetStrategy(new IronFurnacingStrategy());
                Debug.Log("Iron furnacing strategy set");
            }
        }
        else if (incomingResource.name.Contains("Bakir") || incomingResource.name.Contains("Copper") || incomingResource.name.Contains("HamBakir"))
        {
            resourceAccepted = true;
            if (currentStrategy is CopperFurnacingStrategy)
            {
                envanter++;
                Debug.Log($"Copper ore added to inventory. Total: {envanter}");
            }
            else
            {
                envanter = 1;
                Debug.Log($"Setting Copper strategy - copperore_prefab: {copperore_prefab?.name ?? "NULL"}");
                SetStrategy(new CopperFurnacingStrategy());
                Debug.Log("Copper furnacing strategy set");
            }
        }

        if (resourceAccepted)
        {
            // Üretimi başlat
            if (envanter >= currentStrategy.neededAmount && !isFurnacing)
            {
                isFurnacing = true;
                furnaceTimer = 0f;
                Debug.Log($"Furnacing started! Need {currentStrategy.neededAmount}, have {envanter}");
            }
            else if (!isFurnacing)
            {
                Debug.Log($"Waiting for more resources. Need {currentStrategy.neededAmount}, have {envanter}");
            }
        }

        // Resource'ı her durumda sil
        Destroy(incomingResource);
        Debug.Log($"Resource consumed: {incomingResource.name}");
    }

    // Buffer'daki ürünleri göndermeye çalış
    private void TryToSendBufferedProducts()
    {
        if (outputBuffer.Count > 0)
        {
            GameObject productToSend = outputBuffer.Peek();
            Debug.Log($"Trying to send buffered product: {productToSend.name}");

            if (TrySpawnResource(productToSend))
            {
                outputBuffer.Dequeue();
                Debug.Log($"Buffered product {productToSend.name} sent! Buffer size: {outputBuffer.Count}");
            }
            else
            {
                Debug.Log($"ConveyorBelt still occupied, keeping {productToSend.name} in buffer");
            }
        }
    }

    public void SpawnResource(GameObject resourcePrefab)
    {
        Debug.Log($"Furnace.SpawnResource called with: {resourcePrefab?.name ?? "NULL"}");

        if (resourcePrefab != null)
        {
            outputBuffer.Enqueue(resourcePrefab);
            Debug.Log($"Product {resourcePrefab.name} added to buffer. Buffer size: {outputBuffer.Count}");
        }
        else
        {
            Debug.LogError("Furnace: ResourcePrefab is null!");
        }
    }

    private bool TrySpawnResource(GameObject resourcePrefab)
    {
        if (resourcePrefab != null)
        {
            Vector3 checkPosition = GetCheckPosition();
            Debug.Log($"Trying to spawn at direction {yon}, position: {checkPosition}");

            Collider2D conveyorBelt = GetConveyorBeltAtPosition(checkPosition);

            if (conveyorBelt != null)
            {
                ConveyorBelt beltComponent = conveyorBelt.GetComponent<ConveyorBelt>();
                if (beltComponent != null && beltComponent.isEmpty)
                {
                    Vector3 targetPosition = conveyorBelt.transform.position;

                    Debug.Log($"Furnace found EMPTY conveyor belt!");
                    Debug.Log($"Sending resource: {resourcePrefab.name}");

                    // Direkt conveyor belt'e spawn et
                    Vector3 spawnPosition = new Vector3(
                        targetPosition.x,
                        targetPosition.y,
                        targetPosition.z - 1f
                    );

                    GameObject spawnedResource = Instantiate(resourcePrefab, spawnPosition, Quaternion.identity);
                    beltComponent.resourcePrefab = spawnedResource;
                    beltComponent.isEmpty = false;
                    beltComponent.ResetTeleportTimer();

                    Debug.Log($"Furnace spawned resource directly on belt!");
                    return true;
                }
                else
                {
                    Debug.Log($"ConveyorBelt found but OCCUPIED");
                    return false;
                }
            }
            else
            {
                Debug.LogWarning($"No conveyor belt found at direction {yon}");
                return false;
            }
        }
        return false;
    }

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
        }

        return transform.position + offset;
    }

    private Collider2D GetConveyorBeltAtPosition(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(position, new Vector2(0.5f, 0.5f), 0f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("ConveyorBelt") ||
                collider.name.Contains("ConveyorBelt") ||
                collider.name.Contains("ConveyorBeltPng"))
            {
                Debug.Log($"Furnace found conveyor belt: {collider.name}");
                return collider;
            }
        }

        return null;
    }

    private Vector3 GetExpectedEntryDirection()
    {
        switch (yon)
        {
            case 0: return Vector3.left;   // Çıkış sağ → giriş sol
            case 1: return Vector3.down;   // Çıkış yukarı → giriş aşağı
            case 2: return Vector3.right;  // Çıkış sol → giriş sağ
            case 3: return Vector3.up;     // Çıkış aşağı → giriş yukarı
            default:
                return Vector3.zero;
        }
    }
}