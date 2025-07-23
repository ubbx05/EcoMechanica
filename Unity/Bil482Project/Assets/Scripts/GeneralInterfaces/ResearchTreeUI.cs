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
    public List<ResearchNodeUI> allNodeUIs = new List<ResearchNodeUI>(); // T�m butonlar
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
        // T�m butonlar� initialize et
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
        //O an t�klanan d���m�n kilidini a�
        foreach (var nodeUI in allNodeUIs)
        {
            if (nodeUI.researchButton.image.sprite.name == selectedNodeTitle.text)
            {
                //parentlar�n�n kilidinin a��k oldu�unu kontrol et
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
                    Debug.Log("T�m parent d���mlerinin kilidi a��k de�il.");
                    return; // E�er t�m parent d���mlerinin kilidi a��k de�ilse, ��k
                }
                else
                {
                    nodeUI.isUnlocked = true; // D���m�n kilidini a�
                    UpdateNodeColors(); // Renkleri g�ncelle
                    Debug.Log($"{nodeUI.researchButton.image.sprite.name} d���m�n�n kilidi a��ld�.");
                }
            }
        }
    }

    public void UpdateNodeColors()
    {
        // T�m d���mlerin renklerini g�ncelle
        foreach (var nodeUI in allNodeUIs)
        {
            if (!nodeUI.isUnlocked)
            {
                nodeUI.researchButton.image.color = Color.black; // Kilidi kapal� d���m rengi
            }
            else
            {
                //Normal sprite rengini al
                nodeUI.researchButton.image.color = Color.white; // Kilidi a��k d���m rengi
            }
        }
    }
}