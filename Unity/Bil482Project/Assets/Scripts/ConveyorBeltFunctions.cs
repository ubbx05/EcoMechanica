using System;
using UnityEngine;

public class ConveyorBeltFunctions : MonoBehaviour
{   
    public static RaycastMouseOver selectedObject = null;
    public int transferyonu = 0;
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
                transferyonu += 90;
                if(transferyonu >= 360)
                {
                    transferyonu = transferyonu-360;
                }
                transform.Rotate(0, 0, 90);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                transferyonu -= 90;
                if (transferyonu < 0)
                {
                    transferyonu = transferyonu + 360;
                }
                transform.Rotate(0, 0, -90);
            }
        }
    }
}
