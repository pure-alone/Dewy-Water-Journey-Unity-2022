using System.Collections.Generic;
using UnityEngine;

public sealed class DewyAudio : MonoBehaviour
{
    private const string SoundKey = "dewySound";
    private AudioSource bgm;
    private AudioSource sfx;
    private bool userGestureReceived;
    public bool SoundEnabled { get; private set; }

    private static readonly Dictionary<DewyApp.Page, string> Bgm = new Dictionary<DewyApp.Page, string>
    {
        { DewyApp.Page.Home, "home_theme" }, { DewyApp.Page.Scene1, "sunny_ocean" },
        { DewyApp.Page.Scene2, "cloud_workshop" }, { DewyApp.Page.Scene3, "rainy_forest" },
        { DewyApp.Page.Scene4, "underground" }, { DewyApp.Page.Scene5, "mountain_stream" },
        { DewyApp.Page.Scene6, "river_ocean" }, { DewyApp.Page.Credits, "credits_theme" }
    };

    private static readonly Dictionary<string, string> Effects = new Dictionary<string, string>
    {
        { "ui", "ui_click" }, { "drop", "water_drop" }, { "sparkle", "sparkle" },
        { "evaporation", "evaporation" }, { "bubble", "bubble" }, { "rain", "rain" },
        { "forest", "forest_complete" }, { "drip", "underground_drip" },
        { "splash", "stream_splash" }, { "complete", "journey_complete" }
    };

    private void Awake()
    {
        SoundEnabled = PlayerPrefs.GetString(SoundKey, "on") != "off";
        if (Object.FindObjectOfType<AudioListener>() == null)
            gameObject.AddComponent<AudioListener>();

#if UNITY_WEBGL && !UNITY_EDITOR
        userGestureReceived = false;
#else
        userGestureReceived = true;
#endif

        bgm = gameObject.AddComponent<AudioSource>();
        bgm.loop = true;
        bgm.volume = 0.22f;
        bgm.playOnAwake = false;
        bgm.spatialBlend = 0f;
        sfx = gameObject.AddComponent<AudioSource>();
        sfx.loop = false;
        sfx.playOnAwake = false;
        sfx.spatialBlend = 0f;
    }

    public void PlayPageBgm(DewyApp.Page page)
    {
        bgm.Stop();
        if (!Bgm.TryGetValue(page, out string name)) return;
        bgm.clip = Resources.Load<AudioClip>("Audio/" + name);
        if (SoundEnabled && userGestureReceived && bgm.clip != null) bgm.Play();
    }

    public void NotifyUserGesture()
    {
        if (userGestureReceived) return;
        userGestureReceived = true;
        if (SoundEnabled && bgm.clip != null && !bgm.isPlaying) bgm.Play();
    }

    public void PlaySfx(string effect, float volume = 0.48f)
    {
        NotifyUserGesture();
        if (!SoundEnabled || !Effects.TryGetValue(effect, out string name)) return;
        AudioClip clip = Resources.Load<AudioClip>("Audio/" + name);
        if (clip != null) sfx.PlayOneShot(clip, volume);
    }

    public bool Toggle()
    {
        userGestureReceived = true;
        SoundEnabled = !SoundEnabled;
        PlayerPrefs.SetString(SoundKey, SoundEnabled ? "on" : "off");
        PlayerPrefs.Save();
        if (SoundEnabled)
        {
            if (bgm.clip != null && !bgm.isPlaying) bgm.Play();
            PlaySfx("ui", 0.34f);
        }
        else bgm.Pause();
        return SoundEnabled;
    }
}
