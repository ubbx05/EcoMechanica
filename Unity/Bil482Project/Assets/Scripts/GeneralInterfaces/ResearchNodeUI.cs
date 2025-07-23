using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchNodeUI : MonoBehaviour
{
    public Button researchButton;
    private ResearchTreeUI treeUI;
    public List<ResearchNodeUI> parentNode;
    public bool isUnlocked = false;
    public void Initialize(ResearchTreeUI controller)
    {
        treeUI = controller;
        researchButton.onClick.AddListener(OnClick);
    }
    private void OnClick()
    {
        treeUI.UpdateSelectedTitle(researchButton.image.sprite.name);
    }
}