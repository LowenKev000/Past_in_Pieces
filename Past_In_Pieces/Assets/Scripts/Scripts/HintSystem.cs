using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class HintSystem : MonoBehaviour
{
    [Header("References")]
    // UI text element where hints will be displayed
    public TextMeshProUGUI hintText;

    // Reference to the system that provides the current animal and its hints
    public AnimalClassificationSystem classificationSystem;

    [Header("Typing Settings")]
    // Time delay between each letter appearing (typing effect speed)
    public float typingSpeed = 0.05f;

    [Header("Audio")]
    // Audio source used to play typing sounds
    public AudioSource audioSource;

    // Sound played while letters are being typed
    public AudioClip typingSound;

    // Keeps track of the currently running typing coroutine so it can be stopped if needed
    private Coroutine typingCoroutine;

    // Stores the randomly selected hints for THIS round (max 3)
    private List<string> selectedHints = new List<string>();

    // Index used to cycle through the selected hints
    private int hintIndex = 0;


    // Called when a new animal/round starts
    // This selects up to 3 random hints and keeps them fixed for the round
    public void PrepareHints()
    {
        // Clear previous hints and reset index
        selectedHints.Clear();
        hintIndex = 0;

        // Make sure we have a valid classification system and selected animal
        if (classificationSystem == null || classificationSystem.currentAnimal == null)
            return;

        // Get the current animal data
        AnimalData animal = classificationSystem.currentAnimal.animal;

        // Ensure the animal has hints available
        if (animal == null || animal.hints == null || animal.hints.Count == 0)
            return;

        // Create a copy of the hints list so we don't modify the original
        List<string> shuffled = new List<string>(animal.hints);

        // Shuffle the list using Fisher-Yates algorithm
        for (int i = 0; i < shuffled.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffled.Count);
            string temp = shuffled[i];
            shuffled[i] = shuffled[randomIndex];
            shuffled[randomIndex] = temp;
        }

        // Determine how many hints to take (max 3, or fewer if not enough exist)
        int count = Mathf.Min(3, shuffled.Count);

        // Store the selected hints
        for (int i = 0; i < count; i++)
        {
            selectedHints.Add(shuffled[i]);
        }
    }


    // Called (usually by a button) to display the next hint
    public void ShowNextHint()
    {
        // Make sure we have a valid classification system and selected animal
        if (classificationSystem == null || classificationSystem.currentAnimal == null)
            return;

        // If hints haven't been prepared yet, prepare them now
        if (selectedHints.Count == 0)
        {
            PrepareHints();
        }

        // If still no hints, exit
        if (selectedHints.Count == 0)
            return;

        // Loop back to the first hint if we've reached the end
        if (hintIndex >= selectedHints.Count)
            hintIndex = 0;

        // Display the current hint
        ShowHint(selectedHints[hintIndex]);

        // Move to the next hint for the next button press
        hintIndex++;
    }


    // Starts displaying a hint with a typing animation
    private void ShowHint(string message)
    {
        // Stop any previous typing animation before starting a new one
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(message));
    }


    // Displays the text one character at a time to create a typing effect
    private IEnumerator TypeText(string message)
    {
        // Clear existing text before starting
        hintText.text = "";

        int counter = 0;

        foreach (char letter in message)
        {
            // Add the next letter to the text
            hintText.text += letter;

            // Play typing sound for non-space characters to avoid awkward silence clicks
            if (audioSource != null && typingSound != null && letter != ' ')
            {
                // Only play sound every second character to reduce audio spam
                if (counter % 2 == 0)
                {
                    // Slight pitch variation makes the sound feel more natural
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(typingSound);
                }
                counter++;
            }

            // Wait before showing the next letter
            yield return new WaitForSeconds(typingSpeed);
        }

        // Reset pitch after typing finishes to avoid affecting other sounds
        if (audioSource != null)
            audioSource.pitch = 1f;
    }
}