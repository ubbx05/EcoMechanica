using UnityEngine;

public abstract class Resource : MonoBehaviour
{
    // Protected veya public olarak tanımlayabilirsiniz
    public string resourceType;
    public int resourceIncome;

    public string getType()
    {
        return resourceType;
    }

    public int getIncome()
    {
        return resourceIncome;
    }
}