using UnityEngine;
using UnityEngine.SceneManagement;
public class BuildingSelection : MonoBehaviour
{
    GameObject selectedObj;
    [SerializeField] GameObject extractorPrefab;
    [SerializeField] GameObject assemblerPrefab;
    [SerializeField] GameObject cleanerPrefab;
    [SerializeField] GameObject furnacePrefab;
    [SerializeField] GameObject conveyorBeltPrefab;

    void Start()
    {
        selectedObj = null;
    }

    public void SelectExtractor()
    {
        selectedObj = Instantiate(extractorPrefab);
        SceneManager.LoadScene("GameScene");
    }

    public void SelectAssembler()
    {
        selectedObj = Instantiate(assemblerPrefab);
        SceneManager.LoadScene("GameScene");
    }

    public void SelectCleaner()
    {
        selectedObj = Instantiate(cleanerPrefab);
        SceneManager.LoadScene("GameScene");
    }

    public void SelectFurnace()
    {
        selectedObj = Instantiate(furnacePrefab);
        SceneManager.LoadScene("GameScene");
    }

    public void SelectConveyorBelt()
    {
        selectedObj = Instantiate(conveyorBeltPrefab);
        SceneManager.LoadScene("GameScene");
    }
}
