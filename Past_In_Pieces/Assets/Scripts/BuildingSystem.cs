using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BuildingSystem : MonoBehaviour
{
    [System.Serializable]
    public class TagSpawnRule
    {
        [Tooltip("Prefab tag to place.")]
        public string objectTag;

        [Tooltip("Target spawn point tag.")]
        public string spawnPointTag;
    }

    [Header("Debug")]
    public bool debugMode = false;

    [Header("References")]
    public Transform buildingContainer;

    [Header("Placement Rules")]
    public List<TagSpawnRule> spawnRules = new List<TagSpawnRule>();

    // Entry point for placing objects
    public void StartPlacing(GameObject prefab)
    {
        string objectTag = prefab.tag;

        // Get matching spawn rule
        string spawnPointTag = GetSpawnPointTag(objectTag);
        if (spawnPointTag == null)
        {
            if (debugMode)
                Debug.LogWarning($"No spawn rule for tag '{objectTag}'.");
            return;
        }

        GameObject spawnPoint = FindObjectWithTag(spawnPointTag);
        if (spawnPoint == null)
        {
            if (debugMode)
                Debug.LogWarning($"No spawn point found for tag '{spawnPointTag}'.");
            return;
        }

        // Special case: "Body" clears all managed objects
        if (objectTag == "Body")
        {
            RemoveAllManagedObjects();
        }
        else
        {
            RemoveExistingOfTag(objectTag);
        }

        // Instantiate and position object
        GameObject placed = Instantiate(prefab);
        placed.transform.position = spawnPoint.transform.position;
        placed.transform.rotation = Quaternion.Euler(
            placed.transform.eulerAngles.x,
            spawnPoint.transform.eulerAngles.y,
            placed.transform.eulerAngles.z
        );

        // Ensure collider exists
        if (placed.GetComponent<Collider>() == null)
            placed.AddComponent<BoxCollider>();

        // Parent to container if assigned
        if (buildingContainer != null)
            placed.transform.SetParent(buildingContainer, true);

        if (debugMode)
            Debug.Log($"Placed '{placed.name}' at '{spawnPoint.name}'.");
    }

    // Removes all objects defined in spawn rules
    private void RemoveAllManagedObjects()
    {
        foreach (TagSpawnRule rule in spawnRules)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(rule.objectTag);
            foreach (GameObject obj in objs)
            {
                if (debugMode)
                    Debug.Log($"Destroying '{obj.name}'.");
                Destroy(obj);
            }
        }
    }

    // Ensures only one object per tag exists
    private void RemoveExistingOfTag(string tag)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
        if (objs.Length == 0) return;

        GameObject oldest = objs[0];
        foreach (GameObject obj in objs)
        {
            if (obj.GetInstanceID() < oldest.GetInstanceID())
                oldest = obj;
        }

        if (debugMode)
            Debug.Log($"Removing oldest '{oldest.name}'.");

        Destroy(oldest);
    }

    // Get spawn point tag for object
    private string GetSpawnPointTag(string objectTag)
    {
        foreach (TagSpawnRule rule in spawnRules)
        {
            if (rule.objectTag == objectTag)
                return rule.spawnPointTag;
        }
        return null;
    }

    // Find object in scene by tag (recursive search)
    private GameObject FindObjectWithTag(string tag)
    {
        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            GameObject found = FindInChildrenRecursive(root.transform, tag);
            if (found != null) return found;
        }
        return null;
    }

    // Recursive child search helper
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