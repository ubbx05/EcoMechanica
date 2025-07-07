using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject buildingSelectionPanel;

    public GameObject extractorPrefab;
    public GameObject assemblerPrefab;
    public GameObject cleanerPrefab;
    public GameObject workshopPrefab;
    public GameObject furnacePrefab;
    public GameObject conveyorBeltPrefab;
    public BuildingManager BuildingManager;

    void Awake()
    {
        if (buildingSelectionPanel != null)
            buildingSelectionPanel.SetActive(false); // oyun baþlar baþlamaz panel kapalý olsun
    }


    public void ToggleBuildingPanel()
    {
        if (buildingSelectionPanel != null)
        {
            bool isActive = buildingSelectionPanel.activeSelf;
            buildingSelectionPanel.SetActive(!isActive);
        }
    }
    public void SelectExtractor()
    {
        BuildingManager.SetSelectedPrefab(extractorPrefab);
        buildingSelectionPanel.SetActive(false);
    }

    public void SelectAssembler()
    {
        BuildingManager.SetSelectedPrefab(assemblerPrefab);
        buildingSelectionPanel.SetActive(false);
    }

    public void SelectCleaner()
    {
        BuildingManager.SetSelectedPrefab(cleanerPrefab);
        buildingSelectionPanel.SetActive(false);
    }

    public void SelectWorkshop()
    {
        BuildingManager.SetSelectedPrefab(workshopPrefab);
        buildingSelectionPanel.SetActive(false);
    }
    public void SelectFurnace()
    {
        BuildingManager.SetSelectedPrefab(furnacePrefab);
        buildingSelectionPanel.SetActive(false);
    }
    public void SelectConveyorBelt()
    {
        BuildingManager.SetSelectedPrefab(conveyorBeltPrefab);
        buildingSelectionPanel.SetActive(false);
    }
}