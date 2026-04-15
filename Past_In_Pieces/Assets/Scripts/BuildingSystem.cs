using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BuildingSystem : MonoBehaviour
{
    [System.Serializable]
    public class TagSpawnRule
    {
        [Tooltip("Tag of the building prefab that will be placed.")]
        public string objectTag;

        [Tooltip("Tag of the spawn point this object should snap to.")]
        public string spawnPointTag;
    }

    [Header("Debug")]
    [Tooltip("Enable debug logs for placement system.")]
    public bool debugMode = false;

    [Header("References")]
    [Tooltip("Optional camera reference.")]
    public Camera cam;

    [Tooltip("Parent container for all placed buildings.")]
    public Transform buildingContainer;

    [Header("Placement Rules")]
    [Tooltip("Rules mapping object tags to spawn point tags.")]
    public List<TagSpawnRule> spawnRules = new List<TagSpawnRule>();

    private GameObject currentBuilding;
    private bool isMoving = false;

    void Update()
    {
        if (currentBuilding == null) return;

        MoveToSpawnPoint();
    }

    void MoveToSpawnPoint()
    {
        string spawnTag = GetSpawnPointTag(currentBuilding.tag);
        if (spawnTag == null)
        {
            if (debugMode) Debug.LogWarning("Spawn tag not found for object tag: " + currentBuilding.tag);
            return;
        }

        GameObject spawnPoint = FindObjectWithTagInChildren(spawnTag);
        if (spawnPoint == null)
        {
            if (debugMode) Debug.LogWarning("Spawn point not found with tag: " + spawnTag);
            return;
        }

        // Snap position to spawn point
        currentBuilding.transform.position = spawnPoint.transform.position;

        // Align Y rotation only
        Vector3 currentRot = currentBuilding.transform.eulerAngles;
        float targetY = spawnPoint.transform.eulerAngles.y;

        currentBuilding.transform.rotation = Quaternion.Euler(
            currentRot.x,
            targetY,
            currentRot.z
        );

        if (debugMode)
            Debug.Log("Snapped building: " + currentBuilding.name + " to " + spawnPoint.name);

        FinalizePlacement();
    }

    void FinalizePlacement()
    {
        RemoveExistingOfSameTag(currentBuilding.tag);

        // Ensure collider exists
        if (currentBuilding.GetComponent<Collider>() == null)
            currentBuilding.AddComponent<BoxCollider>();

        // Parent to container if assigned
        if (buildingContainer != null)
            currentBuilding.transform.SetParent(buildingContainer, true);

        if (debugMode)
            Debug.Log("Finalized placement: " + currentBuilding.name);

        currentBuilding = null;
        isMoving = false;
    }

    public void StartPlacing(GameObject prefab)
    {
        if (currentBuilding != null)
            Destroy(currentBuilding);

        currentBuilding = Instantiate(prefab);

        // Ignore raycasts while placing
        currentBuilding.layer = LayerMask.NameToLayer("Ignore Raycast");

        isMoving = true;

        // Ensure collider exists for placement logic
        if (currentBuilding.GetComponent<Collider>() == null)
            currentBuilding.AddComponent<BoxCollider>();

        if (debugMode)
            Debug.Log("Started placing: " + prefab.name);
    }

    void RemoveExistingOfSameTag(string tag)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(tag))
        {
            if (obj != currentBuilding)
            {
                if (debugMode)
                    Debug.Log("Removing existing: " + obj.name);

                Destroy(obj);
            }
        }
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
        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
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