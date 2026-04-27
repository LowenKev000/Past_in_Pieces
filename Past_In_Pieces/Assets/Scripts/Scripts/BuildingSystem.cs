using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BuildingSystem : MonoBehaviour
{
    [System.Serializable]
    public class TagSpawnRule
    {
        [Tooltip("Tag of the prefab that will be placed (e.g. 'Head', 'Leg').")]
        public string objectTag;

        [Tooltip("Tag of the spawn point in the scene where this object should appear.")]
        public string spawnPointTag;
    }

    [Header("Debug")]
    public bool debugMode = false;

    [Header("References")]
    // Parent object to keep the hierarchy clean when spawning parts
    public Transform buildingContainer;

    [Header("Placement Rules")]
    // Defines which object tag belongs to which spawn point tag
    public List<TagSpawnRule> spawnRules = new List<TagSpawnRule>();


    // Called by UI buttons to place a specific prefab in the scene
    public void StartPlacing(GameObject prefab)
    {
        if (prefab == null) return;

        // Get the tag of the prefab (used to find spawn rules and existing objects)
        string objectTag = prefab.tag;

        // Find which spawn point this object should use
        string spawnPointTag = GetSpawnPointTag(objectTag);
        if (spawnPointTag == null)
        {
            if (debugMode)
                Debug.LogWarning($"No spawn rule for tag '{objectTag}'.");
            return;
        }

        // Find the actual spawn point in the scene
        GameObject spawnPoint = FindObjectWithTag(spawnPointTag);
        if (spawnPoint == null)
        {
            if (debugMode)
                Debug.LogWarning($"No spawn point found for tag '{spawnPointTag}'.");
            return;
        }

        // If placing the body, remove all other parts first
        if (objectTag == "Body")
        {
            RemoveAllManagedObjects();
        }
        else
        {
            // Otherwise, ensure only one object of this type exists
            RemoveExistingOfTag(objectTag);
        }

        // Create the new object
        GameObject placed = Instantiate(prefab);

        // Move it to the correct position and align rotation with the spawn point
        placed.transform.position = spawnPoint.transform.position;
        placed.transform.rotation = Quaternion.Euler(
            placed.transform.eulerAngles.x,
            spawnPoint.transform.eulerAngles.y,
            placed.transform.eulerAngles.z
        );

        // Ensure the object has a collider (useful for interaction)
        if (placed.GetComponent<Collider>() == null)
            placed.AddComponent<BoxCollider>();

        // Parent the object under a container to keep the scene organized
        if (buildingContainer != null)
            placed.transform.SetParent(buildingContainer, true);

        if (debugMode)
            Debug.Log($"Placed '{placed.name}' at '{spawnPoint.name}'.");
    }


    // Removes all objects that are part of the building system (used when placing a new body)
    public void RemovePart(GameObject prefab)
    {
        if (prefab == null) return;

        // Use the prefab's tag to find matching objects in the scene
        string tag = prefab.tag;

        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);

        if (objs.Length == 0)
        {
            if (debugMode)
                Debug.Log($"No objects found with tag '{tag}' to remove.");
            return;
        }

        // Destroy all matching objects
        foreach (GameObject obj in objs)
        {
            if (debugMode)
                Debug.Log($"Removing '{obj.name}'.");

            Destroy(obj);
        }
    }


    // Removes every object defined in the spawn rules (used when resetting the build)
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


    // Ensures that only one object with a specific tag exists by removing the oldest one
    private void RemoveExistingOfTag(string tag)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
        if (objs.Length == 0) return;

        GameObject oldest = objs[0];

        // Find the oldest instance (lowest instance ID)
        foreach (GameObject obj in objs)
        {
            if (obj.GetInstanceID() < oldest.GetInstanceID())
                oldest = obj;
        }

        if (debugMode)
            Debug.Log($"Removing oldest '{oldest.name}'.");

        Destroy(oldest);
    }


    // Looks up which spawn point tag belongs to a given object tag
    private string GetSpawnPointTag(string objectTag)
    {
        foreach (TagSpawnRule rule in spawnRules)
        {
            if (rule.objectTag == objectTag)
                return rule.spawnPointTag;
        }
        return null;
    }


    // Searches the scene for the first object with the given tag (including children)
    private GameObject FindObjectWithTag(string tag)
    {
        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            GameObject found = FindInChildrenRecursive(root.transform, tag);
            if (found != null) return found;
        }
        return null;
    }


    // Recursively checks children to find a matching tag
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