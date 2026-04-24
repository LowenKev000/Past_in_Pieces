using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AnimalClassificationSystem : MonoBehaviour
{
    [System.Serializable]
    public class AnimalSetup
    {
        public AnimalData animal;
        public int requiredParts;
    }

    [Header("Animal Setup")]
    public List<AnimalSetup> animals = new List<AnimalSetup>();

    [Header("Current Animal (ONLY VARIABLE)")]
    public AnimalSetup currentAnimal;

    [Header("Scene Object To Evaluate")]
    public GameObject targetObject;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public GameObject resultCanvas;
    public TextMeshProUGUI resultText;

    void Start()
    {
        // Hide result UI at start
        if (resultCanvas != null)
            resultCanvas.SetActive(false);

        PickRandomAnimal();
    }

    public void PickRandomAnimal()
    {
        if (animals == null || animals.Count == 0)
            return;

        currentAnimal = animals[Random.Range(0, animals.Count)];
    }

    public void SubmitAnimal()
    {
        if (currentAnimal == null || targetObject == null)
            return;

        if (resultCanvas != null)
            resultCanvas.SetActive(true);

        // Get all placed parts in the scene
        AnimalPart[] parts = targetObject.GetComponentsInChildren<AnimalPart>();

        int matches = 0;
        int totalPlaced = parts.Length;

        foreach (AnimalPart part in parts)
        {
            if (part.animalType == currentAnimal.animal)
            {
                matches++;
            }
        }

        int required = currentAnimal.requiredParts;

        int wrong = totalPlaced - matches;
        int missing = Mathf.Max(0, required - matches);

        float accuracy = 0f;

        if (required > 0)
        {
            accuracy = ((float)(matches - wrong) / required) * 100f;
        }

        accuracy = Mathf.Clamp(accuracy, 0f, 100f);

        // Score is directly based on accuracy
        UpdateScoreUI(accuracy);

        string message = "";

        message += $"{matches} right - {wrong} wrong\n\n";

        message += $"{matches} correct parts\n";
        message += $"{wrong} wrong parts\n";
        message += $"{required} required parts\n";

        if (missing > 0)
        {
            message += $"Missing Parts: {missing}\n";
        }

        if (matches == required && wrong == 0)
        {
            message += "\nPerfect! All parts are correct.";
        }

        ShowResult(message);
    }

    void UpdateScoreUI(float accuracy)
    {
        if (scoreText != null)
            scoreText.text = $"Accuracy: {accuracy:0}%";
    }

    void ShowResult(string message)
    {
        if (resultText != null)
            resultText.text = message;
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}