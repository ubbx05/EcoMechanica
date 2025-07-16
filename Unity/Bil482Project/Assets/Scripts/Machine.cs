using UnityEngine;

public abstract class Machine : MonoBehaviour
{
    public abstract bool HasProductToOutput();
    public abstract GameObject GetOutputProduct();
    public abstract bool CanAcceptProduct(GameObject product);
    public abstract void AcceptProduct(GameObject product);
}
