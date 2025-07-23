using System;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [Header("ConveyorBelt State")]
    public bool isEmpty = true;  // PUBLIC - Workshop/Furnace erişebilsin
    public GameObject resourcePrefab;
    public int giris = 0;
    public int cikis = 0;

    [Header("ConveyorBelt Settings")]
    private RotatingBuildings rotator;
    private int yon;
    private ConveyorBelt nextConveyorBelt;
    private Workshop targetWorkshop;
    private Furnace targetFurnace; // FURNACE DESTEĞİ EKLENDİ

    [Header("Teleportation Timer")]
    private float teleportTimer = 0f;
    private float teleportDelay = 2f; // 2 saniye bekleme
    private bool isReadyToTeleport = false;

    // Debug flags
    bool flag1 = true;
    bool flag2 = true;
    bool flag3 = true;
    bool flag4 = true;

    void Start()
    {
        // Belt'i boş yap
        isEmpty = true;
        resourcePrefab = null;

        // Bileşenleri al
        determineNextConveyorBelt();
        if (rotator == null)
        {
            rotator = GetComponent<RotatingBuildings>();
        }
        yon = rotator.transferyonu;

        // Action'ları dinle
        Extractor.OnResourceSpawned += HandleResourceSpawned;
        Workshop.OnWorkshopResourceSpawned += HandleWorkshopResourceSpawned;
        Assembler.OnAssemblerResourceSpawned += HandleAssemblerResourceSpawned; // YENİ EKLENEN
    }

    void OnDestroy()
    {
        // Memory leak önleme
        Extractor.OnResourceSpawned -= HandleResourceSpawned;
        Workshop.OnWorkshopResourceSpawned -= HandleWorkshopResourceSpawned;
        Assembler.OnAssemblerResourceSpawned -= HandleAssemblerResourceSpawned; // YENİ EKLENEN
    }

    void Update()
    {
        determineNextConveyorBelt();
        transportingRes();
    }

    // Extractor'dan gelen resource'ları işle
    private void HandleResourceSpawned(GameObject resourcePrefab, Vector3 beltPosition, ResourceType resourceType)
    {
        float distance = Vector3.Distance(transform.position, beltPosition);

        if (distance < 0.8f && isEmpty) // Mesafeyi 0.8'den 2.5'e çıkardık
        {
            Vector3 spawnPosition = new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.z - 1f  // Görünür olması için
            );

            GameObject spawnedResource = Instantiate(resourcePrefab, spawnPosition, Quaternion.identity);

            this.resourcePrefab = spawnedResource;
            isEmpty = false;
            ResetTeleportTimer();

            Debug.Log($"Resource {resourcePrefab.name} spawned from extractor");
        }
        else if (!isEmpty)
        {
            Debug.LogWarning($"ConveyorBelt occupied, cannot receive: {resourcePrefab.name}");
        }
        else
        {
            Debug.LogWarning($"Extractor resource too far: distance = {distance}");
        }
    }

    // Workshop'tan gelen resource'ları işle
    private void HandleWorkshopResourceSpawned(GameObject resourcePrefab, Vector3 beltPosition, ResourceType resourceType)
    {
        float distance = Vector3.Distance(transform.position, beltPosition);

        if (distance < 2.5f && isEmpty) // Mesafeyi 0.8'den 2.5'e çıkardık
        {
            Vector3 spawnPosition = new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.z - 1f  // Görünür olması için
            );

            GameObject spawnedResource = Instantiate(resourcePrefab, spawnPosition, Quaternion.identity);

            this.resourcePrefab = spawnedResource;
            isEmpty = false;
            ResetTeleportTimer();

            Debug.Log($"Workshop resource {resourcePrefab.name} spawned visibly");
        }
        else if (!isEmpty)
        {
            Debug.LogWarning($"ConveyorBelt occupied, cannot receive: {resourcePrefab.name}");
        }
        else
        {
            Debug.LogWarning($"Workshop resource too far: distance = {distance}");
        }
    }

    // Assembler'dan gelen resource'ları işle - YENİ EKLENEN
    private void HandleAssemblerResourceSpawned(GameObject resourcePrefab, Vector3 beltPosition, ResourceType resourceType)
    {
        float distance = Vector3.Distance(transform.position, beltPosition);

        if (distance < 2.5f && isEmpty) // Workshop ile aynı mesafe
        {
            Vector3 spawnPosition = new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.z - 1f  // Görünür olması için
            );

            GameObject spawnedResource = Instantiate(resourcePrefab, spawnPosition, Quaternion.identity);

            this.resourcePrefab = spawnedResource;
            isEmpty = false;
            ResetTeleportTimer();

            Debug.Log($"🔧 Assembler resource {resourcePrefab.name} spawned on belt");
        }
        else if (!isEmpty)
        {
            Debug.LogWarning($"⚠️ ConveyorBelt occupied, cannot receive assembler: {resourcePrefab.name}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Assembler resource too far: distance = {distance}");
        }
    }

    public void transportingRes()
    {
        if (resourcePrefab != null && !isEmpty)
        {
            // Timer kontrolü - 2 saniye bekleme
            teleportTimer += Time.deltaTime;

            if (!isReadyToTeleport)
            {
                if (teleportTimer >= teleportDelay)
                {
                    isReadyToTeleport = true;
                    Debug.Log($"Resource {resourcePrefab.name} ready for teleport after {teleportDelay}s");
                }
                else
                {
                    return; // Henüz hazır değil
                }
            }

            // Teleport hazır, hedef kontrol et
            if (nextConveyorBelt != null && nextConveyorBelt.isEmpty)
            {
                TeleportResourceToNextBelt();
            }
            else if (targetWorkshop != null)
            {
                TeleportResourceToWorkshop();
            }
            else if (targetFurnace != null) // FURNACE TELEPORT EKLENDİ
            {
                TeleportResourceToFurnace();
            }
        }
    }

    private void TeleportResourceToNextBelt()
    {
        if (nextConveyorBelt != null && nextConveyorBelt.isEmpty && resourcePrefab != null)
        {
            Debug.Log($"Teleporting to next belt: {resourcePrefab.name}");

            // Physics durdur
            Rigidbody2D rb = resourcePrefab.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            // Teleport et
            resourcePrefab.transform.position = new Vector3(
                nextConveyorBelt.transform.position.x,
                nextConveyorBelt.transform.position.y,
                nextConveyorBelt.transform.position.z - 1f
            );

            // SpriteRenderer görünür tut
            SpriteRenderer sr = resourcePrefab.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 10;
            }

            // Transfer et
            nextConveyorBelt.resourcePrefab = resourcePrefab;
            nextConveyorBelt.isEmpty = false;
            nextConveyorBelt.ResetTeleportTimer();

            // Bu belt'i boşalt
            resourcePrefab = null;
            isEmpty = true;

            Debug.Log("Resource teleported to next belt");
        }
    }

    private void TeleportResourceToWorkshop()
    {
        if (targetWorkshop != null && resourcePrefab != null)
        {
            Debug.Log($"Teleporting to workshop: {resourcePrefab.name}");

            GameObject resourceToTransfer = resourcePrefab;
            resourcePrefab = null;
            isEmpty = true;

            // Physics durdur
            Rigidbody2D rb = resourceToTransfer.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.bodyType = RigidbodyType2D.Static;
            }

            // Resource'ı ConveyorBelt'ten geldiğini işaretle
            TeleportedResource teleportMarker = resourceToTransfer.GetComponent<TeleportedResource>();
            if (teleportMarker == null)
            {
                teleportMarker = resourceToTransfer.AddComponent<TeleportedResource>();
            }
            teleportMarker.isTeleportedFromConveyorBelt = true;

            // Workshop'a teleport et
            resourceToTransfer.transform.position = new Vector3(
                targetWorkshop.transform.position.x,
                targetWorkshop.transform.position.y,
                targetWorkshop.transform.position.z - 1f
            );

            // SpriteRenderer görünür tut
            SpriteRenderer sr = resourceToTransfer.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 10;
            }

            // Collider trigger yap
            Collider2D resourceCollider = resourceToTransfer.GetComponent<Collider2D>();
            if (resourceCollider == null)
            {
                resourceCollider = resourceToTransfer.AddComponent<BoxCollider2D>();
            }
            resourceCollider.isTrigger = true;

            // Workshop trigger'ını çağır
            try
            {
                targetWorkshop.SendMessage("OnTriggerEnter2D", resourceCollider, SendMessageOptions.DontRequireReceiver);
                Debug.Log("Resource teleported to workshop");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Workshop trigger failed: {e.Message}");
                if (resourceToTransfer != null)
                {
                    Destroy(resourceToTransfer);
                }
            }
        }
    }

    // YENİ METOD - Furnace'a teleport
    private void TeleportResourceToFurnace()
    {
        if (targetFurnace != null && resourcePrefab != null)
        {
            Debug.Log($"Teleporting to furnace: {resourcePrefab.name}");

            GameObject resourceToTransfer = resourcePrefab;
            resourcePrefab = null;
            isEmpty = true;

            // Physics durdur
            Rigidbody2D rb = resourceToTransfer.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.bodyType = RigidbodyType2D.Static;
            }

            // Resource'ı ConveyorBelt'ten geldiğini işaretle
            TeleportedResource teleportMarker = resourceToTransfer.GetComponent<TeleportedResource>();
            if (teleportMarker == null)
            {
                teleportMarker = resourceToTransfer.AddComponent<TeleportedResource>();
            }
            teleportMarker.isTeleportedFromConveyorBelt = true;

            // Furnace'a teleport et
            resourceToTransfer.transform.position = new Vector3(
                targetFurnace.transform.position.x,
                targetFurnace.transform.position.y,
                targetFurnace.transform.position.z - 1f
            );

            // SpriteRenderer görünür tut
            SpriteRenderer sr = resourceToTransfer.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 10;
            }

            // Collider trigger yap
            Collider2D resourceCollider = resourceToTransfer.GetComponent<Collider2D>();
            if (resourceCollider == null)
            {
                resourceCollider = resourceToTransfer.AddComponent<BoxCollider2D>();
            }
            resourceCollider.isTrigger = true;

            // Furnace trigger'ını çağır
            try
            {
                targetFurnace.SendMessage("OnTriggerEnter2D", resourceCollider, SendMessageOptions.DontRequireReceiver);
                Debug.Log("Resource teleported to furnace");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Furnace trigger failed: {e.Message}");
                if (resourceToTransfer != null)
                {
                    Destroy(resourceToTransfer);
                }
            }
        }
    }

    public void ResetTeleportTimer()
    {
        teleportTimer = 0f;
        isReadyToTeleport = false;
        Debug.Log($"Timer reset - will wait {teleportDelay}s before moving");
    }

    private static readonly Vector2Int[] directionOffsets = new Vector2Int[]
    {
        new Vector2Int(1, 0),   // sağ
        new Vector2Int(0, 1),   // yukarı
        new Vector2Int(-1, 0),  // sol
        new Vector2Int(0, -1)   // aşağı
    };

    public void determineNextConveyorBelt()
    {
        Vector2Int offset = directionOffsets[yon % 4];
        Vector2 checkPosition = (Vector2)transform.position + offset;

        Collider2D hit = Physics2D.OverlapPoint(checkPosition);
        if (hit != null)
        {
            ConveyorBelt belt = hit.GetComponent<ConveyorBelt>();
            if (belt != null)
            {
                nextConveyorBelt = belt;
                targetWorkshop = null;
                targetFurnace = null; // FURNACE NULL YAP
                if (flag3)
                {
                    Debug.Log("Next conveyor belt found");
                    flag3 = false;
                    flag4 = false;
                }
                flag1 = true;
                flag2 = true;
            }
            else
            {
                Workshop workshop = hit.GetComponent<Workshop>();
                if (workshop != null)
                {
                    targetWorkshop = workshop;
                    nextConveyorBelt = null;
                    targetFurnace = null; // FURNACE NULL YAP
                    if (flag3)
                    {
                        Debug.Log("Target workshop found");
                        flag3 = false;
                        flag4 = false;
                    }
                    flag1 = true;
                    flag2 = true;
                }
                else
                {
                    // FURNACE KONTROLÜ EKLENDİ
                    Furnace furnace = hit.GetComponent<Furnace>();
                    if (furnace != null)
                    {
                        targetFurnace = furnace;
                        nextConveyorBelt = null;
                        targetWorkshop = null;
                        if (flag3)
                        {
                            Debug.Log("Target furnace found");
                            flag3 = false;
                            flag4 = false;
                        }
                        flag1 = true;
                        flag2 = true;
                    }
                    else
                    {
                        nextConveyorBelt = null;
                        targetWorkshop = null;
                        targetFurnace = null; // FURNACE NULL YAP
                        if (flag1 && flag4)
                        {
                            Debug.Log("No next target found");
                            flag1 = false;
                        }
                    }
                }
            }
        }
        else
        {
            nextConveyorBelt = null;
            targetWorkshop = null;
            targetFurnace = null; // FURNACE NULL YAP
            if (flag2)
            {
                flag2 = false;
                Debug.Log("No hit found");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Manuel resource yerleştirme için
        if (isEmpty &&
            !collision.CompareTag("Extractor") &&
            !collision.CompareTag("Workshop") &&
            !collision.CompareTag("Furnace") &&
            !collision.CompareTag("Assembler")) // YENİ EKLENEN
        {
            resourcePrefab = collision.gameObject;
            isEmpty = false;
            ResetTeleportTimer();
            Debug.Log("Resource manually added: " + resourcePrefab.name);
        }
        else if (!isEmpty)
        {
            Debug.LogWarning("ConveyorBelt already occupied");
        }
    }
}