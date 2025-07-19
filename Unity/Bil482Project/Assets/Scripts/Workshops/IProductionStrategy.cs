using UnityEngine;

public interface IProductionStrategy 
{
    void Produce(GameObject inputResource, Workshop workshop);
}
