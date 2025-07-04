using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour
{
    public Queue<Resource> inventory = new Queue<Resource>(3);
    void melt()
    {
        Resource resource = inventory.Dequeue();
        if (resource != null)
        {
            
        }
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
