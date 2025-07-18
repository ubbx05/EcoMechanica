using UnityEngine;
using UnityEngine.UIElements;

public class Workshop : Machine
{
    public enum CraftingRecipe
    {
        Plank,
        CopperWire
    }

    private GameObject plankPrefab;
    public GameObject odunPrefab;

    [SerializeField] private float craftingInterval = 4f; // 4 saniyede bir üretim yapacak
    private RotatingBuildings rotator;
    private IProductionStrategy currentStrategy;
    private int yon;
    private bool flag = false;

    public void SetStrategy(IProductionStrategy strategy)
    {
        currentStrategy = strategy;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Resource"))
        {
            //Debug.Log("Bu alana yerleştirilemez ");
        }
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotator = GetComponent<RotatingBuildings>();
        if (rotator != null)
        {
            yon = rotator.GetTransferYonu();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void DetermineCraftStrategy(CraftingRecipe selectedRecipe)
    {
        switch (selectedRecipe)
        {
            case CraftingRecipe.Plank:
                SetStrategy(new PlankCraftStrategy(this, plankPrefab));
                Debug.Log("Plank strategy selected");
                break;

            case CraftingRecipe.CopperWire:
                SetStrategy(new CopperCraftWireStrategy());
                Debug.Log("Copper wire strategy selected");
                break;

            case CraftingRecipe.IronPlate:
                SetStrategy(new IronCraftPlateStrategy());
                Debug.Log("Iron plate strategy selected");
                break;

            default:
                Debug.LogWarning("Selected recipe has no strategy implemented.");
                currentStrategy = null;
                break;
        }
    }

    public void SpawnResource(GameObject resourcePrefab)
    {
        if (resourcePrefab != null)
        {
            // Y�ne g�re kontrol pozisyonunu hesapla
            Vector3 checkPosition = GetCheckPosition();

            // O pozisyonda conveyor belt var m� kontrol et
            Collider2D conveyorBelt = GetConveyorBeltAtPosition(checkPosition);

            Vector3 spawnPosition;

            if (conveyorBelt != null)
            {
                // Conveyor belt varsa onun �zerinde spawn et
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

            // Resource'� spawn et
            GameObject spawnedResource = Instantiate(resourcePrefab, spawnPosition, Quaternion.identity);

            // Rigidbody2D ekle
            Rigidbody2D rb = spawnedResource.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = spawnedResource.AddComponent<Rigidbody2D>();
            }

            // Conveyor belt �zerindeyse kinematic yap
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
}
