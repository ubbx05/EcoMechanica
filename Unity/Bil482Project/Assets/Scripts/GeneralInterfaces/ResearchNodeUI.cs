using UnityEngine;
using UnityEngine.UI;

public class ResearchNodeUI : MonoBehaviour
{
    public Button researchButton;
    private ResearchTreeUI treeUI;

    public void Initialize(ResearchTreeUI controller)
    {
        treeUI = controller;
        researchButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // Butonun sprite ad�n� g�nder
        treeUI.UpdateSelectedTitle(researchButton.image.sprite.name);
    }
}