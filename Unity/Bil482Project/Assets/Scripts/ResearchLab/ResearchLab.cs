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
    [SerializeField] private int goldPerUnresearchedResource = 10; // Research edilmeyen resource ba��na gold
    [SerializeField] private float goldConversionInterval = 3f; // Gold'a �evirme aral���

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
        // ResearchTree instance'�na eri�im
        researchTree = ResearchTree.Instance;
        if (researchTree == null)
        {
            Debug.LogError("ResearchTree instance not found! Make sure ResearchTree is in the scene.");
        }

        // Resource count dictionary'sini initialize et
        InitializeResourceCounts();

        // Gold conversion coroutine'ini ba�lat
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

        // �evredeki resource'lar� s�rekli kontrol et
        CheckForNewResources();
    }

    private void InitializeResourceCounts()
    {
        // T�m ResourceType'lar� 0 ile initialize et
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            resourceCounts[type] = 0;
        }
    }

    // �evredeki yeni resource'lar� kontrol et
    private void CheckForNewResources()
    {
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, researchRadius, resourceLayerMask);

        foreach (Collider2D obj in nearbyObjects)
        {
            GameObject resourceObj = obj.gameObject;

            // Zaten i�lenmi� mi kontrol et
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

    // Gelen resource'� i�le - research'e dahil et veya gold kuyru�una ekle
    private void ProcessIncomingResource(GameObject resourceObj, Resource resourceComponent)
    {
        ResourceType resourceType = GetResourceType(resourceObj, resourceComponent);

        // Aktif research varsa ve bu resource gerekli mi kontrol et
        if (isResearching && currentNode != null && IsResourceNeededForCurrentResearch(resourceType))
        {
            // Research i�in gerekli - normal research sistemine dahil et
            CollectResourceForResearch(resourceObj, resourceComponent, resourceType);
        }
        else
        {
            // Research edilmiyor veya bu resource gerekli de�il - gold'a �evir
            AddToGoldConversionQueue(resourceObj, resourceType);
        }
    }

    // Mevcut research i�in bu resource gerekli mi?
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

    // Resource'� research i�in topla
    private void CollectResourceForResearch(GameObject resourceObj, Resource resourceComponent, ResourceType resourceType)
    {
        if (!collectedResources.Contains(resourceObj))
        {
            collectedResources.Add(resourceObj);
            resourceCounts[resourceType]++;

            // Resource'u deaktif et (topland�)
            resourceObj.SetActive(false);

            if (showDebugInfo)
            {
                Debug.Log($"Collected {resourceType} for research: {currentNode.name}");
            }

            // Hemen research'e ekle
            TryAddResourceToCurrentResearch(resourceType);
        }
    }

    // Resource'� gold d�n���m kuyru�una ekle
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

    // Gold d�n���m sistemini ba�lat
    private void StartGoldConversion()
    {
        if (goldConversionCoroutine != null)
        {
            StopCoroutine(goldConversionCoroutine);
        }
        goldConversionCoroutine = StartCoroutine(GoldConversionRoutine());
    }

    // Gold d�n���m coroutine
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

    // Resource'� gold'a �evir
    private void ConvertResourceToGold(GameObject resource)
    {
        ResourceType resourceType = GetResourceType(resource, resource.GetComponent<Resource>());

        // Gold ekle
        gameController.Gold += goldPerUnresearchedResource;

        if (showDebugInfo)
        {
            Debug.Log($"Converted {resourceType} to {goldPerUnresearchedResource} gold! Total Gold: {gameController.Gold}");
        }

        // Efekt g�ster
        ShowGoldConversionEffect(resource.transform.position);

        // Resource'� yok et
        Destroy(resource);
    }

    // Gold d�n���m efekti g�ster
    private void ShowGoldConversionEffect(Vector3 position)
    {
        // Burada partik�l efekti, ses, animasyon vb. eklenebilir
        Debug.Log($"Gold conversion effect at {position}");
    }

    // Resource type'�n� belirle
    private ResourceType GetResourceType(GameObject resourceObj, Resource resourceComponent)
    {
        if (resourceComponent != null)
        {
            //return ConvertStringToResourceType(resourceComponent.getType());
            return resourceComponent.getType();
        }
        else
        {
            return GetLegacyResourceType(resourceObj);
        }
    }

    // Legacy resource type'�n� belirle
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

    // Legacy resource kontrol�
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
                // Research'te kullan�lan resource'lar� gold income'a dahil etme
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
        // E�er research devam etmiyorsa, hi�bir resource kullan�lm�yor
        if (!isResearching || currentNode == null)
            return false;

        // Resource'un GameObject'ini kontrol et
        GameObject resourceObj = resource.gameObject;

        // Toplanan resource'lar aras�nda var m�?
        if (collectedResources.Contains(resourceObj))
            return true;

        // Aktif olmayan resource'lar research'te kullan�l�yor olabilir
        if (!resourceObj.activeInHierarchy)
        {
            // Resource type'�n� kontrol et
            //string resourceTypeStr = resource.getType();
            //ResourceType resourceType = ConvertStringToResourceType(resourceTypeStr);
            ResourceType resourceType = resource.getType();

            // Bu resource type'� current research'te gerekli mi?
            foreach (var requirement in currentNode.resourceRequirements)
            {
                if (requirement.resourceType == resourceType)
                {
                    return true; // Bu resource research'te kullan�l�yor
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

        // �evredeki resource'lar� topla
        CollectNearbyResources();

        // Toplanan resource'lar� research'e ekle
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

        // Research ba�latmay� dene
        if (researchTree.StartResearch(researchId))
        {
            currentResearchId = researchId;
            isResearching = true;
            currentNode = researchTree.GetResearchNode(researchId);

            // Mevcut resource'lar� temizle
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

        // Research'teki resource'lar� gold kuyru�una aktar
        TransferResearchResourcesToGoldQueue();

        currentResearchId = "";
        isResearching = false;
        currentNode = null;
    }

    // Research'teki resource'lar� gold kuyru�una aktar
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
        // Bu metod art�k CheckForNewResources() taraf�ndan s�rekli �a�r�l�yor
        // Ama manuel research tetikleme i�in hala kullan�labilir
    }

    private void ProcessCollectedResources()
    {
        // Research i�in toplanan resource'lar� i�le
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

        // Research tamamland� m� kontrol et
        if (currentNode.state == ResearchState.Completed)
        {
            Debug.Log($"Research completed: {currentNode.name}!");
            OnResearchCompleted();
        }
        else if (showDebugInfo)
        {
            // Progress bilgisini g�ster
            float progress = currentNode.GetCompletionPercentage();
            if (progress > 0)
            {
                Debug.Log($"Research progress: {currentNode.name} - {progress * 100:F1}%");
            }
        }
    }

    private void OnResearchCompleted()
    {
        // Research tamamland���nda yap�lacak i�lemler
        isResearching = false;
        currentResearchId = "";

        // Kalan resource'lar� gold kuyru�una aktar
        TransferResearchResourcesToGoldQueue();
    }

    private void ClearCollectedResources()
    {
        collectedResources.Clear();
        InitializeResourceCounts();
    }

    private ResourceType ConvertStringToResourceType(string resourceTypeStr)
    {
        // String'den ResourceType'a d�n��t�rme
        switch (resourceTypeStr)
        {
            case "Odun":
                return ResourceType.Wood;
            case "HamDemir":
                return ResourceType.Iron;
            case "HamBakir":
                return ResourceType.CopperOre;
            default:
                return ResourceType.Wood; // Default de�er
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

    // Public metodlar - UI veya di�er sistemler i�in
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

    // Resource ekleme metodu (manuel resource ekleme i�in)
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

    // Gold d�n���m durumunu kontrol etme metodlar�
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
        // Coroutine'i yeniden ba�lat
        StartGoldConversion();
    }

    // Debug i�in gold kuyru�unu g�ster
    void OnDrawGizmos()
    {
        // Lab'�n etki alan�n� g�ster
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(researchRadius * 2, researchRadius * 2, 0f));

        // Research resource'lar�n� g�ster (mavi)
        if (Application.isPlaying && collectedResources.Count > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < collectedResources.Count; i++)
            {
                Vector3 pos = transform.position + new Vector3(i * 0.2f - 0.4f, 0.7f, 0);
                Gizmos.DrawSphere(pos, 0.1f);
            }
        }

        // Gold kuyru�undaki resource'lar� g�ster (sar�)
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