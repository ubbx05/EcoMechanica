using System;
using UnityEngine;

public abstract class Resource : MonoBehaviour
{

    [Header("Resource Properties")]
    [SerializeField] public string resourceName;
    [SerializeField] public ResourceType resourceType;
    [SerializeField] public int resourceIncome;

    // Events
    public static event Action<Resource> OnResourceCreated;
    public static event Action<Resource> OnResourceDestroyed;
    //public static event Action<Resource> OnResourceProcessed;
    //public static event Action<Resource> OnResourceReachedDestination;

    // Properties
    public string ResourceName => resourceName;
    public ResourceType Type => resourceType;
    public int ResourceIncome => resourceIncome;

    protected virtual void Awake()
    {
        InitializeResource();
    }

    protected virtual void Start()
    {
        OnResourceCreated?.Invoke(this);
    }

    protected virtual void OnDestroy()
    {
        OnResourceDestroyed?.Invoke(this);
    }

    // Abstract methods - her kaynak türü için özelleştirilmeli
    protected abstract void InitializeResource();

    public virtual void ConsumeResource()
    {
        // Ana binaya kaynak teslim etme mantığı
        //GameManager.Instance?.AddResource(this);
        Destroy(gameObject);
    }


    // Protected veya public olarak tanımlayabilirsiniz

    //public string resourceType;
    //public int resourceIncome;

    public ResourceType getType()
    {
        return resourceType;
    }

    public int getIncome()
    {
        return resourceIncome;
    }
}
/* ResearchTree içinde tanımlanmış
public enum ResourceType
{
    hamBakir,
    hamDemir,
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
*/