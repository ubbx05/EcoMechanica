using Mono.Cecil;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class ResearchLab : MonoBehaviour
{
    [SerializeField] public GameController gameController;

    [Header("Research Lab Settings")]
    public float researchRadius = 2f;

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
        // ResearchTree instance'ına erişim
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

        // Çevredeki resource'ları sürekli kontrol et
        CheckForNewResources();
    }

    // Collision ile gelen resource'ları kontrol et
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null || collision.gameObject == null)
        {
            return;
        }

        GameObject incomingResource = collision.gameObject;
        Debug.Log($"🔬 ResearchLab collision detected: {incomingResource.name}");

        // Resource component'ini kontrol et - PARENT VE CHILD'DA ARA
        Resource resourceComponent = incomingResource.GetComponent<Resource>();
        if (resourceComponent == null)
        {
            // Parent'ta yoksa child'larda ara
            resourceComponent = incomingResource.GetComponentInChildren<Resource>();
        }

        if (resourceComponent != null)
        {
            ResourceType resourceType = resourceComponent.getType();
            int goldAmount = resourceComponent.getIncome();

            Debug.Log($"🔍 DEBUG: Resource {incomingResource.name} has income: {goldAmount}");

            // Aktif research varsa ve bu resource gerekli mi kontrol et
            if (isResearching && currentNode != null && IsResourceNeededForCurrentResearch(resourceType))
            {
                // Research için gerekli - research'e ver, gold verme
                CollectResourceForResearch(incomingResource, resourceComponent, resourceType);
                Debug.Log($"🔬 Resource {incomingResource.name} added to research");
            }
            else
            {
                // Research edilmiyor veya bu resource gerekli değil - gold'a çevir                
                if (gameController != null)
                {
                    int oldGold = gameController.Gold;
                    gameController.Gold += goldAmount;
                    Debug.Log($"💰 BEFORE: {oldGold} | ADDING: {goldAmount} | AFTER: {gameController.Gold}");
                    Debug.Log($"💰 Added {goldAmount} gold from {incomingResource.name}. Total Gold: {gameController.Gold}");
                }
                else
                {
                    Debug.LogError("GameController reference is null!");
                }

                // Resource'ı yok et
                Destroy(incomingResource);
                Debug.Log($"🗑️ Resource {incomingResource.name} consumed for gold");
            }
        }
        else
        {
            // Resource component'i yoksa da gold'a çevir (default değerle)
            Debug.Log($"⚠️ Object {incomingResource.name} has no Resource component - converting to default gold");

            if (gameController != null)
            {
                gameController.Gold += 1; // Default gold değeri
                Debug.Log($"💰 Added 1 gold from non-resource {incomingResource.name}. Total Gold: {gameController.Gold}");
            }

            Destroy(incomingResource);
            Debug.Log($"🗑️ Non-resource {incomingResource.name} consumed for default gold");
        }
    }

    private void InitializeResourceCounts()
    {
        // Tüm ResourceType'ları 0 ile initialize et
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            resourceCounts[type] = 0;
        }
    }

    // Çevredeki yeni resource'ları kontrol et
    private void CheckForNewResources()
    {
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, researchRadius);

        foreach (Collider2D obj in nearbyObjects)
        {
            GameObject resourceObj = obj.gameObject;

            // Zaten işlenmiş mi kontrol et
            if (collectedResources.Contains(resourceObj))
            {
                continue;
            }

            // Resource component'ini kontrol et
            Resource resourceComponent = obj.GetComponent<Resource>();
            if (resourceComponent != null)
            {
                ProcessIncomingResource(resourceObj, resourceComponent);
            }
        }
    }

    // Gelen resource'ı işle - research'e dahil et
    private void ProcessIncomingResource(GameObject resourceObj, Resource resourceComponent)
    {
        ResourceType resourceType = resourceComponent.getType();

        // Aktif research varsa ve bu resource gerekli mi kontrol et
        if (isResearching && currentNode != null && IsResourceNeededForCurrentResearch(resourceType))
        {
            // Research için gerekli - normal research sistemine dahil et
            CollectResourceForResearch(resourceObj, resourceComponent, resourceType);
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

    // Resource'ı research için topla
    private void CollectResourceForResearch(GameObject resourceObj, Resource resourceComponent, ResourceType resourceType)
    {
        if (!collectedResources.Contains(resourceObj))
        {
            collectedResources.Add(resourceObj);
            resourceCounts[resourceType]++;

            // Resource'u deaktif et (toplandı)
            resourceObj.SetActive(false);

            if (showDebugInfo)
            {
                Debug.Log($"Collected {resourceType} for research: {currentNode.name}");
            }

            // Hemen research'e ekle
            TryAddResourceToCurrentResearch(resourceType);
        }
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
                // Research'te kullanılan resource'ları gold income'a dahil etme
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
        // Eğer research devam etmiyorsa, hiçbir resource kullanılmıyor
        if (!isResearching || currentNode == null)
            return false;

        // Resource'un GameObject'ini kontrol et
        GameObject resourceObj = resource.gameObject;

        // Toplanan resource'lar arasında var mı?
        if (collectedResources.Contains(resourceObj))
            return true;

        // Aktif olmayan resource'lar research'te kullanılıyor olabilir
        if (!resourceObj.activeInHierarchy)
        {
            ResourceType resourceType = resource.getType();

            // Bu resource type'ı current research'te gerekli mi?
            foreach (var requirement in currentNode.resourceRequirements)
            {
                if (requirement.resourceType == resourceType)
                {
                    return true; // Bu resource research'te kullanılıyor
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

        // Çevredeki resource'ları topla
        CollectNearbyResources();

        // Toplanan resource'ları research'e ekle
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

        // Research başlatmayı dene
        if (researchTree.StartResearch(researchId))
        {
            currentResearchId = researchId;
            isResearching = true;
            currentNode = researchTree.GetResearchNode(researchId);

            // Mevcut resource'ları temizle
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

        currentResearchId = "";
        isResearching = false;
        currentNode = null;
    }

    private void CollectNearbyResources()
    {
        // Bu metod artık CheckForNewResources() tarafından sürekli çağrılıyor
        // Ama manuel research tetikleme için hala kullanılabilir
    }

    private void ProcessCollectedResources()
    {
        // Research için toplanan resource'ları işle
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

        // Research tamamlandı mı kontrol et
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
        // Research tamamlandığında yapılacak işlemler
        isResearching = false;
        currentResearchId = "";
        ClearCollectedResources();
    }

    private void ClearCollectedResources()
    {
        collectedResources.Clear();
        InitializeResourceCounts();
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

    // Public metodlar - UI veya diğer sistemler için
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

    // Debug için research alanını göster
    void OnDrawGizmos()
    {
        // Lab'ın etki alanını göster
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(researchRadius * 2, researchRadius * 2, 0f));

        // Research resource'larını göster (mavi)
        if (Application.isPlaying && collectedResources.Count > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < collectedResources.Count; i++)
            {
                Vector3 pos = transform.position + new Vector3(i * 0.2f - 0.4f, 0.7f, 0);
                Gizmos.DrawSphere(pos, 0.1f);
            }
        }
    }
}