using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(SoundManager)) as SoundManager;

                if (_instance == null)
                {
                    //Debug.LogError("No Active!");
                }
            }
            return _instance;
        }
    }

    // Common
    public AudioSource audioSource;
    public AudioSource uiSpeaker;

    [Header("- Volume")]
    public float bgmVolume;
    public float effectVolume;

    [Space(20)]
    [Header("- Sounds")]
    public AudioClip bgm;
    public AudioClip uiClickSound;

    public void OnEnable()
    {
        audioSource.clip = bgm;
        audioSource.Stop();
        audioSource.Play();

        SaveData.Instance.LoadVolumeData();
    }

    public void SetVolume(float bgmVol, float efxVol)
    {
        bgmVolume = bgmVol;
        effectVolume = efxVol;
        audioSource.volume = bgmVolume;
        uiSpeaker.volume = effectVolume;

        SaveData.Instance.SaveVolumeData();
    }

    public void ActiveUiClickSound(AudioClip clip = null)
    {
        uiSpeaker.volume = effectVolume;
        if (clip == null)
        {
            uiSpeaker.PlayOneShot(uiClickSound);
        }
        else
        {
            uiSpeaker.PlayOneShot(clip);
        }
    }

    private void OnApplicationQuit()
    {
        SaveData.Instance.SaveVolumeData();
    }
}
