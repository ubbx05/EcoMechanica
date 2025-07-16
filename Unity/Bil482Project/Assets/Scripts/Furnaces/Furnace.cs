using System.Collections.Generic;
using UnityEngine;

public class Furnace : Machine
{
    public Queue<Resource> inventory = new Queue<Resource>(3);

    public override void AcceptProduct(GameObject product)
    {
        throw new System.NotImplementedException();
    }

    public override bool CanAcceptProduct(GameObject product)
    {
        throw new System.NotImplementedException();
    }

    public override GameObject GetOutputProduct()
    {
        throw new System.NotImplementedException();
    }

    public override bool HasProductToOutput()
    {
        throw new System.NotImplementedException();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
