using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEditor;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Settings;

public class MenuController : MonoBehaviour
{
    [System.Serializable] public class LanguageSelected : UnityEvent<string> {}
    public LanguageSelected OnLanguageSelected;
    [SerializeField] private Transform locationsPanel;
    [SerializeField] private RectTransform goldCabbageCounter;
    [SerializeField] private RectTransform cabbageCounter;
    [SerializeField] private Button resumeButton, soundButton;
    [SerializeField] private Sprite[] soundButtonImage;
    [SerializeField] private Animator namePanelAnimator, languagePanelAnimator, settingPanelAnimator;
    [SerializeField] private TMP_InputField goatName;
    [SerializeField] private int levelUnlockPrice = 50;
    private GameManager gameManager;
    private GameConfig gameConfig;
    
    private void Awake()
    {
        Debug.Log($"Initialize menu");
        gameManager = GameManager.Instance;
        gameConfig = gameManager.GameConfig;
    }

    private void Start()
    {
        Debug.Log($"Start menu");
        AudioManager.Instance.PlayMusic("MainMenu");
        OnLanguageSelected.AddListener(SetLanguage);

        countersPanelUpdate();

        if (gameConfig.CurrentLevel == 0) resumeButton.interactable = false;

        //locations buttons setting up
        for (int n = 0; n < locationsPanel.childCount; n++)
        {
            var locationButton = locationsPanel.GetChild(n);
            var button = locationButton.gameObject.GetComponent<Button>();
            int i = n+1;
            button.onClick.AddListener(()=>LoadLocation(i));
            if (gameConfig.LocationsOpen.Contains(n+1))
            {
                locationButton.gameObject.GetComponent<Image>().color = new Color(1f,1f,1f,1f);
                locationButton.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = new Color(0.55f,0.86f,1f,1f);
                locationButton.GetChild(1).gameObject.SetActive(false);
                button.interactable = true;
            }
        }
        
        //Set Goat name
        goatName.text = gameConfig.GoatName;
    }

    public void SettingsButtonClicked()
    {
        settingPanelAnimator.SetBool("vertical open", !settingPanelAnimator.GetBool("vertical open"));
        if (namePanelAnimator.GetBool("horizontal open")) namePanelAnimator.SetBool("horizontal open", false);
        if (languagePanelAnimator.GetBool("horizontal open")) languagePanelAnimator.SetBool("horizontal open", false);
    }
    
    public void NameButtonClicked()
    {
        namePanelAnimator.SetBool("horizontal open", !namePanelAnimator.GetBool("horizontal open"));
        if (languagePanelAnimator.GetBool("horizontal open")) languagePanelAnimator.SetBool("horizontal open", false);
    }

    public void LanguageButtonClicked()
    {
        languagePanelAnimator.SetBool("horizontal open", !languagePanelAnimator.GetBool("horizontal open"));
        if (namePanelAnimator.GetBool("horizontal open")) namePanelAnimator.SetBool("horizontal open", false);
    }

    public void SoundButtonClicked()
    {
        switch (gameConfig.Sounds)
        {
            case Sound.MUSIC:
                gameConfig.Sounds = Sound.SOUNDS;
                soundButton.image.sprite = soundButtonImage[1];
                AudioManager.Instance.SetSound();
                break;
            case Sound.SOUNDS:
                gameConfig.Sounds = Sound.OFF;
                soundButton.image.sprite = soundButtonImage[2];
                AudioManager.Instance.SetSound();
                break;
            case Sound.OFF:
                gameConfig.Sounds = Sound.MUSIC;
                soundButton.image.sprite = soundButtonImage[0];
                AudioManager.Instance.SetSound();
                break;
        }
    }

    public void UnlockLocation(int locationNumber)
    {
        Transform locationButton = locationsPanel.GetChild(locationNumber-1);
        Transform pricePanel = locationButton.Find("PricePanel");
        GameObject padlock = pricePanel.Find("Padlock").gameObject;
        if (gameConfig.GoldCabbageCount >= levelUnlockPrice)
        {
            /* LocalizedStringDatabase stringDatabase = LocalizationSettings.Instance.GetStringDatabase();
 
            TableReference table = "Strings";
            TableEntryReference key = $"LOCATION{locationNumber}";
            
            stringDatabase.GetLocalizedStringAsync(table, key).Completed += 
                (UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<string> op) =>
                {
                    string localizedStringResult = op.Result;
                    Debug.Log($"Open \"{localizedStringResult}\" for {levelUnlockPrice} GoldCabbage");
                }; */
            
            gameConfig.LocationsOpen.Add(locationNumber);
            gameConfig.GoldCabbageCount -= levelUnlockPrice;
            countersPanelUpdate();
            //Remove Padlock and PricePanel
            padlock.GetComponent<Animator>().Play("PadlockOpen");
            SetActiveWithDelay(pricePanel.gameObject, false, 0.6f);
            //location buttons setting up
            locationButton.gameObject.GetComponent<Image>().color = new Color(1f,1f,1f,1f);
            locationButton.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = new Color(0.55f,0.86f,1f,1f);
            locationButton.gameObject.GetComponent<Button>().interactable = true;
        }
        else
        {
            padlock.GetComponent<Animator>().Play("PadlockNoAccess");         
        }
    }

    public void LoadLocation(int location)
    {
        gameConfig.CurrentLocation = location;

        AudioManager.Instance.PlayMusic("L" + gameConfig.CurrentLocation);
        FadeLoadScene($"L{gameConfig.CurrentLocation}");
    }

    public void ResumeGame()
    {
        AudioManager.Instance.PlayMusic("L" + gameConfig.CurrentLocation);
        FadeLoadScene($"L{gameConfig.CurrentLocation}-S{gameConfig.CurrentLevel}");
    }

    public void SetLanguage(string code)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(code);
        gameConfig.GameLocale = code;
    }

    public void GoatNameChanged()
    {
        gameConfig.GoatName = goatName.text;
    }

    private void FadeLoadScene(string scene)
    {
        gameManager.LoadScene(scene);
    }

    public void TimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    private void SetActiveWithDelay(GameObject object1, bool active, float time)
    {
        StartCoroutine(gameManager.SetActiveWithDelay(object1, active, time));
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

    public void Sale(string cabbage)
    {
        switch (cabbage)
        {
            case "Gold":
                Debug.Log($"Buy 100 Gold cabbage");
                gameConfig.GoldCabbageCount += 100;
                break;
            
            case "Green":
                Debug.Log($"Buy 500 Green cabbage");
                gameConfig.CabbageCount += 500;
                break;

            default:
                Debug.Log($"Buy {cabbage} cabbage");
                break;
        }

        countersPanelUpdate();        
    }

    public void ApplicationQuit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}