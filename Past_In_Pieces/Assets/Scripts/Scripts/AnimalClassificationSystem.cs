using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
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

    [Header("Audio")]
    public AudioSource audioSource;        // SFX
    public AudioSource musicSource;        // Background music
    public AudioClip submitSound;
    public AudioClip revealSound;
    public AudioClip perfectScoreSound;
    public AudioClip failScoreSound;

    [Header("Reveal Settings")]
    public float lineDelay = 0.8f;
    public float musicFadeDuration = 0.5f;

    [Header("Score Pop Effect")]
    public float popScale = 1.5f;
    public float popDuration = 0.15f;

    void Start()
    {
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

        // Fade out music
        if (musicSource != null)
            StartCoroutine(FadeOutMusic(musicFadeDuration));

        // Submit sound
        if (audioSource != null && submitSound != null)
            audioSource.PlayOneShot(submitSound);

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

        StartCoroutine(ShowResultsStepByStep(matches, wrong, required, missing, accuracy));
    }

    private IEnumerator ShowResultsStepByStep(int matches, int wrong, int required, int missing, float accuracy)
    {
        resultText.text = "";

        // Wait for music fade
        yield return new WaitForSeconds(musicFadeDuration);

        yield return RevealLine($"{matches} right - {wrong} wrong\n\n");
        yield return RevealLine($"{matches} correct parts\n");
        yield return RevealLine($"{wrong} wrong parts\n");
        yield return RevealLine($"{required} required parts\n");

        if (missing > 0)
        {
            yield return RevealLine($"Missing Parts: {missing}\n");
        }

        if (matches == required && wrong == 0)
        {
            yield return RevealLine("\nPerfect! All parts are correct.\n");
        }

        // Show score immediately after last line
        UpdateScoreUI(accuracy);

        // POP effect if perfect
        if (accuracy >= 100f && scoreText != null)
        {
            StartCoroutine(PopScoreText());
        }

        // Small pause before final sound
        yield return new WaitForSeconds(0.3f);

        if (audioSource != null)
        {
            if (accuracy >= 100f && perfectScoreSound != null)
            {
                audioSource.PlayOneShot(perfectScoreSound);
            }
            else if (failScoreSound != null)
            {
                audioSource.PlayOneShot(failScoreSound);
            }
        }
    }

    private IEnumerator RevealLine(string line)
    {
        resultText.text += line;

        if (audioSource != null && revealSound != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(revealSound);
        }

        yield return new WaitForSeconds(lineDelay);
    }

    private IEnumerator PopScoreText()
    {
        Transform t = scoreText.transform;
        Vector3 originalScale = t.localScale;
        Vector3 targetScale = originalScale * popScale;

        float time = 0f;

        // Scale up
        while (time < popDuration)
        {
            time += Time.deltaTime;
            float tVal = Mathf.SmoothStep(0, 1, time / popDuration);
            t.localScale = Vector3.Lerp(originalScale, targetScale, tVal);
            yield return null;
        }

        time = 0f;

        // Scale back down
        while (time < popDuration)
        {
            time += Time.deltaTime;
            float tVal = Mathf.SmoothStep(0, 1, time / popDuration);
            t.localScale = Vector3.Lerp(targetScale, originalScale, tVal);
            yield return null;
        }

        t.localScale = originalScale;
    }

    private IEnumerator FadeOutMusic(float duration)
    {
        if (musicSource == null)
            yield break;

        float startVolume = musicSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop();
    }

    void UpdateScoreUI(float accuracy)
    {
        if (scoreText != null)
            scoreText.text = $"Accuracy: {accuracy:0}%";
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