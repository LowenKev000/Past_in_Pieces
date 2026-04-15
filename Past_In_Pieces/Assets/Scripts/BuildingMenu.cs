using UnityEngine;
using UnityEngine.UI;

public class BuildingMenu : MonoBehaviour
{
    [Header("Debug")]
    [Tooltip("Enable debug logs for menu switching and state changes.")]
    public bool debugMode = false;

    [Header("UI References")]
    [Tooltip("Array of building grid UI panels.")]
    public GameObject[] buildingGrids;

    [Tooltip("ScrollRect used for navigating the active grid.")]
    public ScrollRect scrollRect;

    [Header("State")]
    [Tooltip("Index of the currently active building grid.")]
    private int currentIndex = 0;

    void Start()
    {
        // Ensure the correct grid is shown on startup
        UpdateActiveObject();

        if (debugMode)
            Debug.Log("[BuildingMenu] Initialized at index: " + currentIndex);
    }

    public void NextObject()
    {
        // Move to next grid, loop back if at end
        currentIndex = (currentIndex + 1) % buildingGrids.Length;

        if (debugMode)
            Debug.Log("[BuildingMenu] Switched to next index: " + currentIndex);

        UpdateActiveObject();
    }

    public void PreviousObject()
    {
        // Move to previous grid, loop to last if below zero
        currentIndex--;

        if (currentIndex < 0)
            currentIndex = buildingGrids.Length - 1;

        if (debugMode)
            Debug.Log("[BuildingMenu] Switched to previous index: " + currentIndex);

        UpdateActiveObject();
    }

    public void SwitchTo(int index)
    {
        // Safely switch to a specific grid index
        if (index < 0 || index >= buildingGrids.Length)
        {
            if (debugMode)
                Debug.LogWarning("[BuildingMenu] Invalid index: " + index);
            return;
        }

        currentIndex = index;

        if (debugMode)
            Debug.Log("[BuildingMenu] Switched to index: " + currentIndex);

        UpdateActiveObject();
    }

    private void UpdateActiveObject()
    {
        // Enable only the currently selected grid
        for (int i = 0; i < buildingGrids.Length; i++)
        {
            buildingGrids[i].SetActive(i == currentIndex);
        }

        // Update scroll view to match current grid
        scrollRect.content = buildingGrids[currentIndex].GetComponent<RectTransform>();
        scrollRect.verticalNormalizedPosition = 1f;

        if (debugMode)
            Debug.Log("[BuildingMenu] Updated active grid: " + currentIndex);
    }
}