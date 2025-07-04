using UnityEngine;
using UnityEngine.SceneManagement;
public class BuildingSelection : MonoBehaviour
{
    public GameObject selectedObj;
    [SerializeField] GameObject extractorPrefab;
    [SerializeField] GameObject assemblerPrefab;
    [SerializeField] GameObject cleanerPrefab;
    [SerializeField] GameObject furnacePrefab;
    [SerializeField] GameObject workshopPrefab;
    [SerializeField] GameObject conveyorBeltPrefab;

    void Start()
    {
        selectedObj = null;
    }

    public void SelectWorkshop()
    {
        selectedObj = workshopPrefab;
        SceneManager.LoadScene("GameScene");
    }
    public void SelectExtractor()
    {
        selectedObj = extractorPrefab;
        SceneManager.LoadScene("GameScene");
    }

    public void SelectAssembler()
    {
        selectedObj = assemblerPrefab;
        SceneManager.LoadScene("GameScene");
    }

    public void SelectCleaner()
    {
        selectedObj = cleanerPrefab;
        SceneManager.LoadScene("GameScene");
    }

    public void SelectFurnace()
    {
        selectedObj = furnacePrefab;
        SceneManager.LoadScene("GameScene");
    }

    public void SelectConveyorBelt()
    {
        selectedObj = conveyorBeltPrefab;
        SceneManager.LoadScene("GameScene");
    }
}
