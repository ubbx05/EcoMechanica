using System;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    Boolean isEmpty = true;
    private GameObject resourcePrefab;
    private float speed = 2.0f;
    [SerializeField] private RotatingBuildings rotator;
    private ConveyorBelt nextConveyorBelt;
    void Start()
    {
        determineNextConveyorBelt();
    }

    // Update is called once per frame  
    void Update()
    {
        transportingRes();
    }

    public void determineNextConveyorBelt()
    {
        // cozum dusunulecek
    }

    public void transportingRes()
    {
        Vector3 move = Vector3.zero;
        if (isEmpty == false && resourcePrefab != null && nextConveyorBelt.isEmpty)
        {
            if (rotator.transferyonu == 0) // asagi bakýyor 
            {
                move = new Vector3(0f, 1f, 0f).normalized * speed * Time.deltaTime;
            }
            if (rotator.transferyonu == 1)
            {
                move = new Vector3(1f, 0f, 0f).normalized * speed * Time.deltaTime;
            }
            if (rotator.transferyonu == 2)
            {
                move = new Vector3(0f, -1f, 0f).normalized * speed * Time.deltaTime;
            }
            if (rotator.transferyonu == 3)
            {
                move = new Vector3(-1f, 0f, 0f).normalized * speed * Time.deltaTime;
            }
            resourcePrefab.transform.position += move;
            isEmpty = true;
        }
        else
        {
            isEmpty = false;
        }
    }

    // Yoruldugumdan yarim kaldi isterseniz devam edin su anda ilk basta onceki conveyordan Actionu almasýný saglýcaz
    // urun yolluom sinyali sinyali isEmpty degeriyle karsýlastýrýp bos ise yollayabilirsin sinyali attýrcaz onceki belte 
    // dolu ise hareket etmesini engelleyeceðiz 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Resource"))
        {
            if (isEmpty)
            {
                resourcePrefab = collision.gameObject;
                isEmpty = false;
                Debug.Log("Resource added to conveyor belt: " + resourcePrefab.name);
            }
            else
            {
                Debug.LogWarning("Conveyor belt is already occupied by another resource.");
            }
        }
    }
}
