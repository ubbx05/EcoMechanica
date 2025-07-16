using Mono.Cecil;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class ResearchLab : MonoBehaviour
{
    [SerializeField] public GameController gameController;

    [Header("Research Lab Settings")]
    public float researchRadius = 2f;
    public LayerMask resourceLayerMask = -1;

    [Header("Current Research")]
    public string currentResearchId = "";
    public bool isResearching = false;

    [Header("Debug")]
    public bool showDebugInfo = true;

    // Original fields
    List<Resource> resources = new List<Resource>();

    // Research system fields
    private ResearchTree researchTree;
    private ResearchNode currentNode;
    private List<GameObject> collectedResources = new List<GameObject>();
    private Dictionary<ResourceType, int> resourceCounts = new Dictionary<ResourceType, int>();

    void Start()
    {
        // ResearchTree instance'ýna eriþim
        researchTree = ResearchTree.Instance;
        if (researchTree == null)
        {
            Debug.LogError("ResearchTree instance not found! Make sure ResearchTree is in the scene.");
        }

        // Resource count dictionary'sini initialize et
        InitializeResourceCounts();
    }

    void Update()
    {
        // Original gold income functionality
        int gold = GoldIncome();
        gameController.Gold += gold;

        // Research progress check
        if (isResearching && !string.IsNullOrEmpty(currentResearchId))
        {
            CheckResearchProgress();
        }
    }

    private void InitializeResourceCounts()
    {
        // Tüm ResourceType'larý 0 ile initialize et
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            resourceCounts[type] = 0;
        }
    }

    public int GoldIncome()
    {
        int sonuc = 0;
        for (int i = 0; i < resources.Count; i++)
        {
            if (resources[i] != null)
            {
                // Research'te kullanýlan resource'larý gold income'a dahil etme
                if (!IsResourceUsedInResearch(resources[i]))
                {
                    sonuc += resources[i].getIncome();
                }
            }
        }
        return sonuc;
    }

    private bool IsResourceUsedInResearch(Resource resource)
    {
        // Eðer research devam etmiyorsa, hiçbir resource kullanýlmýyor
        if (!isResearching || currentNode == null)
            return false;

        // Resource'un GameObject'ini kontrol et
        GameObject resourceObj = resource.gameObject;

        // Toplanan resource'lar arasýnda var mý?
        if (collectedResources.Contains(resourceObj))
            return true;

        // Aktif olmayan resource'lar research'te kullanýlýyor olabilir
        if (!resourceObj.activeInHierarchy)
        {
            // Resource type'ýný kontrol et
            string resourceTypeStr = resource.getType();
            ResourceType resourceType = ConvertStringToResourceType(resourceTypeStr);

            // Bu resource type'ý current research'te gerekli mi?
            foreach (var requirement in currentNode.resourceRequirements)
            {
                if (requirement.resourceType == resourceType)
                {
                    return true; // Bu resource research'te kullanýlýyor
                }
            }
        }

        return false;
    }

    public void Research()
    {
        if (!isResearching || string.IsNullOrEmpty(currentResearchId))
        {
            Debug.LogWarning("No active research to process!");
            return;
        }

        // Çevredeki resource'larý topla
        CollectNearbyResources();

        // Toplanan resource'larý research'e ekle
        ProcessCollectedResources();
    }

    public void StartResearch(string researchId)
    {
        if (researchTree == null)
        {
            Debug.LogError("ResearchTree not available!");
            return;
        }

        if (isResearching)
        {
            Debug.LogWarning("Already researching! Stop current research first.");
            return;
        }

        // Research baþlatmayý dene
        if (researchTree.StartResearch(researchId))
        {
            currentResearchId = researchId;
            isResearching = true;
            currentNode = researchTree.GetResearchNode(researchId);

            // Mevcut resource'larý temizle
            ClearCollectedResources();

            Debug.Log($"Started research: {currentNode.name}");

            if (showDebugInfo)
            {
                ShowResearchRequirements();
            }
        }
    }

    public void StopResearch()
    {
        if (!isResearching) return;

        Debug.Log($"Stopped research: {currentNode.name}");

        // Resource'larý geri ver (isteðe baðlý)
        ReturnCollectedResources();

        currentResearchId = "";
        isResearching = false;
        currentNode = null;
    }

    private void CollectNearbyResources()
    {
        // Belirli bir radius içindeki resource'larý bul
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, researchRadius, resourceLayerMask);

        foreach (Collider2D obj in nearbyObjects)
        {
            // Resource component'ini kontrol et
            Resource resourceComponent = obj.GetComponent<Resource>();
            if (resourceComponent != null)
            {
                // Resource'u topla
                CollectResource(obj.gameObject, resourceComponent);
            }
            else
            {
                // Legacy resource type kontrolü (mevcut sistemle uyumluluk için)
                CheckLegacyResourceTypes(obj.gameObject);
            }
        }
    }

    private void CollectResource(GameObject resourceObj, Resource resourceComponent)
    {
        string resourceTypeStr = resourceComponent.getType();
        ResourceType resourceType = ConvertStringToResourceType(resourceTypeStr);

        if (resourceType != ResourceType.Wood || resourceTypeStr == "Odun") // Geçerli bir resource type ise
        {
            if (!collectedResources.Contains(resourceObj))
            {
                collectedResources.Add(resourceObj);
                resourceCounts[resourceType]++;

                // Resource'u deaktif et (toplandý)
                resourceObj.SetActive(false);

                if (showDebugInfo)
                {
                    Debug.Log($"Collected {resourceTypeStr} for research");
                }
            }
        }
    }

    private void CheckLegacyResourceTypes(GameObject obj)
    {
        // Mevcut sistemdeki resource type'larý kontrol et
        if (obj.GetComponent<Odun>() != null)
        {
            CollectLegacyResource(obj, ResourceType.Wood);
        }
        else if (obj.GetComponent<HamDemir>() != null)
        {
            CollectLegacyResource(obj, ResourceType.Iron);
        }
        else if (obj.GetComponent<HamBakir>() != null)
        {
            CollectLegacyResource(obj, ResourceType.CopperOre);
        }
    }

    private void CollectLegacyResource(GameObject resourceObj, ResourceType type)
    {
        if (!collectedResources.Contains(resourceObj))
        {
            collectedResources.Add(resourceObj);
            resourceCounts[type]++;

            // Resource'u deaktif et
            resourceObj.SetActive(false);

            if (showDebugInfo)
            {
                Debug.Log($"Collected legacy {type} for research");
            }
        }
    }

    private void ProcessCollectedResources()
    {
        if (currentNode == null) return;

        // Her resource requirement için kontrol et
        foreach (var requirement in currentNode.resourceRequirements)
        {
            ResourceType requiredType = requirement.resourceType;
            int availableAmount = resourceCounts[requiredType];

            if (availableAmount > 0)
            {
                // Gerekenden fazla resource eklenmeye çalýþýlýrsa reddedilir
                int amountToAdd = Mathf.Min(availableAmount, requirement.requiredAmount - requirement.currentAmount);

                if (amountToAdd > 0)
                {
                    // Research tree'ye resource ekle
                    if (researchTree.AddResourceToResearch(currentResearchId, requiredType, amountToAdd))
                    {
                        // Baþarýyla eklendi, local count'u güncelle
                        resourceCounts[requiredType] -= amountToAdd;

                        if (showDebugInfo)
                        {
                            Debug.Log($"Added {amountToAdd} {requiredType} to research {currentNode.name}");
                        }
                    }
                }
            }
        }
    }

    private void CheckResearchProgress()
    {
        if (currentNode == null) return;

        // Research tamamlandý mý kontrol et
        if (currentNode.state == ResearchState.Completed)
        {
            Debug.Log($"Research completed: {currentNode.name}!");
            OnResearchCompleted();
        }
        else if (showDebugInfo)
        {
            // Progress bilgisini göster
            float progress = currentNode.GetCompletionPercentage();
            if (progress > 0)
            {
                Debug.Log($"Research progress: {currentNode.name} - {progress * 100:F1}%");
            }
        }
    }

    private void OnResearchCompleted()
    {
        // Research tamamlandýðýnda yapýlacak iþlemler
        isResearching = false;
        currentResearchId = "";

        // Kalan resource'larý geri ver
        ReturnCollectedResources();

        // UI güncellemesi veya baþka iþlemler
        // Örneðin: UpdateResearchUI();
    }

    private void ClearCollectedResources()
    {
        collectedResources.Clear();
        InitializeResourceCounts();
    }

    private void ReturnCollectedResources()
    {
        // Toplanan ama kullanýlmayan resource'larý geri ver
        foreach (GameObject resource in collectedResources)
        {
            if (resource != null)
            {
                resource.SetActive(true);
            }
        }

        ClearCollectedResources();
    }

    private ResourceType ConvertStringToResourceType(string resourceTypeStr)
    {
        // String'den ResourceType'a dönüþtürme
        switch (resourceTypeStr)
        {
            case "Odun":
                return ResourceType.Wood;
            case "HamDemir":
                return ResourceType.Iron;
            case "HamBakir":
                return ResourceType.CopperOre;
            default:
                return ResourceType.Wood; // Default deðer
        }
    }

    private void ShowResearchRequirements()
    {
        if (currentNode == null) return;

        Debug.Log($"Research Requirements for {currentNode.name}:");
        foreach (var requirement in currentNode.resourceRequirements)
        {
            Debug.Log($"- {requirement.resourceType}: {requirement.currentAmount}/{requirement.requiredAmount}");
        }
    }

    // Public metodlar - UI veya diðer sistemler için
    public List<ResearchNode> GetAvailableResearch()
    {
        return researchTree?.GetAvailableResearch() ?? new List<ResearchNode>();
    }

    public List<ResearchNode> GetCompletedResearch()
    {
        return researchTree?.GetCompletedResearch() ?? new List<ResearchNode>();
    }

    public ResearchNode GetCurrentResearch()
    {
        return currentNode;
    }

    public bool IsResearching()
    {
        return isResearching;
    }

    // Resource ekleme metodu (manuel resource ekleme için)
    public void AddResource(Resource resource)
    {
        if (resource != null && !resources.Contains(resource))
        {
            resources.Add(resource);
        }
    }

    public void RemoveResource(Resource resource)
    {
        if (resource != null && resources.Contains(resource))
        {
            resources.Remove(resource);
        }
    }

    
}