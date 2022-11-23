using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

public enum Sound { MUSIC, SOUNDS, OFF }

public class AudioManager : Singleton<AudioManager>
{
    private GameConfig gameConfig;
    private GameManager gameManager;
    private AudioListener microphone;
    private AudioMixer myMixer;
    private AudioSource currentMusic, nextMusic, goatHit;
    private float transitionTime = 3f;

    public AudioSource GoatHit { get => goatHit; private set => goatHit = value; }

    private void Awake()
    {
        gameManager = GameManager.Instance;
        gameConfig = gameManager.GameConfig;

        microphone = gameObject.AddComponent<AudioListener>();
        currentMusic = gameObject.AddComponent<AudioSource>();
        nextMusic = gameObject.AddComponent<AudioSource>();
        currentMusic.loop = nextMusic.loop = true;
        goatHit = gameObject.AddComponent<AudioSource>();
        goatHit.playOnAwake = false;
        
        Addressables.LoadAssetAsync<AudioMixer>("GameAudioMixer").Completed +=
            (UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<AudioMixer> aMixer) => {
                myMixer = aMixer.Result;
                AudioMixerGroup backgroundsMusic = aMixer.Result.FindMatchingGroups("Backgrounds")[0];
                currentMusic.outputAudioMixerGroup = backgroundsMusic;
                nextMusic.outputAudioMixerGroup = backgroundsMusic;
                goatHit.outputAudioMixerGroup = aMixer.Result.FindMatchingGroups("Backgrounds")[0];
                
                SetSound();
                };

        Addressables.LoadAssetAsync<AudioClip>("goat-hit").Completed +=
                (UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<AudioClip> clip) => {
                    goatHit.clip = clip.Result;
                };
    }

    public void PlayMusic(string clipName)
    {
        if (currentMusic.clip?.name == clipName) return;  //if music has not changed, leave to play
        if (nextMusic.clip?.name == clipName) 
        {
            ChangeMusic();
            Debug.Log($"ExistingAudioClip: {currentMusic.clip.name}");
        }
        else
        {            
            Addressables.LoadAssetAsync<AudioClip>("music-" + clipName).Completed +=
                (UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<AudioClip> clip) => {
                    nextMusic.clip = clip.Result;
                    ChangeMusic();
                    Debug.Log($"LoadedAudioClip: {currentMusic.clip.name}");
                    };
        }
    }

    public void StopMusic(AudioSource audioSource)
    {
        AudioDown(audioSource);
    }

    public void SetSound()
    {
        switch (gameConfig.Sounds)
        {
            case Sound.MUSIC:
                if (Application.platform == RuntimePlatform.Android)
                    {
                        AudioListener.volume = 1f; //костыли для андроид
                    }

                myMixer.SetFloat("masterVolume", 0f);
                myMixer.SetFloat("backgroundVolume", 0f);
                Debug.Log($"Sound: {gameConfig.Sounds}");
                break;
            case Sound.SOUNDS:
                myMixer.SetFloat("backgroundVolume", -80f);
                Debug.Log($"Sound: {gameConfig.Sounds}");
                break;
            case Sound.OFF:
                if (Application.platform == RuntimePlatform.Android)
                {
                    AudioListener.volume = 0f; //костыли для андроид
                }

                myMixer.SetFloat("masterVolume", -80f);
                Debug.Log($"Sound: {gameConfig.Sounds}");
                break;
        }
    }

    private void ChangeMusic()
    {
        StopAllCoroutines();
        AudioUp(nextMusic);
        AudioDown(currentMusic);
        AudioSource temp = currentMusic;
        currentMusic = nextMusic;
        nextMusic = temp;
    }

    private void AudioUp(AudioSource audioSource)
    {
        audioSource.volume = 0;
        audioSource.Play();
        StartCoroutine(FadeAudioSource(audioSource, transitionTime, 1));
    }

    private void AudioDown(AudioSource audioSource)
    {
        audioSource.volume = 1;
        StartCoroutine(FadeAudioSource(audioSource, transitionTime, 0));
    }

    private IEnumerator FadeAudioSource(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.fixedDeltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        if (start > 0) audioSource.Stop();
        yield break;
    }
   
}
