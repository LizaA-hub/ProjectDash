using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;
    public SoundDataManager.soundType startMusic; //can be set for each scene
    [SerializeField]
    private SoundDataManager.Sound[] soundPool;
    private AudioSource source;
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

        source = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //play the start music
    }

    public void PlayMusic(SoundDataManager.soundType type, float fade = 0f, float volume = 1f)
    {
        AudioClip clip = GetClip(type);
        if (clip == null) return;

        source.volume = volume;

        if(fade > 0f)
        {
            if (source.isPlaying) {
                StartCoroutine(FadeOut(fade));
            }
            source.clip = clip;
            source.volume = 0f;
            source.Play();
            StartCoroutine(FadeIn(fade, volume));
        }
        else
        {
            source.clip = clip;
            source.volume = volume;
            source.Play();
        }
        

    }

    public void PlaySound(SoundDataManager.soundType type, float volume = 1f)
    {
        AudioClip clip = GetClip(type);
        if (clip == null) return;

        source.PlayOneShot(clip,volume);
    }

    public void PlaySound(SoundDataManager.soundType type,Vector3 position, float volume = 1f)
    {
        AudioClip clip = GetClip(type);
        if (clip == null) return;

        AudioSource.PlayClipAtPoint(clip, position, volume);
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

    private IEnumerator FadeOut(float delay)
    {
        float countDown = delay;
        float initialVolume = source.volume;
        while (countDown > 0f) {
            source.volume = Mathf.Lerp(initialVolume, 0f, (delay - countDown) / delay);
            countDown -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        source.Stop();
    }

    private IEnumerator FadeIn(float delay, float finalVolume) {
        float countDown = delay;
        float initialVolume = source.volume;
        while (countDown > 0f)
        {
            source.volume = Mathf.Lerp(0f, finalVolume, (delay - countDown) / delay);
            countDown -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }}
}
