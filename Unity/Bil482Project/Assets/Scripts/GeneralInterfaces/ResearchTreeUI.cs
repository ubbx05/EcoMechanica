using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ResearchTreeUI : MonoBehaviour
{
    public GameObject researchTreePanel;
    public Transform contentParent;
    public TMP_Text selectedNodeTitle;
    public Button startResearchButton;
    public Button cancelResearchButton;
    public List<ResearchNodeUI> allNodeUIs = new List<ResearchNodeUI>(); // Tüm butonlar
    public void ToggleResearchTreePanel()
    {
        if (researchTreePanel != null)
        {
            bool isActive = researchTreePanel.activeSelf;
            researchTreePanel.SetActive(!isActive);
        }
    }

    public void ClosePanel()
    {
        if (researchTreePanel != null)
        {
            researchTreePanel.SetActive(false);
        }
    }

    private void Start()
    {
        // Tüm butonlarý initialize et
        foreach (var nodeUI in allNodeUIs)
        {
            nodeUI.Initialize(this);
        }
    }

    public void UpdateSelectedTitle(string newTitle)
    {
        selectedNodeTitle.text = newTitle;
    }
}