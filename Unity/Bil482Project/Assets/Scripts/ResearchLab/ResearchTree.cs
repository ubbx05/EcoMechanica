using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourceRequirement
{
    public ResourceType resourceType;
    public int requiredAmount;
    public int currentAmount;

    public bool IsComplete => currentAmount >= requiredAmount;
    public bool CanAddMore => currentAmount < requiredAmount;

    public ResourceRequirement(ResourceType type, int required)
    {
        resourceType = type;
        requiredAmount = required;
        currentAmount = 0;
    }
}

public enum ResourceType
{
    Wood,
    Plank,
    CopperOre,
    IronOre,
    Copper,
    Iron,
    CopperWire,
    Steel,
    Magnet,
    Circuitboard
}

public enum ResearchState
{
    Locked,      // Henüz açýlmamýþ
    Unlocked,    // Açýlmýþ ama baþlanmamýþ
    InProgress,  // Araþtýrma devam ediyor
    Completed    // Tamamlanmýþ
}

[System.Serializable]
public class ResearchNode
{
    public string id;
    public string name;
    public string description;
    public ResearchState state;
    public List<ResourceRequirement> resourceRequirements; // Maksimum 2 resource
    public List<string> prerequisiteIds; // Önkoþul research ID'leri
    public List<string> unlocksIds; // Bu research'ün açtýðý research ID'leri
    public Sprite icon;

    public ResearchNode(string nodeId, string nodeName, string nodeDescription)
    {
        id = nodeId;
        name = nodeName;
        description = nodeDescription;
        state = ResearchState.Locked;
        resourceRequirements = new List<ResourceRequirement>();
        prerequisiteIds = new List<string>();
        unlocksIds = new List<string>();
    }

    public void AddResourceRequirement(ResourceType type, int amount)
    {
        if (resourceRequirements.Count >= 2)
        {
            Debug.LogError($"Research node {name} already has 2 resource requirements!");
            return;
        }

        resourceRequirements.Add(new ResourceRequirement(type, amount));
    }

    public bool IsReadyToComplete()
    {
        if (state != ResearchState.InProgress) return false;

        foreach (var requirement in resourceRequirements)
        {
            if (!requirement.IsComplete) return false;
        }

        return true;
    }

    public bool CanAddResource(ResourceType type, int amount)
    {
        if (state != ResearchState.InProgress) return false;

        ResourceRequirement requirement = GetResourceRequirement(type);
        if (requirement == null) return false; // Bu resource type gerekli deðil

        return requirement.CanAddMore && (requirement.currentAmount + amount) <= requirement.requiredAmount;
    }

    public bool AddResource(ResourceType type, int amount)
    {
        if (!CanAddResource(type, amount)) return false;

        ResourceRequirement requirement = GetResourceRequirement(type);
        requirement.currentAmount += amount;

        Debug.Log($"Added {amount} {type} to {name}. Progress: {requirement.currentAmount}/{requirement.requiredAmount}");

        return true;
    }

    private ResourceRequirement GetResourceRequirement(ResourceType type)
    {
        foreach (var requirement in resourceRequirements)
        {
            if (requirement.resourceType == type)
                return requirement;
        }
        return null;
    }

    public float GetCompletionPercentage()
    {
        if (resourceRequirements.Count == 0) return 0f;

        float totalProgress = 0f;
        foreach (var requirement in resourceRequirements)
        {
            totalProgress += (float)requirement.currentAmount / requirement.requiredAmount;
        }

        return totalProgress / resourceRequirements.Count;
    }
}

public class ResearchTree : MonoBehaviour
{
    [SerializeField] private List<ResearchNode> allNodes;
    private Dictionary<string, ResearchNode> nodeMap;

    public static ResearchTree Instance { get; private set; }
    public static int ExtractorLevelUnlocked = 1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeTree();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CreateSampleResearchTree();
    }

    private void InitializeTree()
    {
        nodeMap = new Dictionary<string, ResearchNode>();

        foreach (var node in allNodes)
        {
            nodeMap[node.id] = node;
        }
    }

    private void CreateSampleResearchTree()
    {
        ResearchNode ResearchLab = new ResearchNode("ResearchLab", "Basic HQ", "It unlocks the technology and gives you income.");
        ResearchLab.state = ResearchState.Unlocked;

        // Temel teknolojiler
        ResearchNode Furnace = new ResearchNode("Furnace", "Basic Melting", "It unlocks the technology of smelting iron and copper ores.");
        Furnace.AddResourceRequirement(ResourceType.Wood, 10);
        Furnace.prerequisiteIds.Add("ResearchLab");

        ResearchNode Workshop = new ResearchNode("workshop", "Advanced gears", "Make different usable devices for your enviroment");
        Workshop.AddResourceRequirement(ResourceType.CopperOre, 8);
        Workshop.AddResourceRequirement(ResourceType.IronOre, 8);
        Workshop.prerequisiteIds.Add("Furnace");

        ResearchNode Iron = new ResearchNode("iron_production", "iron production", "produce Iron");
        Iron.AddResourceRequirement(ResourceType.IronOre, 20);
        Iron.prerequisiteIds.Add("Furnace");

        ResearchNode Steel = new ResearchNode("steel_production", "steel production", "produce Steel");
        Steel.AddResourceRequirement(ResourceType.IronOre, 20);
        Steel.prerequisiteIds.Add("iron_production");

        ResearchNode Copper = new ResearchNode("copper_production", "copper production", "produce Copper");
        Copper.AddResourceRequirement(ResourceType.CopperOre, 20);
        Copper.prerequisiteIds.Add("Furnace");

        ResearchNode CopperWire = new ResearchNode("copperWire_production", "copperwire production", "produce Copper Wire");
        Copper.AddResourceRequirement(ResourceType.Copper, 20);
        Copper.prerequisiteIds.Add("copper_production");

        ResearchNode magnetProduction = new ResearchNode("magnet_production", "Magnet Production", "Unlocks the ability to produce magnets for advanced machinery");
        magnetProduction.AddResourceRequirement(ResourceType.CopperWire, 20);
        magnetProduction.prerequisiteIds.Add("copperWire_production");


        ResearchNode advancedForging = new ResearchNode("advanced_forging", "Advanced Forging", "Forge resources faster");
        advancedForging.AddResourceRequirement(ResourceType.Steel, 20);
        advancedForging.AddResourceRequirement(ResourceType.CopperWire, 15);
        advancedForging.prerequisiteIds.Add("steel_production");

        ResearchNode circuitboardProduction = new ResearchNode("circuitboard_production", "Circuit Board Production", "Produce circuit boards for electronics");
        circuitboardProduction.AddResourceRequirement(ResourceType.Iron, 10);
        circuitboardProduction.AddResourceRequirement(ResourceType.Magnet, 5);
        circuitboardProduction.prerequisiteIds.Add("magnet_production");

        ResearchNode computerProduction = new ResearchNode("computer_production", "Computer Production", "Produce computers for cleaners");
        computerProduction.AddResourceRequirement(ResourceType.Circuitboard, 10);
        computerProduction.AddResourceRequirement(ResourceType.Steel, 20);
        computerProduction.prerequisiteIds.Add("circuitboard_production");

        // Tree'ye ekle
        AddNodeToTree(ResearchLab);
        AddNodeToTree(Furnace);
        AddNodeToTree(Workshop);
        AddNodeToTree(Copper);
        AddNodeToTree(CopperWire);
        AddNodeToTree(Iron);
        AddNodeToTree(Steel);
        AddNodeToTree(magnetProduction);
        AddNodeToTree(advancedForging);
        AddNodeToTree(circuitboardProduction);
        AddNodeToTree(computerProduction);

        // Unlock iliþkilerini kur
        ResearchLab.unlocksIds.Add("Furnace");
        Furnace.unlocksIds.Add("Workshop");
        Furnace.unlocksIds.Add("Iron");
        Iron.unlocksIds.Add("Steel");
        Furnace.unlocksIds.Add("Copper");
        Copper.unlocksIds.Add("CopperWire");
        CopperWire.unlocksIds.Add("magnet_production");
        CopperWire.unlocksIds.Add("advanced_forging");
        Steel.unlocksIds.Add("advanced_forging");
        magnetProduction.unlocksIds.Add("circuitboard_production");
        Iron.unlocksIds.Add("circuitboard_production");
        circuitboardProduction.unlocksIds.Add("computer_production");
        Steel.unlocksIds.Add("computer_production");

        CheckAndUpdateUnlocks();
    }

    private void AddNodeToTree(ResearchNode node)
    {
        if (!allNodes.Contains(node))
        {
            allNodes.Add(node);
        }
        nodeMap[node.id] = node;
    }

    public bool StartResearch(string nodeId)
    {
        if (!nodeMap.ContainsKey(nodeId))
        {
            Debug.LogError($"Research node {nodeId} not found!");
            return false;
        }

        ResearchNode node = nodeMap[nodeId];

        // Zaten tamamlanmýþ veya devam eden research baþlatýlamaz
        if (node.state == ResearchState.Completed)
        {
            Debug.LogWarning($"Research {node.name} is already completed!");
            return false;
        }

        if (node.state == ResearchState.InProgress)
        {
            Debug.LogWarning($"Research {node.name} is already in progress!");
            return false;
        }

        // Locked durumda baþlatýlamaz
        if (node.state == ResearchState.Locked)
        {
            Debug.LogWarning($"Research {node.name} is locked! Complete prerequisites first.");
            return false;
        }

        // Önkoþullarý kontrol et
        if (!ArePrerequisitesCompleted(node))
        {
            Debug.LogWarning($"Prerequisites for {node.name} are not completed!");
            return false;
        }

        node.state = ResearchState.InProgress;
        Debug.Log($"Started research: {node.name}");
        return true;
    }

    public bool AddResourceToResearch(string nodeId, ResourceType resourceType, int amount)
    {
        if (!nodeMap.ContainsKey(nodeId))
        {
            Debug.LogError($"Research node {nodeId} not found!");
            return false;
        }

        ResearchNode node = nodeMap[nodeId];

        // Sadece devam eden research'e resource eklenebilir
        if (node.state != ResearchState.InProgress)
        {
            Debug.LogWarning($"Cannot add resources to {node.name} - research is not in progress!");
            return false;
        }

        // Resource eklenebilir mi kontrol et
        if (!node.CanAddResource(resourceType, amount))
        {
            Debug.LogWarning($"Cannot add {amount} {resourceType} to {node.name}!");
            return false;
        }

        // Resource'u ekle
        bool success = node.AddResource(resourceType, amount);

        // Tamamlanýp tamamlanmadýðýný kontrol et
        if (success && node.IsReadyToComplete())
        {
            CompleteResearch(nodeId);
        }

        return success;
    }

    private void CompleteResearch(string nodeId)
    {
        ResearchNode node = nodeMap[nodeId];
        node.state = ResearchState.Completed;

        Debug.Log($"Research completed: {node.name}!");

        // Yeni research'leri unlock et
        foreach (string unlockId in node.unlocksIds)
        {
            UnlockResearch(unlockId);
        }

        // Research tamamlandýðýnda yapýlacak iþlemler
        OnResearchCompleted(node);
    }

    private void UnlockResearch(string nodeId)
    {
        if (!nodeMap.ContainsKey(nodeId)) return;

        ResearchNode node = nodeMap[nodeId];

        if (node.state == ResearchState.Locked && ArePrerequisitesCompleted(node))
        {
            node.state = ResearchState.Unlocked;
            Debug.Log($"Research unlocked: {node.name}");
        }
    }

    private bool ArePrerequisitesCompleted(ResearchNode node)
    {
        foreach (string prerequisiteId in node.prerequisiteIds)
        {
            if (!nodeMap.ContainsKey(prerequisiteId)) return false;

            if (nodeMap[prerequisiteId].state != ResearchState.Completed)
                return false;
        }

        return true;
    }

    private void CheckAndUpdateUnlocks()
    {
        foreach (var node in allNodes)
        {
            if (node.state == ResearchState.Locked && ArePrerequisitesCompleted(node))
            {
                node.state = ResearchState.Unlocked;
            }
        }
    }

    private void OnResearchCompleted(ResearchNode node)
    {
        switch (node.id)
        {
            case "basic_woodworking":
                // Odun çýkarma verimliliðini artýr
                Debug.Log("Wood extraction efficiency increased!");
                break;
            case "iron_forging":
                // Demir iþleme yetisi kazanýldý
                Debug.Log("Iron forging unlocked!");
                break;
            case "advanced_mining":
                // Maden çýkarma verimliliði artýrýldý
                Debug.Log("Mining efficiency increased!");
                ExtractorLevelUnlocked = 2;
                break;
            case "steel_production":
                // Çelik üretimi açýldý
                Debug.Log("Steel production unlocked!");
                break;
        }
    }

    public ResearchNode GetResearchNode(string nodeId)
    {
        return nodeMap.ContainsKey(nodeId) ? nodeMap[nodeId] : null;
    }

    public List<ResearchNode> GetAvailableResearch()
    {
        List<ResearchNode> available = new List<ResearchNode>();

        foreach (var node in allNodes)
        {
            if (node.state == ResearchState.Unlocked)
            {
                available.Add(node);
            }
        }

        return available;
    }

    public List<ResearchNode> GetCompletedResearch()
    {
        List<ResearchNode> completed = new List<ResearchNode>();

        foreach (var node in allNodes)
        {
            if (node.state == ResearchState.Completed)
            {
                completed.Add(node);
            }
        }

        return completed;
    }

    public List<ResearchNode> GetInProgressResearch()
    {
        List<ResearchNode> inProgress = new List<ResearchNode>();

        foreach (var node in allNodes)
        {
            if (node.state == ResearchState.InProgress)
            {
                inProgress.Add(node);
            }
        }

        return inProgress;
    }

    // Test için kullanýlacak method
    public void TestResearchFlow()
    {
        Debug.Log("=== Research Tree Test ===");

        // Basic Woodworking baþlat
        StartResearch("basic_woodworking");

        // Wood ekle (baþarýlý)
        AddResourceToResearch("basic_woodworking", ResourceType.Wood, 5);
        AddResourceToResearch("basic_woodworking", ResourceType.Wood, 5); // Tamamlanacak

        // Iron Forging baþlat
        StartResearch("iron_forging");

        // Yanlýþ resource eklemeyi dene (baþarýsýz)
        AddResourceToResearch("iron_forging", ResourceType.Wood, 5);

        // Doðru resource'lar ekle
        AddResourceToResearch("iron_forging", ResourceType.Iron, 5);
        AddResourceToResearch("iron_forging", ResourceType.CopperWire, 3);

        // Fazla resource eklemeyi dene (baþarýsýz)
        AddResourceToResearch("iron_forging", ResourceType.Iron, 1);
    }
}