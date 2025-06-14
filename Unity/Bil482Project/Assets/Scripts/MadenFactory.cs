using UnityEngine;

public class MadenFactory : MonoBehaviour
{
    private GameObject madenPrefab;

    public MadenFactory(GameObject prefab)
    {
        madenPrefab = prefab;
    }

    public GameObject Uret(Vector3 position)
    {
        // Kare (Cube) obje oluştur
        GameObject kare = GameObject.CreatePrimitive(PrimitiveType.Cube);
        kare.transform.position = position;

        // Sorting Layer'ı "madenler" olarak ayarla (sadece SpriteRenderer için çalışır)
        var spriteRenderer = kare.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = "Madenler";
        }

        Debug.Log("Maden üretildi: ");
        return kare;
    }
}
