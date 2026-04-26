using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class HintSystem : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI hintText;
    public AnimalClassificationSystem classificationSystem;

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip typingSound;

    private Coroutine typingCoroutine;
    private int hintIndex;

    public void ShowNextHint()
    {
        if (classificationSystem == null || classificationSystem.currentAnimal == null)
            return;

        AnimalData animal = classificationSystem.currentAnimal.animal;

        if (animal == null || animal.hints == null || animal.hints.Count == 0)
            return;

        if (hintIndex >= animal.hints.Count)
            hintIndex = 0;

        ShowHint(animal.hints[hintIndex]);
        hintIndex++;
    }

    private void ShowHint(string message)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(message));
    }

    private IEnumerator TypeText(string message)
    {
        hintText.text = "";

        int counter = 0;

        foreach (char letter in message)
        {
            hintText.text += letter;

            // Play typing sound (skip spaces and reduce spam)
            if (audioSource != null && typingSound != null && letter != ' ')
            {
                if (counter % 2 == 0) // play every 2nd character
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(typingSound);
                }
                counter++;
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        // Reset pitch after typing
        if (audioSource != null)
            audioSource.pitch = 1f;
    }
}