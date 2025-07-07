using UnityEngine;

public class RaycastMouseOver : MonoBehaviour
{
    [SerializeField] private Sprite overSprite;
    [SerializeField] private Sprite notOverSprite;
   
    //MADENLERDE VESA�RE �LK BA�TA KIYAS YAPILACAK Z EKSEN� HANG�S�N�N DAHA Y�KSEKSE O 0 A E��T KABUL ED�LECEK
    private new Collider2D collider2D;
    private SpriteRenderer spriteRenderer;
    private bool isSelected = false;
    public static RaycastMouseOver selectedObject = null;

    void Start()
    {
        collider2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetOver(false);
    }

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        Collider2D hit = Physics2D.OverlapPoint(mousePos2D);

        // Sol t�k ile se�im
        if (Input.GetMouseButtonDown(0))
        {
            if (hit == collider2D)
            {
                if (selectedObject == this)
                {
                    // Ayn� objeye t�kland�ysa se�imi kald�r
                    Deselect();
                }
                else
                {
                    if (selectedObject != null)
                        selectedObject.Deselect();
                    Select();
                }
            }
            else if (selectedObject == this)
            {
                // Ba�ka bir yere t�kland�ysa, se�imi kald�r
                Deselect();
            }
        }

        // Sa� t�k ile se�im kald�rma
        if (Input.GetMouseButtonDown(1) && isSelected)
        {
            Deselect();
        }
    }

    private void Select()
    {
        isSelected = true;
        selectedObject = this;
        spriteRenderer.sprite = overSprite;
        Debug.Log($"Se�ildi: {gameObject.name}");
    }

    private void Deselect()
    {
        isSelected = false;
        if (selectedObject == this)
            selectedObject = null;
        spriteRenderer.sprite = notOverSprite;
        Debug.Log($"Se�im kald�r�ld�: {gameObject.name}");
    }

    private void SetOver(bool isOver)
    {
        if (isOver)
            spriteRenderer.sprite = overSprite;
        else
            spriteRenderer.sprite = notOverSprite;
    }
}