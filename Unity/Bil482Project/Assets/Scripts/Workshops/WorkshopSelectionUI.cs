using UnityEngine;

public class WorkshopSelectionUI : MonoBehaviour
{
    private Workshop selectedWorkshop;

    public void OpenForWorkshop(Workshop workshop)
    {
        selectedWorkshop = workshop;
        gameObject.SetActive(true);
    }

    public void OnRecipeSelected(int recipeIndex)
    {
        switch (recipeIndex)
        {
            case 0:
                selectedWorkshop.SetStrategy(new PlankCraftStrategy());
                break;
            case 1:
                //selectedWorkshop.SetStrategy(new CopperWireCraftStrategy());
                break;
        }
    }
}