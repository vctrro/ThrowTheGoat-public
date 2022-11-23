using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class GameConfig
{
    [SerializeField] private Sound sounds = Sound.MUSIC;
    [SerializeField] private string gameLocale;
    [SerializeField] private int cabbageCount = 15;
    [SerializeField] private int goldCabbageCount = 0;
    [SerializeField] private string goatName = "Goat";
    [SerializeField] private int deadGoatCount = 0;
    [SerializeField] private List<int> locationsOpen = new List<int> {1};
    [SerializeField] private int currentLocation = 0;
    [SerializeField] private int currentLevel = 0;


    public Sound Sounds { get => sounds; set => sounds = value; }
    public int CabbageCount 
    {
        get => cabbageCount; 
        set 
        {
            if (cabbageCount > value) deadGoatCount++;
            cabbageCount = value; 
        }
    }
    public string GameLocale { get => gameLocale; set => gameLocale = value; }
    public int GoldCabbageCount { get => goldCabbageCount; set => goldCabbageCount = value; }
    public string GoatName { get => goatName; set => goatName = value; }
    public int DeadGoatCount { get => deadGoatCount; set => deadGoatCount = value; }
    public List<int> LocationsOpen { get => locationsOpen; set => locationsOpen = value; }
    public int CurrentLocation { get => currentLocation; set => currentLocation = value; }
    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }
}
