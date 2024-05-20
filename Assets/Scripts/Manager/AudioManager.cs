using System.Collections;
using System.Collections.Generic;
using com.tksr.schema;
using Newtonsoft.Json;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource Background;
    public AudioSource Effect;

    public AudioClip AudioDefault;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Background == null)
        {
            GameObject background = new GameObject();
            background.name = "Background";
            AudioSource audio = background.AddComponent<AudioSource>();
            background.transform.parent = this.transform;
            Background = audio;
        }
        if (Effect == null)
        {
            GameObject effect = new GameObject();
            effect.name = "Effect";
            AudioSource audio = effect.AddComponent<AudioSource>();
            effect.transform.parent = this.transform;
            Background = audio;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private SchemaSound schemaSound;

    public void LoadSoundSchema(string jsonSound)
    {
        schemaSound = JsonConvert.DeserializeObject<SchemaSound>(jsonSound);
    }

    public TKRMusic GetBackgroundMusic(int Id)
    {
        if (schemaSound.background.ContainsKey(Id.ToString()))
        {
            var background = schemaSound.background[Id.ToString()];
            return background;
        }

        return null;
    }


    public void PlayMusic()
    {
        if (Background != null)
        {
            Background.loop = true;
            Background.Play();
        }
    }

    public void PlayMainUIBackgroundMusic()
    {
        if (AudioDefault != null)
        {
            if (Background != null)
            {
                Background.clip = AudioDefault;
            }

            PlayMusic();
        }
    }

    public void PlayScenarioBackgroundMusic(int soundId)
    {
        var background = GetBackgroundMusic(soundId);
        if (background != null)
        {
            var audioClip = GameAssetBundlesManager.Instance.LoadAudioSync(background.ABPath, background.AssetName);
            if (Background != null)
            {
                Background.clip = audioClip;
            }
            PlayMusic();
        }
    }

    public void StopMusic()
    {
        if (Background != null)
        {
            Background.Stop();
        }
    }

    public void PlayEffectFromBackground(int soundId)
    {
        var effect = GetBackgroundMusic(soundId);
        if (effect != null)
        {
            var audioClip = GameAssetBundlesManager.Instance.LoadAudioSync(effect.ABPath, effect.AssetName);
            if (Effect != null)
            {
                Effect.clip = audioClip;
                Effect.loop = false;
                Effect.Play();
            }
        }
    }
}
