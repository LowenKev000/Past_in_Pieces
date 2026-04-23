using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AnimalClassificationSystem : MonoBehaviour
{
    [Header("Animal Setup")]
    public List<AnimalData> animals = new List<AnimalData>();

    [Header("Current Animal (ONLY VARIABLE)")]
    public AnimalData currentAnimal;

    [Header("Scene Object To Evaluate")]
    public GameObject targetObject;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public GameObject resultCanvas;
    public TextMeshProUGUI resultText;

    private int score = 0;

    void Start()
    {
        if (resultCanvas != null)
            resultCanvas.SetActive(false);

        PickRandomAnimal();
        UpdateScoreUI();
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

        float accuracy = GetMatchAccuracy(targetObject);

        int gainedScore = Mathf.RoundToInt(accuracy);

        score += gainedScore;

        ShowResult($"Accuracy: {accuracy:0}% (+{gainedScore})");

        UpdateScoreUI();
    }

    public float GetMatchAccuracy(GameObject parentObject)
    {
        AnimalPart[] parts = parentObject.GetComponentsInChildren<AnimalPart>();

        int total = 0;
        int matches = 0;

        foreach (AnimalPart part in parts)
        {
            total++;

            if (part.animalType == currentAnimal)
            {
                matches++;
            }
        }

        if (total == 0)
            return 0f;

        return (float)matches / total * 100f;
    }

    void ShowResult(string message)
    {
        if (resultText != null)
            resultText.text = message;
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
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