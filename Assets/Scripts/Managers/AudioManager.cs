using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ClipProperty
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}

public class AudioManager : SoftSingleton<AudioManager>
{
    public List<AudioSource> audioSources = new();
    private AudioSource GetEmptyAudioSource()
    {
        var emptyAudioSource = audioSources.FirstOrDefault(x => !x.isPlaying);
        if (emptyAudioSource == null)
        {
            emptyAudioSource = gameObject.AddComponent<AudioSource>();
            audioSources.Add(emptyAudioSource);
        }
        return emptyAudioSource;
    }
    public void PlaySound(ClipProperty clipProperty)
    {
        if (clipProperty == null)
        {
            Debug.Log("Clip property null");
            return;
        }

        var source = GetEmptyAudioSource();
        var rndClip = clipProperty.clip;
        if (rndClip == null)
        {
            Debug.Log("clip null");
            return;
        }

        source.clip = rndClip;
        source.volume = clipProperty.volume;

        source.Play();
    }
}
