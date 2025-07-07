using UnityEngine;

public class RaycastMouseOver : MonoBehaviour
{
    [SerializeField] private Sprite overSprite;
    [SerializeField] private Sprite notOverSprite;
   
    //MADENLERDE VESAÝRE ÝLK BAÞTA KIYAS YAPILACAK Z EKSENÝ HANGÝSÝNÝN DAHA YÜKSEKSE O 0 A EÞÝT KABUL EDÝLECEK
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

        // Sol týk ile seçim
        if (Input.GetMouseButtonDown(0))
        {
            if (hit == collider2D)
            {
                if (selectedObject == this)
                {
                    // Ayný objeye týklandýysa seçimi kaldýr
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
                // Baþka bir yere týklandýysa, seçimi kaldýr
                Deselect();
            }
        }

        // Sað týk ile seçim kaldýrma
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
        Debug.Log($"Seçildi: {gameObject.name}");
    }

    private void Deselect()
    {
        isSelected = false;
        if (selectedObject == this)
            selectedObject = null;
        spriteRenderer.sprite = notOverSprite;
        Debug.Log($"Seçim kaldýrýldý: {gameObject.name}");
    }

    private void SetOver(bool isOver)
    {
        if (isOver)
            spriteRenderer.sprite = overSprite;
        else
            spriteRenderer.sprite = notOverSprite;
    }
}