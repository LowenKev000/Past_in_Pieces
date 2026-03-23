using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingSystem : MonoBehaviour
{
    public Camera cam;
    public LayerMask groundLayer;

    public bool useGrid = true;
    public float gridSize = 1f;

    private GameObject currentBuilding;

    void Update()
    {
        if (currentBuilding == null) return;

        FollowMouse();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlaceBuilding();
        }
    }

    void FollowMouse()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 pos = hit.point;

            if (useGrid)
            {
                pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
                pos.y = Mathf.Round(pos.y / gridSize) * gridSize;
                pos.z = Mathf.Round(pos.z / gridSize) * gridSize;
            }

            currentBuilding.transform.position = pos;
        }
    }

    void PlaceBuilding()
    {
        currentBuilding = null;
    }

    public void StartPlacing(GameObject prefab)
    {
        if (currentBuilding != null)
        {
            Destroy(currentBuilding);
        }

        currentBuilding = Instantiate(prefab);
    }
}