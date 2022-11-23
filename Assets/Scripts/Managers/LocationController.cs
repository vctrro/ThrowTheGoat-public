using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocationController : MonoBehaviour
{
    [SerializeField] private RectTransform goldCabbageCounter;
    [SerializeField] private RectTransform cabbageCounter;
    [SerializeField] private TextMeshProUGUI locationName;
    [SerializeField] private Transform levelsPanel;
    [SerializeField] private Sprite[] ratingWinSprites;
    private GameManager gameManager;
    private GameConfig gameConfig;
    private LocationConfig locationConfig;

    private void Awake()
    {
        gameManager = GameManager.Instance;     
        gameConfig = gameManager.GameConfig;
        if (PlayerPrefs.HasKey($"LocationConfig-L{gameConfig.CurrentLocation}"))
            {
                locationConfig = JsonUtility.FromJson<LocationConfig>(PlayerPrefs.GetString($"LocationConfig-L{gameConfig.CurrentLocation}"));
            }
            else
            {
                locationConfig = new LocationConfig();
                locationConfig.LevelsRating.Add(-1);
            }
    }

    private void Start()
    {
        countersPanelUpdate();
        locationConfig.Name = locationName.text;
        locationConfig.NumberOfLevels = levelsPanel.childCount;
        gameConfig.CurrentLevel = locationConfig.LevelsRating.Count;

        //levels buttons setting up
        for (int n = 0; n < levelsPanel.childCount; n++)
        {
            var levelButton = levelsPanel.GetChild(n);
            levelButton.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = (n+1).ToString();
            if (n < locationConfig.LevelsRating.Count)
            {
                var button = levelButton.gameObject.GetComponent<Button>();
                button.interactable = true;
                int i = n+1;
                button.onClick.AddListener(()=>LoadLevel(i));
                levelButton.gameObject.GetComponent<Image>().color = new Color(1f,1f,1f,1f);
                if (locationConfig.LevelsRating[n] >= 0)
                {
                    var cabbage = levelButton.GetChild(1);
                    cabbage.gameObject.SetActive(true);
                    cabbage.GetChild(0).gameObject.GetComponent<Image>().sprite = ratingWinSprites[locationConfig.LevelsRating[n]];
                }
            }
        }
    }

    public void LoadLevel(int level = 1)
    {
        Debug.Log(level);
        gameConfig.CurrentLevel = level;
        gameManager.LoadScene($"L{gameConfig.CurrentLocation}-S{level}");
    }

    public void GoToMenu()
    {
        AudioManager.Instance.PlayMusic("MainMenu");
        gameManager.LoadScene("MainMenu");
    }

    private void countersPanelUpdate()
    {
        if (gameConfig.GoldCabbageCount > 99) 
        {
            goldCabbageCounter.anchoredPosition = new Vector3(90, 0);
        }
        else
        {
            goldCabbageCounter.anchoredPosition = new Vector3(48, 0);
        }
        if (gameConfig.GoldCabbageCount > 999) goldCabbageCounter.anchoredPosition = new Vector3(125, 0);
        goldCabbageCounter.GetComponentInChildren<TextMeshProUGUI>().text = gameConfig.GoldCabbageCount.ToString();
        if (gameConfig.CabbageCount > 99) 
        {
            cabbageCounter.anchoredPosition = new Vector3(90, 0);
        }
        else
        {
            cabbageCounter.anchoredPosition = new Vector3(48, 0);
        }
        if (gameConfig.CabbageCount > 999) cabbageCounter.anchoredPosition = new Vector3(125, 0);
        cabbageCounter.GetComponentInChildren<TextMeshProUGUI>().text = gameConfig.CabbageCount.ToString();
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetString($"LocationConfig-L{gameConfig.CurrentLocation}", JsonUtility.ToJson(locationConfig));
    }
}
