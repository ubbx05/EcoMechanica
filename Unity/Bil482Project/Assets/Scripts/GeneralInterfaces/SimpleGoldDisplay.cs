using UnityEngine;
using TMPro;

/// <summary>
/// GameController'daki Gold deðerini basit þekilde gösterir
/// </summary>
public class SimpleGoldDisplay : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TMP_Text goldText;              // Gold miktarýný gösteren text
    [SerializeField] private GameController gameController;  // GameController referansý

    void Start()
    {
        // GameController'ý bul eðer assign edilmemiþse
        if (gameController == null)
        {
            gameController = FindAnyObjectByType<GameController>();
        }

        if (gameController == null)
        {
            Debug.LogError("GameController bulunamadý!");
            return;
        }

        if (goldText == null)
        {
            Debug.LogError("Gold Text referansý atanmamýþ!");
            return;
        }
    }

    void Update()
    {
        // Gold text'ini güncelle
        if (gameController != null && goldText != null)
        {
            goldText.text = "Gold: " + gameController.Gold;
        }
    }
}