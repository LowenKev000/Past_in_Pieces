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

    // Index used to cycle through the list of hints
    private int hintIndex;


    // Called (usually by a button) to display the next hint in the list
    public void ShowNextHint()
    {
        // Make sure we have a valid classification system and selected animal
        if (classificationSystem == null || classificationSystem.currentAnimal == null)
            return;

        // Get the current animal data
        AnimalData animal = classificationSystem.currentAnimal.animal;

        // Ensure the animal has hints available
        if (animal == null || animal.hints == null || animal.hints.Count == 0)
            return;

        // Loop back to the first hint if we've reached the end
        if (hintIndex >= animal.hints.Count)
            hintIndex = 0;

        // Display the current hint
        ShowHint(animal.hints[hintIndex]);

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