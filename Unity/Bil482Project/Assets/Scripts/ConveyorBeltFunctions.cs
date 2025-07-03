using System;
using UnityEngine;

public class ConveyorBeltFunctions : MonoBehaviour
{   
    public static RaycastMouseOver selectedObject = null;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Sadece seçili objede rotasyon yapýlacak
        if (RaycastMouseOver.selectedObject != null && RaycastMouseOver.selectedObject.gameObject == gameObject)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                transform.Rotate(0, 0, 90);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                transform.Rotate(0, 0, -90);
            }
        }
    }
}
