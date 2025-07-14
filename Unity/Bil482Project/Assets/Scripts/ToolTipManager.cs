using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager Instance;

    public GameObject toolTipPanel;
    public TMP_Text buildingName;
    public TMP_Text level;
    private RectTransform panelTransform;

    public void Awake()
    {
        Instance = this;
        panelTransform = toolTipPanel.GetComponent<RectTransform>();
        HideToolTip();
    }

    // Update is called once per frame
    

    public void ShowToolTip(string n, string l)
    {
        buildingName.text = n;
        level.text = l;
        toolTipPanel.SetActive(true);
    }
    public void HideToolTip()
    {
        toolTipPanel.SetActive(false);
    }

    public void DeleteSelectingBuilding() 
    { 
        if (RaycastMouseOver.selectedObject != null)
        {
            GameObject toDelete = RaycastMouseOver.selectedObject.gameObject;
            RaycastMouseOver.selectedObject = null;
            Destroy(toDelete);

            toolTipPanel.SetActive(false);
        }
    }
}
