using System;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    Boolean isEmpty = true;  // Başlangıçta boş olmalı
    private GameObject resourcePrefab;
    public int giris = 0;
    public int cikis = 0;
    private float speed = 1.5f;  // Hareket hızını artırdık
    private RotatingBuildings rotator;
    private int yon;
    private ConveyorBelt nextConveyorBelt;
    bool flag1 = true;
    bool flag2 = true;
    bool flag3 = true;
    bool flag4 = true;

    void Start()
    {
        // Belt'i kesinlikle boş yap
        isEmpty = true;
        resourcePrefab = null;

        determineNextConveyorBelt();
        if (rotator == null)
        {
            rotator = GetComponent<RotatingBuildings>();
        }
        yon = rotator.transferyonu;

        // Extractor'dan gelen Resource Type Action'ını dinle
        Extractor.OnResourceSpawned += HandleResourceSpawned;
    }

    void OnDestroy()
    {
        // Memory leak'i önlemek için Action'dan aboneliği kaldır
        Extractor.OnResourceSpawned -= HandleResourceSpawned;
    }

    // Extractor'dan gelen resource spawn bildirimini resource type bilgisi ile işle
    private void HandleResourceSpawned(GameObject resourcePrefab, Vector3 beltPosition, ResourceType resourceType)
    {
        // Bu conveyor belt'in üzerinde mi spawn olacak kontrol et
        float distance = Vector3.Distance(transform.position, beltPosition);

        // Tolerance mesafesi içinde ve belt boşsa resource'ı spawn et
        if (distance < 0.8f && isEmpty)
        {
            // ConveyorBelt kendisi spawn yapıyor
            Vector3 spawnPosition = new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.z - 0.1f
            );

            // Resource'ı spawn et
            GameObject spawnedResource = Instantiate(resourcePrefab, spawnPosition, Quaternion.identity);

            // Rigidbody2D ekle ve kinematic yap
            Rigidbody2D rb = spawnedResource.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = spawnedResource.AddComponent<Rigidbody2D>();
            }
            rb.bodyType = RigidbodyType2D.Kinematic;

            // Resource'ı belt'e ata ve belt'i dolu yap
            this.resourcePrefab = spawnedResource;  // Spawn edilen objeyi ata, parametre olarak gelen prefab'ı değil!
            isEmpty = false;

            // Resource tipine göre özel işlemler
            HandleSpecificResourceType(resourceType, spawnedResource);
        }
    }

    // Resource tipine göre özel işlemler
    private void HandleSpecificResourceType(ResourceType resourceType, GameObject resource)
    {
        // Gelecekte resource tipine göre özel işlemler buraya eklenebilir
        // Örnek: ses efektleri, partikel efektleri, sayaçlar vb.
    }

    // Update is called once per frame  
    void Update()
    {
        determineNextConveyorBelt();
        transportingRes();
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
                if (flag3 == true)
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
                if (flag1 == true && flag4 == true)
                {
                    Debug.Log("diger belt bulunamadi");
                    flag1 = false;
                }
            }
        }
        else
        {
            if (flag2 == true)
            {
                flag2 = false;
                Debug.Log("diger hit bulunamadi");
            }
        }
    }

    public void transportingRes()
    {
        // Belt boş değilse ve resource varsa hareket ettir
        if (!isEmpty && resourcePrefab != null)
        {
            // Yön kontrolü - rotator varsa yönü güncelle
            if (rotator != null)
            {
                yon = rotator.transferyonu;
            }

            // Rigidbody2D ile fizik tabanlı hareket
            Rigidbody2D rb = resourcePrefab.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Hareket yönünü hesapla
                Vector2 moveDirection = Vector2.zero;
                switch (yon)
                {
                    case 0: // Sağ
                        moveDirection = Vector2.right;
                        break;
                    case 1: // Yukarı
                        moveDirection = Vector2.up;
                        break;
                    case 2: // Sol
                        moveDirection = Vector2.left;
                        break;
                    case 3: // Aşağı
                        moveDirection = Vector2.down;
                        break;
                }

                // Hedef pozisyonda ConveyorBelt var mı kontrol et
                Vector3 targetPos = resourcePrefab.transform.position + (Vector3)(moveDirection * speed * Time.deltaTime);

                if (IsConveyorBeltAtPosition(targetPos))
                {
                    // Fizik tabanlı hareket - linearVelocity kullan (Unity 2023+)
                    rb.linearVelocity = moveDirection * speed;
                }
                else
                {
                    // Hedef pozisyonda conveyor belt yoksa dur
                    rb.linearVelocity = Vector2.zero;

                    // Sonraki belt'e transfer dene
                    if (nextConveyorBelt != null && nextConveyorBelt.isEmpty)
                    {
                        float distanceFromCenter = Vector3.Distance(resourcePrefab.transform.position, transform.position);
                        if (distanceFromCenter > 0.4f)
                        {
                            TransferResourceToNextBelt();
                        }
                    }
                }
            }
        }
    }

    // Belirli pozisyonda ConveyorBelt var mı kontrol et
    private bool IsConveyorBeltAtPosition(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(position);

        foreach (Collider2D collider in colliders)
        {
            // ConveyorBelt tag'i veya ismi kontrolü
            if (collider.CompareTag("ConveyorBelt") ||
                collider.name.Contains("ConveyorBelt") ||
                collider.name.Contains("ConveyorBeltPng"))
            {
                return true;
            }
        }

        return false;
    }

    private void CheckTransferToNextBelt()
    {
        // Bu fonksiyon artık transportingRes() içinde kullanılıyor
        // Ayrı çağrılmasına gerek yok
    }

    private void TransferResourceToNextBelt()
    {
        if (nextConveyorBelt != null && nextConveyorBelt.isEmpty && resourcePrefab != null)
        {
            // Resource'ın linearVelocity'sini sıfırla
            Rigidbody2D rb = resourcePrefab.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            // Resource'ı sonraki belt'e transfer et
            nextConveyorBelt.resourcePrefab = resourcePrefab;
            nextConveyorBelt.isEmpty = false;

            // Resource'ın pozisyonunu sonraki belt'in merkezine ayarla
            resourcePrefab.transform.position = new Vector3(
                nextConveyorBelt.transform.position.x,
                nextConveyorBelt.transform.position.y,
                nextConveyorBelt.transform.position.z - 0.1f
            );

            // Bu belt'i boşalt
            resourcePrefab = null;
            isEmpty = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Bu metod artık sadece manuel resource yerleştirme için kullanılacak
        // Extractor'dan gelen resource'lar Action ile yönetiliyor
        if (isEmpty && !collision.CompareTag("Extractor"))
        {
            resourcePrefab = collision.gameObject;
            isEmpty = false;
            Debug.Log("Resource manually added to conveyor belt: " + resourcePrefab.name);
        }
        else if (!isEmpty)
        {
            Debug.LogWarning("Conveyor belt is already occupied by another resource.");
        }
    }
}