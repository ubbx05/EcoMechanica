using UnityEngine;
using UnityEngine.SceneManagement;
public class BuildingSelection : MonoBehaviour
{
    [SerializeField] GameObject extractorPrefab;
    [SerializeField] GameObject assemblerPrefab;
    [SerializeField] GameObject cleanerPrefab;
    [SerializeField] GameObject furnacePrefab;
    [SerializeField] GameObject workshopPrefab;
    [SerializeField] GameObject conveyorBeltPrefab;

    void Start()
    {
        BuildingManager.SelectedPrefab = null;
    }

    public void SelectWorkshop()
    {
        BuildingManager.SelectedPrefab = workshopPrefab;
        SceneManager.LoadScene("GameScene");
    }
    public void SelectExtractor()
    {
        BuildingManager.SelectedPrefab = extractorPrefab;
        SceneManager.LoadScene("GameScene");
    }

    public void SelectAssembler()
    {
        BuildingManager.SelectedPrefab = assemblerPrefab;
        SceneManager.LoadScene("GameScene");
    }

    public void SelectCleaner()
    {
        BuildingManager.SelectedPrefab = cleanerPrefab;
        SceneManager.LoadScene("GameScene");
    }

    public void SelectFurnace()
    {
        BuildingManager.SelectedPrefab = furnacePrefab;
        SceneManager.LoadScene("GameScene");
    }

    public void SelectConveyorBelt()
    {
        BuildingManager.SelectedPrefab = conveyorBeltPrefab;
        SceneManager.LoadScene("GameScene");
    }
}
