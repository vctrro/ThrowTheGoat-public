using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class LocationConfig
{
    [SerializeField] private string name;
    [SerializeField] private int numberOfLevels;
    [SerializeField] private List<int> levelsRating = new List<int>();

    public string Name { get => name; set => name = value; }
    public int NumberOfLevels { get => numberOfLevels; set => numberOfLevels = value; }
    public List<int> LevelsRating { get => levelsRating; set => levelsRating = value; }
}
