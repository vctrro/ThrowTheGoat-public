using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Localization;

public class SceneController : MonoBehaviour
{
    [System.Serializable] public class GoatIsDeadEvent : UnityEvent<Vector2> {}
    [System.Serializable] public class ThrowGoatEvent : UnityEvent<int> {}
    public GoatIsDeadEvent OnGoatIsDead;
    public ThrowGoatEvent OnThrowGoat;
    public LevelConfig levelConfig;
    public UnityEvent OnHitGoat, OnWin;

    [SerializeField] private TextMeshProUGUI locationAndLevel;
    [SerializeField] private TextMeshProUGUI attempts;
    [SerializeField] private TextMeshProUGUI hitReceived;
    [SerializeField] private TextMeshProUGUI lostGoats;
    [SerializeField] private GameObject cabbagePrefab, winTrigger;
    [SerializeField] private GameObject[] cabbageScale, cabbageRatingScale;
    [SerializeField] private GameObject cabbageRating;
    [SerializeField] private AudioClip ratingZeroClip;
    [SerializeField] private Sprite[] ratingWinSprites;
    [SerializeField] private TextMeshProUGUI attemptsWin;
    [SerializeField] private AudioClip cabbagePlusClip;
    [SerializeField] private Button forwardButton, soundButton;
    [SerializeField] private Sprite[] soundButtonImage;
    private GameManager gameManager;
    private GameConfig gameConfig;
    private LocationConfig locationConfig;
    private string currentSceneName;
    private int throwCount, ratingCounter = 5;
    private GameObject goat, cabbageUI, menuPause, winPanel;
    private TextMeshProUGUI cabbageCounter;
    private Animator textAnimator;

    private void Awake()
    {
        gameManager = GameManager.Instance;     
        gameConfig = gameManager.GameConfig;
        locationConfig = JsonUtility.FromJson<LocationConfig>(PlayerPrefs.GetString($"LocationConfig-L{gameConfig.CurrentLocation}"));
        currentSceneName = $"L{gameConfig.CurrentLocation}-S{gameConfig.CurrentLevel}";

        if (PlayerPrefs.HasKey($"LevelConfig-{currentSceneName}"))
        {
            levelConfig = JsonUtility.FromJson<LevelConfig>(PlayerPrefs.GetString($"LevelConfig-{currentSceneName}"));
        }
        else
        {
            levelConfig = new LevelConfig();
        }
        
        if (locationConfig.LevelsRating.Count < gameConfig.CurrentLevel) locationConfig.LevelsRating.Add(-1);

    }
    
    private void Start()
    {
        OnHitGoat.AddListener(HitGoat);
        OnWin.AddListener(()=>StartCoroutine(Win()));
        OnGoatIsDead.AddListener(GoatIsDead);
        forwardButton.GetComponent<Button>().onClick.AddListener(NextLevel);

        throwCount = 0;

        cabbageUI = GameObject.Find("CabbageUI");
        cabbageCounter = cabbageUI.transform.Find("CabbageCount").GetComponent<TextMeshProUGUI>();
        textAnimator = cabbageUI.GetComponent<Animator>();

        menuPause = GameObject.Find("Canvas").transform.Find("MenuPause").gameObject;
        winPanel = GameObject.Find("Canvas").transform.Find("WinPanel").gameObject;
        goat = GameObject.Find("Goat");

        //Initialize visual elements and counters!
        cabbageCounter.text = gameConfig.CabbageCount.ToString();
        locationAndLevel.text = $"{locationConfig.Name}  # {gameConfig.CurrentLevel}";

        switch (gameConfig.Sounds)
        {
            case Sound.MUSIC:
                soundButton.image.sprite = soundButtonImage[0];
                break;
            case Sound.SOUNDS:
                soundButton.image.sprite = soundButtonImage[1];
                break;
            case Sound.OFF:
                soundButton.image.sprite = soundButtonImage[2];
                break;
        }
    }

    public void ThrowGoat()
    {
        throwCount++;
        OnThrowGoat.Invoke(throwCount);
        if (throwCount == 1)
        {
            levelConfig.AttemptCounter++;
        }
        Debug.Log($"Throw: {throwCount}");
    }

    public void PauseGame()
    {
        //info
        attempts.text = levelConfig.AttemptCounter.ToString();
        hitReceived.text = levelConfig.HitCounter.ToString();
        lostGoats.text = levelConfig.LostGoats.ToString();
        //pause
        Time.timeScale = 0;
        menuPause.SetActive(true);
        menuPause.GetComponent<Animator>().Play("MenuPauseIN");
    }

    public void ResumeGame()
    {
        StartCoroutine(PauseOUT());
        Time.timeScale = 1;
    }

    public void ReloadLevel(bool lostGoat = false)
    {
        if (lostGoat) LostGoat();
        FadeLoadScene(SceneManager.GetActiveScene().name);
    }

    public void LocationMenu(bool lostGoat = false)
    {
        if (throwCount > 0 && lostGoat) LostGoat();
        FadeLoadScene($"L{gameConfig.CurrentLocation}");
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
    //Activate-deactivate any objects with delay
    public void SetActiveDelay(GameObject object1, bool active, float time)
    {
        StartCoroutine(gameManager.SetActiveWithDelay(object1, active, time));
    }

    private IEnumerator Win()
    {
        //Write data!
        var oldRating = levelConfig.Rating;
        if (levelConfig.Rating < ratingCounter)
        {
            //If new rating > 2 open new level
            if (levelConfig.Rating < 2 && ratingCounter >= 2) locationConfig.LevelsRating.Add(-1);
            //Add Gold Cabbage
            if (ratingCounter == 5) gameConfig.GoldCabbageCount++;
            //Add cabbage
            gameConfig.CabbageCount += ratingCounter-oldRating;
            //Update rating
            levelConfig.Rating = ratingCounter;
            locationConfig.LevelsRating[gameConfig.CurrentLevel-1] = ratingCounter;
        }
        
        // Debug.Log($"<color=green> YOU WIN!</color>");
        yield return new WaitForSeconds(1f);
        
        //Freeze Goat
        var body = goat.transform.Find("bone_1");
        body.gameObject.GetComponent<Animator>().enabled = false;
        body.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        //Move Goat
        var winBounds = winTrigger.GetComponent<BoxCollider2D>().bounds;
        body.position = new Vector3(winBounds.center.x, winBounds.max.y + 1f, 0);
        //Goat animations
        goat.GetComponent<Animator>().enabled = true;;
        goat.GetComponent<AudioSource>().enabled = true;
        //Firework
        var firework = winTrigger.transform.Find("Firework");
        firework.Find("MagicPoof1").gameObject.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        firework.Find("MagicPoof2").gameObject.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        firework.Find("MagicPoof3").gameObject.SetActive(true);
        
        //WinPanel
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < oldRating; i++)
        {
            cabbageRatingScale[i].GetComponent<AudioSource>().playOnAwake = false;
            cabbageRatingScale[i].SetActive(true);
            if (i == 1)
            {
                var buttonPanel = forwardButton.transform.parent;
                buttonPanel.Find("Padlock").gameObject.SetActive(false);
                buttonPanel.Find("ForwardButton").gameObject.GetComponent<Button>().interactable = true;
            }
        }
        attemptsWin.text = levelConfig.AttemptCounter.ToString();
        winPanel.SetActive(true);
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.6f);

        //Rating scale
        // yield return StartCoroutine(CabbageBurst(true));
        yield return StartCoroutine(CabbageRatingAppears());

        //Rating number
        cabbageRating.transform.Find("RatingImage").gameObject.GetComponent<Image>().sprite = ratingWinSprites[ratingCounter];
        winPanel.GetComponent<Animator>().Play("WinRating", 0);
        if (ratingCounter < oldRating || ratingCounter < 2)
        {
            cabbageRating.GetComponent<AudioSource>().clip = ratingZeroClip;
        }

        yield return new WaitForSecondsRealtime(0.5f);

        //Get reward
        for (int i = oldRating; i < 5; i++)
        {
            if (cabbageRatingScale[i].activeSelf)
            {
                cabbageRatingScale[i].GetComponent<Animator>().Play("MiniCabbageUIRotation");
                cabbageRatingScale[i].GetComponent<AudioSource>().PlayOneShot(cabbagePlusClip);
                yield return new WaitForSecondsRealtime(0.1f);

                float currentTime = 0f, duration = .3f;
                while (currentTime < duration)
                {
                    currentTime += Time.fixedDeltaTime;
                    cabbageRatingScale[i].transform.position = Vector3.Lerp(cabbageRatingScale[i].transform.position, 
                                                                    cabbageUI.transform.position, currentTime / duration);
                    yield return null;
                }
                cabbageRatingScale[i].SetActive(false);
                StartCoroutine(ChangeCabbageCount(1));
            }
        }
    }

    private void NextLevel()
    {        
        gameConfig.CurrentLevel++;

        FadeLoadScene($"L{gameConfig.CurrentLocation}-S{gameConfig.CurrentLevel}");
    }

    private void HitGoat()
    {
        levelConfig.HitCounter++;
        //UI scale health minus
        StartCoroutine(CabbageBurst());
        ratingCounter = Mathf.Clamp(--ratingCounter, 0, 5);
    }

    private void LostGoat()
    {
        gameConfig.CabbageCount--;
        levelConfig.LostGoats++;
    }

    private void GoatIsDead(Vector2 target)
    {
        LostGoat();
        StartCoroutine(WhenGoatDied(target));
        goat.SetActive(false);
        // gameConfig.DeadGoatCount++;
    }

    private void FadeLoadScene(string scene)
    {
        gameManager.LoadScene(scene);
    }

    private IEnumerator WhenGoatDied(Vector2 targetPosition)
    {
        yield return StartCoroutine(CabbageBurst(true));

        RectTransformUtility
            .ScreenPointToWorldPointInRectangle(GameObject.Find("Canvas")
            .GetComponent<RectTransform>(), cabbageUI.transform.position, Camera.main, out Vector3 cabbageUIposition);

        GameObject twistyCabbage = Instantiate(cabbagePrefab, cabbageUIposition, Quaternion.identity);
        twistyCabbage.GetComponent<TargetJoint2D>().target = targetPosition + Vector2.down*2;
        twistyCabbage.GetComponent<Rigidbody2D>().AddForce(new Vector2((targetPosition.x - twistyCabbage.transform.position.x)*80, 0));
        
        yield return StartCoroutine(ChangeCabbageCount(-1));

        yield return new WaitForSeconds(1.0f);

        Destroy(twistyCabbage);
        yield return new WaitForSeconds(1.0f);

        ReloadLevel();
    }

    private IEnumerator ChangeCabbageCount(int value)
    {
        textAnimator.Play("ChangeCountMin", 1);
        var temp = int.Parse(cabbageCounter.text);
        yield return new WaitForSecondsRealtime(0.2f);

        cabbageCounter.text = (temp + value).ToString();
        textAnimator.Play("ChangeCountMax", 1);
    }

    private IEnumerator CabbageBurst(bool all = false)
    {
        foreach (var item in cabbageScale)
        {
            if (item.GetComponent<Image>().enabled == true)
            {
                item.GetComponent<Animator>().Play("MiniCabbageUIBurst");
                if (all)
                {
                    yield return new WaitForSecondsRealtime(0.15f);
                }
                else
                {
                    break;
                }
            }
        }
    }

    private IEnumerator CabbageRatingAppears()
    {
        for (int i = 0; i < ratingCounter; i++)
        {
            if (cabbageRatingScale[i].activeSelf == false)
            {
                cabbageRatingScale[i].SetActive(true);
                yield return new WaitForSecondsRealtime(0.15f);

                if (i == 1)
                {
                    winPanel.GetComponent<Animator>().Play("WinForwardUnlock", 1);
                }
                if (i == 4)
                {
                    winPanel.GetComponent<Animator>().Play("CabbageGoldAppears", 2);
                    yield return new WaitForSecondsRealtime(1.5f);
                }
            }
        }
    }

    private IEnumerator PauseOUT()
    {
        menuPause.GetComponent<Animator>().Play("MenuPauseOUT");
        yield return new WaitForSeconds(1f);
        menuPause.SetActive(false);        
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetString($"LevelConfig-{currentSceneName}", JsonUtility.ToJson(levelConfig));
        PlayerPrefs.SetString($"LocationConfig-L{gameConfig.CurrentLocation}", JsonUtility.ToJson(locationConfig));
    }
}
