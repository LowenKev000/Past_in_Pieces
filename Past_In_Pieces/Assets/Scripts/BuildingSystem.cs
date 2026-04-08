using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingSystem : MonoBehaviour
{
    public Camera cam;
    public LayerMask groundLayer;
    public LayerMask buildingLayer;

    public bool useGrid = true;
    public float gridSize = 1f;

    public float snapRadius = 3f;

    private GameObject currentBuilding;
    private bool isMoving = false;

    void Update()
    {
        if (currentBuilding == null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                SelectBuilding();
            }
        }
        else
        {
            MoveBuilding();

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                PlaceBuilding();
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                CancelPlacing();
            }
        }
    }

    void SelectBuilding()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, buildingLayer))
        {
            currentBuilding = hit.collider.gameObject;
            currentBuilding.layer = LayerMask.NameToLayer("Ignore Raycast");
            isMoving = true;
        }
    }

    void MoveBuilding()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 pos = hit.point;

            if (useGrid)
            {
                pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
                pos.z = Mathf.Round(pos.z / gridSize) * gridSize;
            }

            GameObject[] snapPoints = GameObject.FindGameObjectsWithTag("SnapPoint");
            Transform bestSnap = null;
            float closestDist = Mathf.Infinity;

            foreach (GameObject snap in snapPoints)
            {
                if (snap.transform.IsChildOf(currentBuilding.transform)) continue;

                float dist = Vector3.Distance(pos, snap.transform.position);
                if (dist < snapRadius && dist < closestDist)
                {
                    closestDist = dist;
                    bestSnap = snap.transform;
                }
            }

            if (bestSnap != null)
            {
                pos = bestSnap.position;
                currentBuilding.transform.rotation = bestSnap.rotation;
            }

            currentBuilding.transform.position = pos;
        }
    }

    void PlaceBuilding()
    {
        currentBuilding.layer = Mathf.RoundToInt(Mathf.Log(buildingLayer.value, 2));
        if (currentBuilding.GetComponent<Collider>() == null)
        {
            currentBuilding.AddComponent<BoxCollider>();
        }

        currentBuilding = null;
        isMoving = false;
    }

    void CancelPlacing()
    {
        if (isMoving)
        {
            currentBuilding.layer = Mathf.RoundToInt(Mathf.Log(buildingLayer.value, 2));
        }
        else
        {
            Destroy(currentBuilding);
        }

        currentBuilding = null;
        isMoving = false;
    }

    public void StartPlacing(GameObject prefab)
    {
        if (currentBuilding != null)
        {
            Destroy(currentBuilding);
        }

        currentBuilding = Instantiate(prefab);
        currentBuilding.layer = LayerMask.NameToLayer("Ignore Raycast");
        isMoving = false;

        if (currentBuilding.GetComponent<Collider>() == null)
            currentBuilding.AddComponent<BoxCollider>();
    }
}