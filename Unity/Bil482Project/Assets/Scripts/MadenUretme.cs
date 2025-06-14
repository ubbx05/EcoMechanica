using UnityEngine;

public class MadenUretme : MonoBehaviour
{
    public GameObject madenPrefab;
    public float uretimSuresi = 2f;
    private float timer = 0f;
    private MadenFactory madenFactory;
    public float yon = 0f;

    void Start()
    {
        madenFactory = new MadenFactory(madenPrefab); // MonoBehaviour değil, normal class
    }

    void Update()
    {
            timer += Time.deltaTime;
            while (timer >= uretimSuresi)
            {
                Vector3 spawnPosition;
                if (yon == 0) // YUKARI
                    spawnPosition = transform.position + transform.up * 1f;
                else if (yon == 1) // SAĞ
                    spawnPosition = transform.position + transform.right * 1f;
                else if (yon == 2) // SOL
                    spawnPosition = transform.position + -transform.right * 1f;
                else // AŞAĞI
                    spawnPosition = transform.position + -transform.up * 1f;

                madenFactory.Uret(spawnPosition);
                timer -= uretimSuresi;
            }
        
    }
}
