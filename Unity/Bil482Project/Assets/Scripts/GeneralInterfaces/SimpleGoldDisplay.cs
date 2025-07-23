using UnityEngine;
using TMPro;

/// <summary>
/// GameController'daki Gold de�erini basit �ekilde g�sterir
/// </summary>
public class SimpleGoldDisplay : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TMP_Text goldText;              // Gold miktar�n� g�steren text
    [SerializeField] private GameController gameController;  // GameController referans�

    void Start()
    {
        // GameController'� bul e�er assign edilmemi�se
        if (gameController == null)
        {
            gameController = FindAnyObjectByType<GameController>();
        }

        if (gameController == null)
        {
            Debug.LogError("GameController bulunamad�!");
            return;
        }

        if (goldText == null)
        {
            Debug.LogError("Gold Text referans� atanmam��!");
            return;
        }
    }

    void Update()
    {
        // Gold text'ini g�ncelle
        if (gameController != null && goldText != null)
        {
            goldText.text = "Gold: " + gameController.Gold;
        }
    }
}