using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;
    public SoundDataManager.soundType startMusic; //can be set for each scene
    [SerializeField]
    private SoundDataManager.Sound[] soundPool;
    [SerializeField]
    private AudioSource musicSource, SfxSource;

    #region Unity Functions
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        if(startMusic != SoundDataManager.soundType.None)
        {
            PlayMusic(startMusic,0.5f);
        }
    }
    #endregion
    #region Public Functions
    public void PlayMusic(SoundDataManager.soundType type, float fade = 0f)
    {
        AudioClip clip = GetClip(type);
        if (clip == null) return;


        float volume = GameManagerV2.instance.currentDatas.music / 10f;
        

        if(fade > 0f)
        {
            if ((musicSource.clip != null) && musicSource.isPlaying) {
                StartCoroutine(FadeOut(fade, volume, clip));
            }
            else
            {
                musicSource.clip = clip;
                musicSource.volume = 0f;
                musicSource.Play();
                StartCoroutine(FadeIn(fade, volume));
            }
            
        }
        else
        {
            musicSource.clip = clip;
            musicSource.volume = volume;
            musicSource.Play();
        }


    }

    public void PlaySound(SoundDataManager.soundType type, float volume = 1f)
    {
        AudioClip clip = GetClip(type);
        if (clip == null) return;

        volume *= GameManagerV2.instance.currentDatas.sfx / 10f;
        SfxSource.PlayOneShot(clip,volume);
    }

    public void PlaySound(SoundDataManager.soundType type,Vector3 position, float volume = 1f)
    {
        AudioClip clip = GetClip(type);
        if (clip == null) return;

        volume *= GameManagerV2.instance.currentDatas.sfx / 10f;
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    #endregion
    #region Private Functions

    public void ModifyMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    private AudioClip GetClip(SoundDataManager.soundType type) {
        foreach (var sound in soundPool)
        {
            if(sound.type == type)
            {
                return sound.possibleClips[Random.Range(0, sound.possibleClips.Length)];
            }
        }
        Debug.Log("Sound Manager : " + type + " not found in sound pool");
        return null;
    }

    private IEnumerator FadeOut(float delay,float volume, AudioClip clip)
    {
        float initialVolume = musicSource.volume;
        for (float timer = 0f; timer < delay; timer += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(initialVolume, 0f, timer / delay);
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.clip = clip;
        musicSource.Play();
        StartCoroutine(FadeIn(delay, volume));
    }

    private IEnumerator FadeIn(float delay, float finalVolume) {
        for (float timer = 0f; timer < delay; timer += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, finalVolume, timer / delay);
            yield return null;
        }

        musicSource.volume = finalVolume;
    }
    #endregion
}
