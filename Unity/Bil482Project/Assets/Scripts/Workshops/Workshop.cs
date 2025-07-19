using UnityEngine;
using UnityEngine.UIElements;

public class Workshop : Machine
{
    private float craftTimer = 0f;
    [SerializeField] private float craftingInterval = 4f;

    private IProductionStrategy currentStrategy;
    private RotatingBuildings rotator;
    private int yon;
    private bool isCrafting = false;
    public int envanter = 0;

    [Header("Input Resource Prefabs")]
    [SerializeField] public GameObject odunPrefab;
    [SerializeField] public GameObject bakirIngotPrefab;
    [SerializeField] public GameObject demirIngotPrefab;

    [Header("Output Resource Prefabs")]
    [SerializeField] private GameObject plankPrefab;
    [SerializeField] private GameObject copperWirePrefab;
    [SerializeField] private GameObject steelPrefab;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GetComponent<BoxCollider2D>() != null)
        {
            Debug.LogWarning("Needs empty space");
        }

        rotator = GetComponent<RotatingBuildings>();
        if (rotator != null)
        {
            yon = rotator.GetTransferYonu();
        }
    }

    void Update()
    {
        // Her frame'de pozisyon ve y�n kontrol�
        if (rotator != null)
        {
            yon = rotator.GetTransferYonu();
        }

        if (isCrafting)
        {
            craftTimer += Time.deltaTime;

            if (craftTimer >= craftingInterval)
            {
                isCrafting = false;
                craftTimer = 0f;

                currentStrategy?.Produce(null, this);
                envanter -= currentStrategy.neededAmount;
                Debug.Log("4 saniye sonra üretim gerçekleşti.");
            }
        }
    }

    public void SetStrategy(IProductionStrategy strategy)
    {
        currentStrategy = strategy;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject incomingResource = collision.gameObject;

        Vector3 expectedEntryDirection = GetExpectedEntryDirection();
        Vector3 actualDirection = (incomingResource.transform.position - transform.position).normalized;

        if (Vector3.Dot(expectedEntryDirection, actualDirection) < 0.7f) // 1'e yakınsa doğru, < 0.7 yanlış yön
        {
            Debug.Log("Kaynak yanlış yönden geldi, kabul edilmedi.");
            return;
        }

        // Resource türüne göre stratejiyi belirle
        if (incomingResource.name.Contains(odunPrefab.name))
        {
            if (currentStrategy is PlankCraftStrategy)
            {
                Debug.Log("PlankCraftStrategy zaten atandı.");
                envanter++;
            }
            else
            {
                envanter = 0;
                SetStrategy(new PlankCraftStrategy());
                Debug.Log("Odun geldi, Plank strategy atandı.");
            }
        }
        else if (incomingResource.name.Contains(bakirIngotPrefab.name))
        {
            if (currentStrategy is CopperWireCraftStrategy)
            {
                Debug.Log("CopperWireCraftStrategy zaten atandı.");
                envanter++;
            }
            else
            {
                envanter = 0;
                SetStrategy(new CopperWireCraftStrategy());
                Debug.Log("BakirIngot geldi, CopperWire strategy atandı.");
            }
        }
        else if (incomingResource.name.Contains(demirIngotPrefab.name))
        {
            if (currentStrategy is SteelCraftStrategy)
            {
                Debug.Log("SteelCraftStrategy zaten atandı.");
                envanter++;
            }
            else
            {
                envanter = 0;
                SetStrategy(new SteelCraftStrategy());
                Debug.Log("DemirIngot geldi, Steel strategy atandı.");
            }
        }
        else
        {
            Debug.LogWarning("Bilinmeyen resource tipi.");
            return;
        }

        // Strategy uygulanır
        if (envanter >= currentStrategy.neededAmount && !isCrafting)
        {
            isCrafting = true;
            craftTimer = 0f;
            Debug.Log("Üretim için sayaç başlatıldı.");
        }
        // Girdi kaynağı yok edilir
        Destroy(incomingResource);
    }


    public override void AcceptProduct(GameObject product)
    {
        throw new System.NotImplementedException();
    }

    public override bool CanAcceptProduct(GameObject product)
    {
        throw new System.NotImplementedException();
    }

    public override GameObject GetOutputProduct()
    {
        throw new System.NotImplementedException();
    }

    public override bool HasProductToOutput()
    {
        throw new System.NotImplementedException();
    }

    public void SpawnResource(GameObject resourcePrefab)
    {
        if (resourcePrefab != null)
        {
            Vector3 checkPosition = GetCheckPosition();
            Collider2D conveyorBelt = GetConveyorBeltAtPosition(checkPosition);

            Vector3 spawnPosition = conveyorBelt != null
                ? new Vector3(conveyorBelt.transform.position.x, conveyorBelt.transform.position.y, conveyorBelt.transform.position.z - 0.1f)
                : checkPosition;

            GameObject spawnedResource = Instantiate(resourcePrefab, spawnPosition, Quaternion.identity);

            Rigidbody2D rb = spawnedResource.GetComponent<Rigidbody2D>();
            if (rb == null) rb = spawnedResource.AddComponent<Rigidbody2D>();

            if (conveyorBelt != null)
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    private Vector3 GetCheckPosition()
    {
        Vector3 offset = Vector3.zero;

        switch (yon)
        {
            case 0: // Sa�
                offset = new Vector3(0.75f, 0f, 0f);
                break;
            case 1: // Yukar�
                offset = new Vector3(0f, 0.75f, 0f);
                break;
            case 2: // Sol
                offset = new Vector3(-0.75f, 0f, 0f);
                break;
            case 3: // A�a��
                offset = new Vector3(0f, -0.75f, 0f);
                break;
            default:
                Debug.LogError("Wrong direction input");
                break;
        }

        return transform.position + offset;
    }

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

    private Vector3 GetExpectedEntryDirection()
    {
        switch (yon)
        {
            case 0: return Vector3.left;   // Çıkış sağ → giriş sol
            case 1: return Vector3.down;   // Çıkış yukarı → giriş aşağı
            case 2: return Vector3.right;  // Çıkış sol → giriş sağ
            case 3: return Vector3.up;     // Çıkış aşağı → giriş yukarı
            default:
                Debug.LogError("Geçersiz yön");
                return Vector3.zero;
        }
    }
}
