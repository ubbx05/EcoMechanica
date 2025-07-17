using System;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    Boolean isEmpty = true;
    private GameObject resourcePrefab;
    public int giris = 0;
    public int cikis = 0;
    private float speed = 2.0f;
    private RotatingBuildings rotator;
    private int yon;
    private ConveyorBelt nextConveyorBelt;
    bool flag1 = true;
    bool flag2 = true;
    bool flag3 = true;
    void Start()
    {
        determineNextConveyorBelt();
        if (rotator == null)
        {
            rotator = GetComponent<RotatingBuildings>();
        }
        yon = rotator.transferyonu;
    }

    // Update is called once per frame  
    void Update()
    {
        determineNextConveyorBelt();
        transportingRes();
    }
    private static readonly Vector2Int[] directionOffsets = new Vector2Int[]
    {
        new Vector2Int(1, 0),   // sa�
        new Vector2Int(0, 1),   // yukar�
        new Vector2Int(-1, 0),  // sol
        new Vector2Int(0, -1)   // a�a��
    };
    public void determineNextConveyorBelt()
    {
        Vector2Int offset = directionOffsets[yon % 4];
        Vector2 checkPosition = (Vector2)transform.position + offset;

        Collider2D hit = Physics2D.OverlapPoint(checkPosition);
        if (hit != null)
        {
            ConveyorBelt belt = hit.GetComponent<ConveyorBelt>();
            if (belt != null)
            {
                
                nextConveyorBelt = belt;
                if (flag3 == true) {
                    Debug.Log("Next conveyor belt found");
                    flag3 = false;
                }
                flag1 = true;
                flag2 = true;
            }
            else
            {
                if (flag1 == true)
                {
                    Debug.Log("diger belt bulunamadi");
                    flag1 = false;
                }
            }
        }
        else
        {
            if (flag2 == true)
            {
                flag2 = false;
                Debug.Log("diger hit bulunamadi");
            }
        }
    }

    public void transportingRes()
    {
        Vector3 move = Vector3.zero;
        if (nextConveyorBelt != null && isEmpty == false && resourcePrefab != null && nextConveyorBelt.isEmpty)
        {
            if (rotator.transferyonu == 0) // asagi bak�yor 
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

            //Mevcut kaynak next belte gönderiliyor
            nextConveyorBelt.resourcePrefab = resourcePrefab;

            isEmpty = true;
        }
        else
        {
            isEmpty = false;
        }
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
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
