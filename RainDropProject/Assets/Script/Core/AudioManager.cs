using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;


public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSFX;
    [SerializeField] private AudioSource audioMusic;

    [Header("Audio Libraries")]
    [SerializeField] private List<Music> Musics = new List<Music>();
    [SerializeField] private List<SFX> SFXs = new List<SFX>();

    //private Slider SliderMusic;
    //private Slider SliderSFX;

    //[SerializeField] private float _SliderAudioIntervail = 1;
    //private float _SliderAudioT;

    #region Singleton
    private static AudioManager _instance;

    // Proprietà per accedere all'istanza del singleton
    public static AudioManager InstanceGM
    {
        get
        {
            // Se l'istanza non esiste, la crea
            if (_instance == null)
            {
                // Cerca un'istanza esistente nella scena
                _instance = FindObjectOfType<AudioManager>();

                // Se non esiste, crea un nuovo GameObject con questo script
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(AudioManager).Name);
                    _instance = singletonObject.AddComponent<AudioManager>();
                }
            }
            return _instance;
        }
    }

    // Assicura che l'istanza venga distrutta correttamente se necessario
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    // Assicura che l'istanza del singleton non venga distrutta al cambio di scena
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }


    }
    #endregion

    private void OnEnable()
    {
        EventManager.Instance.Subscribe("PlayMusicByStringGlobal", PlayMusicByString);
        EventManager.Instance.Subscribe("PlayMusicByClipGlobal", PlayMusicByClip);
        EventManager.Instance.Subscribe("PlaySFXByStringGlobal", PlaySFXByString);
        EventManager.Instance.Subscribe("PlaySFXByClipGlobal", PlaySFXByClip);
        EventManager.Instance.Subscribe("PlayMusicByStringLocal", PlayMusicLocalByString);
        EventManager.Instance.Subscribe("PlayMusicByClipLocal", PlayMusicLocalByClip);
        EventManager.Instance.Subscribe("PlaySFXByStringLocal", PlaySFXLocalByString);
        EventManager.Instance.Subscribe("PlaySFXByClipLocal", PlaySFXLocalByClip);
        EventManager.Instance.Subscribe("PlaySFXByClipLocal", PlaySFXLocalByClip);
        EventManager.Instance.Subscribe("OnSFXSliderChanged", SetSfxVolume);
        EventManager.Instance.Subscribe("OnMusicSliderChanged", SetMusicVolume);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe("PlayMusicByStringGlobal", PlayMusicByString);
        EventManager.Instance.Unsubscribe("PlayMusicByClipGlobal", PlayMusicByClip);
        EventManager.Instance.Unsubscribe("PlaySFXByStringGlobal", PlaySFXByString);
        EventManager.Instance.Unsubscribe("PlaySFXByClipGlobal", PlaySFXByClip);
        EventManager.Instance.Unsubscribe("PlayMusicByStringLocal", PlayMusicLocalByString);
        EventManager.Instance.Unsubscribe("PlayMusicByClipLocal", PlayMusicLocalByClip);
        EventManager.Instance.Unsubscribe("PlaySFXByStringLocal", PlaySFXLocalByString);
        EventManager.Instance.Unsubscribe("OnSFXSliderChanged", SetSfxVolume);
        EventManager.Instance.Unsubscribe("OnMusicSliderChanged", SetMusicVolume);
    }

    private void Start()
    {
        audioMusic.clip = Musics[0].clip;
        audioMusic.Play();
    }
    /// <summary>
    /// Play the music clip from the Musics List using the Audio Source of the Manager
    /// </summary>
    private void PlayMusicByString(object[] args)
    {//global music
        string clipName = (string)args[0];

        audioMusic.Stop();
        foreach(Music music in Musics)
        {
            if (music != null)
            {
                if (music.name == clipName)
                {
                    audioMusic.clip = music.clip;
                    audioMusic.Play();
                    return;
                }
            }
        }
    }
    /// <summary>
    /// Play the music clip from the Musics List using the Audio Source of the Manager
    /// </summary>
    private void PlayMusicByClip(object[] args)
    {//global music
        AudioClip ac = (AudioClip)args[0];

        audioMusic.Stop();
        foreach (Music music in Musics)
        {
            if (music != null)
            {
                if (music.clip == ac)
                {
                    audioMusic.clip = ac;
                    audioMusic.Play();
                    return;
                }
            }
        }
    }
    /// <summary>
    /// Playe the sfx clip from the sfxs list using the Audio Source of the Manager
    /// </summary>
    private void PlaySFXByString(object[] args)
    {//global sfx
        string clipName = (string)args[0];

        foreach (SFX sfx in SFXs)
        {
            if (sfx != null)
            {
                if (sfx.Name == clipName)
                {
                    audioSFX.PlayOneShot(sfx.clip);
                    return;
                }
            }
        }
    }
    /// <summary>
    /// Playe the sfx clip from the sfxs list using the Audio Source of the Manager
    /// </summary>
    private void PlaySFXByClip(object[] args)
    {//global sfx
        AudioClip ac = (AudioClip)args[0];

        foreach (SFX sfx in SFXs)
        {
            if (sfx != null)
            {
                if (sfx.clip == ac)
                {
                    audioSFX.PlayOneShot(ac);
                    return;
                }
            }
        }
    }
    /// <summary>
    /// Play the music in the musics lists with the given audio source
    /// </summary>
    private void PlayMusicLocalByString(object[] args)
    {//local music
        string clipName = (string)args[0];
        AudioSource audioS = null;
        if ((AudioSource)args[1] == null)
        {
            audioS = audioMusic;
        }
        else
        {
            audioS = (AudioSource)args[1];
        }

        if (audioS != null && clipName != null)
        {
            foreach (Music music in Musics)
            {
                if (music != null)
                {
                    if (music.name == clipName)
                    {
                        audioS.clip = music.clip;
                        audioS.Play();
                        return;
                    }
                }
            }
        }
    }
    /// <summary>
    /// Play the music in the musics lists with the given audio source
    /// </summary>
    private void PlayMusicLocalByClip(object[] args)
    {//local music
        AudioClip ac = (AudioClip)args[0];
        AudioSource audioS = null;
        if ((AudioSource)args[1] == null)
        {
            audioS = audioMusic;
        }
        else
        {
            audioS = (AudioSource)args[1];
        }

        if (audioS != null && ac != null)
        {
            foreach (Music music in Musics)
            {
                if (music != null)
                {
                    if (music.clip == ac)
                    {
                        audioS.clip = ac;
                        audioS.Play();
                        return;
                    }
                }
            }
        }
    }
    /// <summary>
    /// Play the sfx in the sfxs lists with the given audio source
    /// </summary>
    private void PlaySFXLocalByString(object[] args)
    {//local music
        string clipName = (string)args[0];
        AudioSource audioS = null;
        if ((AudioSource)args[1] == null)
        {
            audioS = audioSFX;
        }
        else
        {
            audioS = (AudioSource)args[1];
        }

        if (audioS != null && clipName != null)
        {
            foreach (SFX sfx in SFXs)
            {
                if (sfx != null)
                {
                    if (sfx.Name == clipName)
                    {
                        audioS.PlayOneShot(sfx.clip);
                        return;
                    }
                }
            }
        }
    }
    /// <summary>
    /// Play the sfx in the sfxs lists with the given audio source
    /// </summary>
    private void PlaySFXLocalByClip(object[] args)
    {//local sound effect
        AudioClip ac = (AudioClip)args[0];
        AudioSource audioS = null;
        if ((AudioSource)args[1] == null)
        {
            audioS = audioSFX;
        }
        else
        {
            audioS = (AudioSource)args[1];
        }

        if (audioS != null && ac != null)
        {
            foreach (SFX sfx in SFXs)
            {
                if (sfx != null)
                {
                    if (sfx.clip == ac)
                    {
                        audioS.PlayOneShot(ac);
                        return;
                    }
                }
            }
        }
    }

    #region Volume

    private void SetSfxVolume(object[] args)
    {
        audioSFX.volume = (float)args[0] / 10;
        if (!audioSFX.isPlaying)
        {
            audioSFX.PlayOneShot(SFXs[0].clip);
        }
    }

    private void SetMusicVolume(object[] args)
    {
        audioMusic.volume = (float)args[0] / 10;
    }

    #endregion


    [Serializable]
    public class Music
    {
        public string name;
        public AudioClip clip;
    }

    [Serializable]
    public class SFX
    {
        public string Name;
        public AudioClip clip;
    }
}
