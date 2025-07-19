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

    [Header("Gold Conversion Settings")]
    [SerializeField] private int goldPerUnresearchedResource = 10; // Research edilmeyen resource baþýna gold
    [SerializeField] private float goldConversionInterval = 3f; // Gold'a çevirme aralýðý

    [Header("Debug")]
    public bool showDebugInfo = true;

    // Original fields
    List<Resource> resources = new List<Resource>();

    // Research system fields
    private ResearchTree researchTree;
    private ResearchNode currentNode;
    private List<GameObject> collectedResources = new List<GameObject>();
    private Dictionary<ResourceType, int> resourceCounts = new Dictionary<ResourceType, int>();

    // Gold conversion fields
    private Queue<GameObject> unresearchedResourceQueue = new Queue<GameObject>();
    private Coroutine goldConversionCoroutine;

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

        // Gold conversion coroutine'ini baþlat
        StartGoldConversion();
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

        // Çevredeki resource'larý sürekli kontrol et
        CheckForNewResources();
    }

    private void InitializeResourceCounts()
    {
        // Tüm ResourceType'larý 0 ile initialize et
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            resourceCounts[type] = 0;
        }
    }

    // Çevredeki yeni resource'larý kontrol et
    private void CheckForNewResources()
    {
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, researchRadius, resourceLayerMask);

        foreach (Collider2D obj in nearbyObjects)
        {
            GameObject resourceObj = obj.gameObject;

            // Zaten iþlenmiþ mi kontrol et
            if (collectedResources.Contains(resourceObj) ||
                unresearchedResourceQueue.Contains(resourceObj))
            {
                continue;
            }

            // Resource component'ini kontrol et
            Resource resourceComponent = obj.GetComponent<Resource>();
            if (resourceComponent != null || IsLegacyResource(resourceObj))
            {
                ProcessIncomingResource(resourceObj, resourceComponent);
            }
        }
    }

    // Gelen resource'ý iþle - research'e dahil et veya gold kuyruðuna ekle
    private void ProcessIncomingResource(GameObject resourceObj, Resource resourceComponent)
    {
        ResourceType resourceType = GetResourceType(resourceObj, resourceComponent);

        // Aktif research varsa ve bu resource gerekli mi kontrol et
        if (isResearching && currentNode != null && IsResourceNeededForCurrentResearch(resourceType))
        {
            // Research için gerekli - normal research sistemine dahil et
            CollectResourceForResearch(resourceObj, resourceComponent, resourceType);
        }
        else
        {
            // Research edilmiyor veya bu resource gerekli deðil - gold'a çevir
            AddToGoldConversionQueue(resourceObj, resourceType);
        }
    }

    // Mevcut research için bu resource gerekli mi?
    private bool IsResourceNeededForCurrentResearch(ResourceType resourceType)
    {
        if (currentNode == null) return false;

        foreach (var requirement in currentNode.resourceRequirements)
        {
            if (requirement.resourceType == resourceType &&
                requirement.currentAmount < requirement.requiredAmount)
            {
                return true;
            }
        }
        return false;
    }

    // Resource'ý research için topla
    private void CollectResourceForResearch(GameObject resourceObj, Resource resourceComponent, ResourceType resourceType)
    {
        if (!collectedResources.Contains(resourceObj))
        {
            collectedResources.Add(resourceObj);
            resourceCounts[resourceType]++;

            // Resource'u deaktif et (toplandý)
            resourceObj.SetActive(false);

            if (showDebugInfo)
            {
                Debug.Log($"Collected {resourceType} for research: {currentNode.name}");
            }

            // Hemen research'e ekle
            TryAddResourceToCurrentResearch(resourceType);
        }
    }

    // Resource'ý gold dönüþüm kuyruðuna ekle
    private void AddToGoldConversionQueue(GameObject resourceObj, ResourceType resourceType)
    {
        unresearchedResourceQueue.Enqueue(resourceObj);

        // Resource'u deaktif et
        resourceObj.SetActive(false);

        if (showDebugInfo)
        {
            Debug.Log($"Added {resourceType} to gold conversion queue");
        }
    }

    // Gold dönüþüm sistemini baþlat
    private void StartGoldConversion()
    {
        if (goldConversionCoroutine != null)
        {
            StopCoroutine(goldConversionCoroutine);
        }
        goldConversionCoroutine = StartCoroutine(GoldConversionRoutine());
    }

    // Gold dönüþüm coroutine
    private System.Collections.IEnumerator GoldConversionRoutine()
    {
        while (true)
        {
            if (unresearchedResourceQueue.Count > 0)
            {
                GameObject resourceToConvert = unresearchedResourceQueue.Dequeue();

                if (resourceToConvert != null)
                {
                    ConvertResourceToGold(resourceToConvert);
                }
            }

            yield return new WaitForSeconds(goldConversionInterval);
        }
    }

    // Resource'ý gold'a çevir
    private void ConvertResourceToGold(GameObject resource)
    {
        ResourceType resourceType = GetResourceType(resource, resource.GetComponent<Resource>());

        // Gold ekle
        gameController.Gold += goldPerUnresearchedResource;

        if (showDebugInfo)
        {
            Debug.Log($"Converted {resourceType} to {goldPerUnresearchedResource} gold! Total Gold: {gameController.Gold}");
        }

        // Efekt göster
        ShowGoldConversionEffect(resource.transform.position);

        // Resource'ý yok et
        Destroy(resource);
    }

    // Gold dönüþüm efekti göster
    private void ShowGoldConversionEffect(Vector3 position)
    {
        // Burada partikül efekti, ses, animasyon vb. eklenebilir
        Debug.Log($"Gold conversion effect at {position}");
    }

    // Resource type'ýný belirle
    private ResourceType GetResourceType(GameObject resourceObj, Resource resourceComponent)
    {
        if (resourceComponent != null)
        {
            return ConvertStringToResourceType(resourceComponent.getType());
        }
        else
        {
            return GetLegacyResourceType(resourceObj);
        }
    }

    // Legacy resource type'ýný belirle
    private ResourceType GetLegacyResourceType(GameObject obj)
    {
        if (obj.GetComponent<Odun>() != null)
            return ResourceType.Wood;
        else if (obj.GetComponent<HamDemir>() != null)
            return ResourceType.Iron;
        else if (obj.GetComponent<HamBakir>() != null)
            return ResourceType.CopperOre;
        else
            return ResourceType.Wood; // Default
    }

    // Legacy resource kontrolü
    private bool IsLegacyResource(GameObject obj)
    {
        return obj.GetComponent<Odun>() != null ||
               obj.GetComponent<HamDemir>() != null ||
               obj.GetComponent<HamBakir>() != null;
    }

    // Mevcut research'e resource eklemeyi dene
    private void TryAddResourceToCurrentResearch(ResourceType resourceType)
    {
        if (currentNode == null || !isResearching) return;

        foreach (var requirement in currentNode.resourceRequirements)
        {
            if (requirement.resourceType == resourceType &&
                requirement.currentAmount < requirement.requiredAmount)
            {
                int availableAmount = resourceCounts[resourceType];
                int amountToAdd = Mathf.Min(availableAmount, requirement.requiredAmount - requirement.currentAmount);

                if (amountToAdd > 0)
                {
                    if (researchTree.AddResourceToResearch(currentResearchId, resourceType, amountToAdd))
                    {
                        resourceCounts[resourceType] -= amountToAdd;

                        if (showDebugInfo)
                        {
                            Debug.Log($"Added {amountToAdd} {resourceType} to research {currentNode.name}");
                        }
                    }
                }
                break;
            }
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

        // Research'teki resource'larý gold kuyruðuna aktar
        TransferResearchResourcesToGoldQueue();

        currentResearchId = "";
        isResearching = false;
        currentNode = null;
    }

    // Research'teki resource'larý gold kuyruðuna aktar
    private void TransferResearchResourcesToGoldQueue()
    {
        foreach (GameObject resource in collectedResources)
        {
            if (resource != null)
            {
                unresearchedResourceQueue.Enqueue(resource);
                Debug.Log($"Transferred research resource to gold queue: {resource.name}");
            }
        }
        ClearCollectedResources();
    }

    private void CollectNearbyResources()
    {
        // Bu metod artýk CheckForNewResources() tarafýndan sürekli çaðrýlýyor
        // Ama manuel research tetikleme için hala kullanýlabilir
    }

    private void ProcessCollectedResources()
    {
        // Research için toplanan resource'larý iþle
        foreach (ResourceType type in resourceCounts.Keys)
        {
            if (resourceCounts[type] > 0)
            {
                TryAddResourceToCurrentResearch(type);
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

        // Kalan resource'larý gold kuyruðuna aktar
        TransferResearchResourcesToGoldQueue();
    }

    private void ClearCollectedResources()
    {
        collectedResources.Clear();
        InitializeResourceCounts();
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
            Debug.Log($"  - {requirement.resourceType}: {requirement.currentAmount}/{requirement.requiredAmount}");
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

    // Gold dönüþüm durumunu kontrol etme metodlarý
    public int GetGoldQueueCount()
    {
        return unresearchedResourceQueue.Count;
    }

    public void SetGoldPerResource(int goldAmount)
    {
        goldPerUnresearchedResource = goldAmount;
    }

    public void SetGoldConversionInterval(float interval)
    {
        goldConversionInterval = interval;
        // Coroutine'i yeniden baþlat
        StartGoldConversion();
    }

    // Debug için gold kuyruðunu göster
    void OnDrawGizmos()
    {
        // Lab'ýn etki alanýný göster
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(researchRadius * 2, researchRadius * 2, 0f));

        // Research resource'larýný göster (mavi)
        if (Application.isPlaying && collectedResources.Count > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < collectedResources.Count; i++)
            {
                Vector3 pos = transform.position + new Vector3(i * 0.2f - 0.4f, 0.7f, 0);
                Gizmos.DrawSphere(pos, 0.1f);
            }
        }

        // Gold kuyruðundaki resource'larý göster (sarý)
        if (Application.isPlaying && unresearchedResourceQueue.Count > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < unresearchedResourceQueue.Count; i++)
            {
                Vector3 pos = transform.position + new Vector3(i * 0.2f - 0.4f, -0.7f, 0);
                Gizmos.DrawSphere(pos, 0.1f);
            }
        }
    }
}