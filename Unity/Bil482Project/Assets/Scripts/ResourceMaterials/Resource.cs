using UnityEngine;

public abstract class Resource : MonoBehaviour
{
    // Protected veya public olarak tanımlayabilirsiniz
    public string resourceType;

    public string getType()
    {
        return resourceType;
    }
}