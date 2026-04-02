using UnityEngine;

public class BuildingMenu : MonoBehaviour
{
    public GameObject[] buildingGrids;

    private int currentIndex = 0;

    void Start()
    {
        UpdateActiveObject();
    }

    public void NextObject()
    {
        currentIndex++;
        if (currentIndex >= buildingGrids.Length)
            currentIndex = 0;

        UpdateActiveObject();
    }

    public void PreviousObject()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = buildingGrids.Length - 1;

        UpdateActiveObject();
    }

    public void SwitchTo(int index)
    {
        if (index < 0 || index >= buildingGrids.Length) return;

        currentIndex = index;
        UpdateActiveObject();
    }

    private void UpdateActiveObject()
    {
        for (int i = 0; i < buildingGrids.Length; i++)
        {
            buildingGrids[i].SetActive(i == currentIndex);
        }
    }
}