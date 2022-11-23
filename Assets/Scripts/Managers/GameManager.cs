using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;

//TODO: 

public class GameManager : Singleton<GameManager>
{
    // (Optional) Prevent non-singleton constructor use.
    // protected GameManager() {}
    private GameConfig gameConfig;
    private Animator fade;

    public GameConfig GameConfig { get => gameConfig; }

    private void Awake()
    {
        InitializeGame();
        
    }

    private void Start()
    {
        Addressables.InstantiateAsync("FadeScreen", gameObject.transform).Completed +=
            (UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> fadeScreen) => {
            fade = fadeScreen.Result.GetComponent<Animator>();
            };
    }

    private void OnApplicationFocus(bool focusStatus)
    {
        //think
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        //save data
        if (pauseStatus)
        {
            PlayerPrefs.SetString("GameConfig", JsonUtility.ToJson(gameConfig));
            PlayerPrefs.Save();
        }
    }

    private void OnApplicationQuit()
    {
        //save data
        PlayerPrefs.SetString("GameConfig", JsonUtility.ToJson(gameConfig));
        PlayerPrefs.Save();
    }

    public void LoadScene(string scene)
    {
        if (SceneUtility.GetBuildIndexByScenePath(scene) >= 0)
        {
            StartCoroutine(FadeLoadScene(scene));
        }
        else
        {
            StartCoroutine(FadeLoadScene($"L{gameConfig.CurrentLocation}"));
        }
    }

    public IEnumerator SetActiveWithDelay(GameObject object1, bool active, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        object1.SetActive(active);
    }

    private void InitializeGame()
    {
        if (PlayerPrefs.HasKey("GameConfig"))
        {
            gameConfig = JsonUtility.FromJson<GameConfig>(PlayerPrefs.GetString("GameConfig"));
            if (gameConfig.GameLocale != null) StartCoroutine(SetLocale());
        }
        else
        {
            gameConfig = new GameConfig();
            PlayerPrefs.SetString("GameConfig", JsonUtility.ToJson(gameConfig));
        }

        AudioManager audioManager = AudioManager.Instance;

        Debug.Log($"Initialize Game");
        Debug.Log($"Goat Name       {GameConfig.GoatName}");
        Debug.Log($"Goats dead      {GameConfig.DeadGoatCount}");
        Debug.Log($"Cabbage count   {GameConfig.CabbageCount}");
        Debug.Log($"Gold Cabbage count   {GameConfig.GoldCabbageCount}");
        Debug.Log($"Location        {GameConfig.CurrentLocation}");
        Debug.Log($"Level           {GameConfig.CurrentLevel}");
        Debug.Log($"Sound:          {GameConfig.Sounds}");
        Debug.Log($"Locale:          {GameConfig.GameLocale}");
    }

    private IEnumerator AsyncLoadScene(string scene)
    {
        AsyncOperation load = SceneManager.LoadSceneAsync(scene);
        while (!load.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator FadeLoadScene(string scene)
    {
        fade.Play("FadeIN");
        yield return new WaitForSecondsRealtime(0.4f);
        yield return AsyncLoadScene(scene);
        fade.Play("FadeOUT");
        Time.timeScale = 1;
    }

    private IEnumerator SetLocale()
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(gameConfig.GameLocale);
    }


    private void OnDestroy()
    {
        // Debug.LogWarning($"{gameObject} has been destroyed");
    }
}
