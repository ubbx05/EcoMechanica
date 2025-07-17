using UnityEngine;

public class RotatingBuildings : MonoBehaviour
{
    public static RaycastMouseOver selectedObject = null;
    public int transferyonu = 0; // sað = 0 , aþaðý  = 3 , sol = 2 , yukarý = 1d Q ile gezdiðim sýrayla yazdým
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
            if (Input.GetKeyDown(KeyCode.Q))
            {
                transferyonu += 1;
                if (transferyonu >= 4)
                {
                    transferyonu = transferyonu - 4;
                }
                transform.Rotate(0, 0, 90);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                transferyonu -= 1;
                if (transferyonu < 0)
                {
                    transferyonu = transferyonu + 4;
                    
                }
                transform.Rotate(0, 0, -90);
            }
        }

        kontrol();
    }

    public int GetTransferYonu()
    {
        return transferyonu;
    }

    public void SetTransferYonu(int yon)
    {
        transferyonu = yon;
    }

    public void kontrol()
    {
        if (transferyonu >= 4)
        {
            transferyonu = transferyonu - 4;
        }
        if (transferyonu < 0)
        {
            transferyonu = transferyonu + 4;

        }
    }
}
