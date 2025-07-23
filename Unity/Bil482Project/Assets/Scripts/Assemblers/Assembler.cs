using System.Collections.Generic;
using UnityEngine;
using System;

public class Assembler : MonoBehaviour
{
    private float assemblyTimer = 0f;
    [SerializeField] private float assemblyInterval = 6f; // Workshop ve Furnace'den biraz daha uzun

    private AssemblyStrategy currentStrategy;
    private RotatingBuildings rotator;
    private int yon;
    private bool isAssembling = false;

    // İki farklı ürün için ayrı envanterler - genel amaçlı
    public int productA_envanter = 0; // İlk ürün tipi
    public int productB_envanter = 0; // İkinci ürün tipi

    [Header("Input Resource Prefabs - All Types")]
    [SerializeField] public GameObject demirIngotPrefab;
    [SerializeField] public GameObject magnetPrefab;
    [SerializeField] public GameObject circuitBoardPrefab;
    [SerializeField] public GameObject steelPrefab;

    [Header("Output Resource Prefabs")]
    [SerializeField] public GameObject computer_prefab; // Computer ürünü
    [SerializeField] public GameObject circuitBoard_prefab; // CircuitBoard ürünü

    [Header("Assembler Output Buffer")]
    public Queue<GameObject> outputBuffer = new Queue<GameObject>();
    private float retryTimer = 0f;
    private float retryDelay = 1f;

    // Assembler'dan ConveyorBelt'e bildirim göndermek için Action
    public static Action<GameObject, Vector3, ResourceType> OnAssemblerResourceSpawned;

    void Start()
    {
        rotator = GetComponent<RotatingBuildings>();
        if (rotator != null)
        {
            yon = rotator.GetTransferYonu();
        }

        // Varsayılan stratejiyi ayarla - başlangıçta boş
        currentStrategy = null;

        Debug.Log($"Assembler Start - computer_prefab: {computer_prefab?.name ?? "NULL"}");
        Debug.Log($"Assembler Start - circuitBoard_prefab: {circuitBoard_prefab?.name ?? "NULL"}");
    }

    void Update()
    {
        // Her frame'de pozisyon ve yön kontrolü
        if (rotator != null)
        {
            yon = rotator.GetTransferYonu();
        }

        // Montaj kontrolü
        if (isAssembling)
        {
            assemblyTimer += Time.deltaTime;

            if (assemblyTimer >= assemblyInterval)
            {
                isAssembling = false;
                assemblyTimer = 0f;

                Debug.Log($"Assembly completed! Strategy: {currentStrategy?.GetType().Name}");
                Debug.Log($"ProductA Envanter before assembly: {productA_envanter}");
                Debug.Log($"ProductB Envanter before assembly: {productB_envanter}");

                // Üretimi yap - ürünü buffer'a ekle - TUTARLI METOD İSMİ
                currentStrategy?.Produce(null, this);

                // Envanterlerden resource'ları düş
                productA_envanter -= currentStrategy.neededProductA;
                productB_envanter -= currentStrategy.neededProductB;

                Debug.Log($"ProductA Envanter after assembly: {productA_envanter}");
                Debug.Log($"ProductB Envanter after assembly: {productB_envanter}");
                Debug.Log("Assembly cycle finished");
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

    public void SetStrategy(AssemblyStrategy strategy)
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
        Debug.Log($"Assembler received: {incomingResource.name}");

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

        // Resource türüne göre stratejiyi belirle ve envantere ekle - Workshop'taki gibi
        if (incomingResource.name.Contains("DemirIngot") || incomingResource.name.Contains("IronIngot") || incomingResource.name.Contains("Iron"))
        {
            resourceAccepted = true;
            if (currentStrategy is CircuitBoardAssembleStrategy)
            {
                productA_envanter++;
                Debug.Log($"DemirIngot added for CircuitBoard production. Current productA_envanter: {productA_envanter}");
            }
            else
            {
                // Yeni gelen DemirIngot, CircuitBoard üretimi için strateji ayarla
                SetStrategy(new CircuitBoardAssembleStrategy());
                productA_envanter++;
                Debug.Log($"Strategy set to CircuitBoard production. DemirIngot added: {productA_envanter}");
            }
        }
        else if (incomingResource.name.Contains("Magnet") || incomingResource.name.Contains("Mıknatıs"))
        {
            resourceAccepted = true;
            if (currentStrategy is CircuitBoardAssembleStrategy)
            {
                productB_envanter++;
                Debug.Log($"Magnet added for CircuitBoard production. Current productB_envanter: {productB_envanter}");
            }
            else
            {
                // Yeni gelen Magnet, CircuitBoard üretimi için strateji ayarla
                SetStrategy(new CircuitBoardAssembleStrategy());
                productB_envanter++;
                Debug.Log($"Strategy set to CircuitBoard production. Magnet added: {productB_envanter}");
            }
        }
        else if (incomingResource.name.Contains("CircuitBoard") || incomingResource.name.Contains("Devre"))
        {
            resourceAccepted = true;
            if (currentStrategy is ComputerAssembleStrategy)
            {
                productA_envanter++;
                Debug.Log($"CircuitBoard added for Computer production. Current productA_envanter: {productA_envanter}");
            }
            else
            {
                // Yeni gelen CircuitBoard, Computer üretimi için strateji ayarla
                SetStrategy(new ComputerAssembleStrategy());
                productA_envanter++;
                Debug.Log($"Strategy set to Computer production. CircuitBoard added: {productA_envanter}");
            }
        }
        else if (incomingResource.name.Contains("Steel") || incomingResource.name.Contains("Çelik"))
        {
            resourceAccepted = true;
            if (currentStrategy is ComputerAssembleStrategy)
            {
                productB_envanter++;
                Debug.Log($"Steel added for Computer production. Current productB_envanter: {productB_envanter}");
            }
            else
            {
                // Yeni gelen Steel, Computer üretimi için strateji ayarla
                SetStrategy(new ComputerAssembleStrategy());
                productB_envanter++;
                Debug.Log($"Strategy set to Computer production. Steel added: {productB_envanter}");
            }
        }

        if (resourceAccepted)
        {
            Destroy(incomingResource);

            // Her iki üründen de yeterli miktarda varsa montajı başlat
            if (!isAssembling && currentStrategy != null &&
                productA_envanter >= currentStrategy.neededProductA &&
                productB_envanter >= currentStrategy.neededProductB)
            {
                isAssembling = true;
                assemblyTimer = 0f;

                if (currentStrategy is CircuitBoardAssembleStrategy)
                {
                    Debug.Log("🔧 CircuitBoard Assembly started - DemirIngot + Magnet available!");
                }
                else if (currentStrategy is ComputerAssembleStrategy)
                {
                    Debug.Log("🔧 Computer Assembly started - CircuitBoard + Steel available!");
                }
            }
        }
        else
        {
            Debug.Log($"❌ Resource rejected: {incomingResource.name}");
            Destroy(incomingResource);
        }
    }

    private Vector3 GetExpectedEntryDirection()
    {
        // Giriş yönünü hesapla (rotasyon tersi)
        switch (yon % 4)
        {
            case 0: return Vector3.left;   // Sağa dönük -> soldan giriş
            case 1: return Vector3.down;   // Yukarı dönük -> aşağıdan giriş
            case 2: return Vector3.right;  // Sola dönük -> sağdan giriş
            case 3: return Vector3.up;     // Aşağı dönük -> yukarıdan giriş
            default: return Vector3.left;
        }
    }

    // ===== Assembler.cs - Çözüm 1 Uygulaması =====

    // TryToSendBufferedProducts metodunu şu şekilde güncelleyin:
    private void TryToSendBufferedProducts()
    {
        if (outputBuffer.Count == 0) return;

        GameObject productToSend = outputBuffer.Peek();
        Vector3 outputPosition = GetOutputPosition();

        // İlk belt'i bul
        Collider2D firstBelt = GetConveyorBeltAtPosition(outputPosition);

        if (firstBelt != null)
        {
            ConveyorBelt beltComponent = firstBelt.GetComponent<ConveyorBelt>();

            // İlk belt boşsa Action ile gönder
            if (beltComponent != null && beltComponent.isEmpty)
            {
                ResourceType resourceType = DetermineResourceType(productToSend);
                OnAssemblerResourceSpawned?.Invoke(productToSend, outputPosition, resourceType);

                outputBuffer.Dequeue();
                Debug.Log($"🔧 Assembler found empty belt, sending: {productToSend.name}");
            }
            else
            {
                // İlk belt doluysa, belt zincirinin kendisi halleder
                Debug.Log("🔧 Assembler: First belt occupied, waiting for chain to clear...");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Assembler: No conveyor belt found at output direction");
        }
    }

    // Yeni metod: ConveyorBelt bulma (diğer machine'lerle aynı)
    private Collider2D GetConveyorBeltAtPosition(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(position, new Vector2(0.5f, 0.5f), 0f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("ConveyorBelt") ||
                collider.name.Contains("ConveyorBelt") ||
                collider.name.Contains("ConveyorBeltPng"))
            {
                Debug.Log($"🔧 Assembler found conveyor belt: {collider.name}");
                return collider;
            }
        }

        return null;
    }

    // GetOutputPosition metodunu güncelleme (0.75f offset ekle)
    private Vector3 GetOutputPosition()
    {
        Vector3 offset = Vector3.zero;
        switch (yon % 4)
        {
            case 0: // Sağa
                offset = new Vector3(0.75f, 0f, 0f);
                break;
            case 1: // Yukarı
                offset = new Vector3(0f, 0.75f, 0f);
                break;
            case 2: // Sola
                offset = new Vector3(-0.75f, 0f, 0f);
                break;
            case 3: // Aşağı
                offset = new Vector3(0f, -0.75f, 0f);
                break;
        }
        return transform.position + offset;
    }

    private ResourceType DetermineResourceType(GameObject product)
    {
        ResourceType sonuc = ResourceType.CopperOre;
        if (product.name.Contains("Computer") || product.name.Contains("Bilgisayar"))
            sonuc =  ResourceType.Computer;
        else if (product.name.Contains("CircuitBoard") || product.name.Contains("Devre"))
            sonuc =  ResourceType.CircuitBoard;

        return sonuc ;
    }

    // Public metotlar - dış erişim için
    public int GetProductACount() => productA_envanter;
    public int GetProductBCount() => productB_envanter;
    public bool IsAssembling() => isAssembling;
    public int GetBufferCount() => outputBuffer.Count;

    // Workshop/Furnace gibi SpawnResource metodu
    public void SpawnResource(GameObject resourcePrefab)
    {
        Debug.Log($"🔧 Assembler.SpawnResource called with: {resourcePrefab?.name ?? "NULL"}");

        if (resourcePrefab != null)
        {
            Vector3 outputPosition = transform.position;
            outputPosition.z -= 1f; // Görünür olması için

            GameObject assembledProduct = Instantiate(resourcePrefab, outputPosition, Quaternion.identity);

            // TeleportedResource komponenti ekle
            TeleportedResource teleportComponent = assembledProduct.AddComponent<TeleportedResource>();
            teleportComponent.isTeleportedFromConveyorBelt = false;

            outputBuffer.Enqueue(assembledProduct);

            string productType = resourcePrefab == computer_prefab ? "Computer" :
                               resourcePrefab == circuitBoard_prefab ? "CircuitBoard" : "Product";

            Debug.Log($"🎯 {productType} created and added to buffer. Buffer size: {outputBuffer.Count}");
        }
        else
        {
            Debug.LogError("Assembler: ResourcePrefab is null!");
        }
    }
}