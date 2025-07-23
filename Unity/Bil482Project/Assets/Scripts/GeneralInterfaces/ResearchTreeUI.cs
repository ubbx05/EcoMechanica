using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ResearchTreeUI : MonoBehaviour
{
    public GameObject researchTreePanel;
    public Transform contentParent;
    public TMP_Text selectedNodeTitle;
    public Button unlockResearchButton;
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

    public void Awake()
    {
        UpdateNodeColors();
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
    public void Unlock()
    {
        //O an týklanan düðümün kilidini aç
        foreach (var nodeUI in allNodeUIs)
        {
            if (nodeUI.researchButton.image.sprite.name == selectedNodeTitle.text)
            {
                //parentlarýnýn kilidinin açýk olduðunu kontrol et
                bool allParentsUnlocked = true;
                foreach (var parent in nodeUI.parentNode)
                {
                    if (!parent.isUnlocked)
                    {
                        allParentsUnlocked = false;
                        break;
                    }
                }
                if (!allParentsUnlocked)
                {
                    Debug.Log("Tüm parent düðümlerinin kilidi açýk deðil.");
                    return; // Eðer tüm parent düðümlerinin kilidi açýk deðilse, çýk
                }
                else
                {
                    nodeUI.isUnlocked = true; // Düðümün kilidini aç
                    UpdateNodeColors(); // Renkleri güncelle
                    Debug.Log($"{nodeUI.researchButton.image.sprite.name} düðümünün kilidi açýldý.");
                }
            }
        }
    }

    public void UpdateNodeColors()
    {
        // Tüm düðümlerin renklerini güncelle
        foreach (var nodeUI in allNodeUIs)
        {
            if (!nodeUI.isUnlocked)
            {
                nodeUI.researchButton.image.color = Color.black; // Kilidi kapalý düðüm rengi
            }
            else
            {
                //Normal sprite rengini al
                nodeUI.researchButton.image.color = Color.white; // Kilidi açýk düðüm rengi
            }
        }
    }
}