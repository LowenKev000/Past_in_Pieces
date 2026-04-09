using UnityEngine;
using UnityEngine.UI;

public class BuildingMenu : MonoBehaviour
{
    public GameObject[] buildingGrids;
    public ScrollRect scrollRect;

    private int currentIndex = 0;

    void Start()
    {
        UpdateActiveObject();
    }

    public void NextObject()
    {
        currentIndex = (currentIndex + 1) % buildingGrids.Length;
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

        scrollRect.content = buildingGrids[currentIndex].GetComponent<RectTransform>();
        scrollRect.verticalNormalizedPosition = 1f;
    }
}