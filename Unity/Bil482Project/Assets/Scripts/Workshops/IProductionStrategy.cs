using UnityEngine;

public interface IProductionStrategy 
{
    int neededAmount { get; set; }

    void Produce(GameObject inputResource, Workshop workshop);
}
