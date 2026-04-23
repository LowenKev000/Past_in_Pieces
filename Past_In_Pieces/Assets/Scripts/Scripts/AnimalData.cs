using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AnimalData", menuName = "Animals/Animal Data")]
public class AnimalData : ScriptableObject
{
    public string animalName;

    public Sprite icon;

    [TextArea(3, 10)]
    public List<string> hints = new List<string>();
}