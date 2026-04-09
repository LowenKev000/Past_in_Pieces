using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class BuildingSystem : MonoBehaviour
{
    [System.Serializable]
    public class TagSpawnRule
    {
        public string objectTag;
        public string spawnPointTag;
    }

    public Camera cam;
    public List<TagSpawnRule> spawnRules = new List<TagSpawnRule>();
    public Transform buildingContainer;

    private GameObject currentBuilding;
    private bool isMoving = false;

    void Update()
    {
        if (currentBuilding == null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
                SelectBuilding();
        }
        else
        {
            MoveToSpawnPoint();

            if (Mouse.current.leftButton.wasPressedThisFrame)
                PlaceBuilding();

            if (Mouse.current.rightButton.wasPressedThisFrame)
                CancelPlacing();
        }
    }

    void SelectBuilding()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            currentBuilding = hit.collider.gameObject;
            currentBuilding.layer = LayerMask.NameToLayer("Ignore Raycast");
            isMoving = true;
        }
    }

    void MoveToSpawnPoint()
    {
        string spawnTag = GetSpawnPointTag(currentBuilding.tag);
        if (spawnTag == null)
        {
            Debug.LogWarning($"[BuildingSystem] No spawn rule found for object tag '{currentBuilding.tag}'.");
            return;
        }

        GameObject spawnPoint = FindObjectWithTagInChildren(spawnTag);
        if (spawnPoint == null)
        {
            Debug.LogWarning($"[BuildingSystem] No object with tag '{spawnTag}' found in scene or children.");
            return;
        }

        currentBuilding.transform.position = spawnPoint.transform.position;
    }

    void PlaceBuilding()
    {
        if (currentBuilding.GetComponent<Collider>() == null)
            currentBuilding.AddComponent<BoxCollider>();

        if (buildingContainer != null)
            currentBuilding.transform.SetParent(buildingContainer, true);

        currentBuilding = null;
        isMoving = false;
    }

    void CancelPlacing()
    {
        if (!isMoving)
            Destroy(currentBuilding);

        currentBuilding = null;
        isMoving = false;
    }

    public void StartPlacing(GameObject prefab)
    {
        if (currentBuilding != null)
            Destroy(currentBuilding);

        currentBuilding = Instantiate(prefab);
        currentBuilding.layer = LayerMask.NameToLayer("Ignore Raycast");
        isMoving = false;

        if (currentBuilding.GetComponent<Collider>() == null)
            currentBuilding.AddComponent<BoxCollider>();
    }

    private string GetSpawnPointTag(string objectTag)
    {
        foreach (TagSpawnRule rule in spawnRules)
        {
            if (rule.objectTag == objectTag)
                return rule.spawnPointTag;
        }
        return null;
    }

    private GameObject FindObjectWithTagInChildren(string tag)
    {
        foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            GameObject found = FindInChildrenRecursive(root.transform, tag);
            if (found != null)
                return found;
        }
        return null;
    }

    private GameObject FindInChildrenRecursive(Transform parent, string tag)
    {
        if (parent.CompareTag(tag))
            return parent.gameObject;

        foreach (Transform child in parent)
        {
            GameObject found = FindInChildrenRecursive(child, tag);
            if (found != null)
                return found;
        }

        return null;
    }
}