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

    // =========================
    // PLACE OBJECT
    // =========================
    public void StartPlacing(GameObject prefab)
    {
        if (prefab == null) return;

        string objectTag = prefab.tag;

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

        // Body clears everything
        if (objectTag == "Body")
        {
            RemoveAllManagedObjects();
        }
        else
        {
            RemoveExistingOfTag(objectTag);
        }

        GameObject placed = Instantiate(prefab);

        placed.transform.position = spawnPoint.transform.position;
        placed.transform.rotation = Quaternion.Euler(
            placed.transform.eulerAngles.x,
            spawnPoint.transform.eulerAngles.y,
            placed.transform.eulerAngles.z
        );

        if (placed.GetComponent<Collider>() == null)
            placed.AddComponent<BoxCollider>();

        if (buildingContainer != null)
            placed.transform.SetParent(buildingContainer, true);

        if (debugMode)
            Debug.Log($"Placed '{placed.name}' at '{spawnPoint.name}'.");
    }

    // =========================
    // REMOVE OBJECT (BY PREFAB)
    // =========================
    public void RemovePart(GameObject prefab)
    {
        if (prefab == null) return;

        string tag = prefab.tag;

        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);

        if (objs.Length == 0)
        {
            if (debugMode)
                Debug.Log($"No objects found with tag '{tag}' to remove.");
            return;
        }

        foreach (GameObject obj in objs)
        {
            if (debugMode)
                Debug.Log($"Removing '{obj.name}'.");

            Destroy(obj);
        }
    }

    // =========================
    // REMOVE ALL
    // =========================
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

    // =========================
    // REMOVE SINGLE TAG
    // =========================
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

    // =========================
    // RULE LOOKUP
    // =========================
    private string GetSpawnPointTag(string objectTag)
    {
        foreach (TagSpawnRule rule in spawnRules)
        {
            if (rule.objectTag == objectTag)
                return rule.spawnPointTag;
        }
        return null;
    }

    // =========================
    // FIND OBJECT BY TAG
    // =========================
    private GameObject FindObjectWithTag(string tag)
    {
        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            GameObject found = FindInChildrenRecursive(root.transform, tag);
            if (found != null) return found;
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