using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour {

    public static SoundManager instance;

    [SerializeField] AudioSource p1, p2, p3, p4,
        hypersquareClipPlayer, embededLooper, PMPSource, backingTrack;

    private void Awake()
    {
        if(SoundManager.instance == null)
        {
            SoundManager.instance = this;
        }
        else if(SoundManager.instance != this)
        {
            Destroy(gameObject);
            Debug.Log("Copy of sound manager found. Destroying self.");
            return;
        }

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += ManageSceneTransition;
    }

    
    public void HypersquarePlayClip(AudioClip newClip)
    {
        hypersquareClipPlayer.volume = 1f;
        hypersquareClipPlayer.clip = newClip;
        hypersquareClipPlayer.Play();
    }

    public void HypersquarePlayClip(AudioClip newClip, float volume)
    {
        hypersquareClipPlayer.volume = volume;
        hypersquareClipPlayer.clip = newClip;
        hypersquareClipPlayer.Play();
    }

    public void HypersquareStop()
    {
        hypersquareClipPlayer.Stop();
    }

    void ManageSceneTransition(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Lobby")
        {
            backingTrack.Stop();
        }else if(scene.name == "GameScene Split Screen")
        {
            backingTrack.Play();
        }
    }

    //public void HypersquareEmbeding(bool isPlaying)
    //{
    //    if (isPlaying)
    //    {
    //        if (embedLooper.isPlaying)
    //            return;
    //        embedLooper.Play();
    //    }
    //    else
    //    {
    //        if (!embedLooper.isPlaying)
    //            return;
    //        embedLooper.Stop();
    //    }
    //}

    public void HypersquareEmbedded(bool isPlaying)
    {
        if (isPlaying)
        {
            if (embededLooper.isPlaying)
                return;
            embededLooper.Play();
        }
        else
        {
            if (!embededLooper.isPlaying)
                return;
            embededLooper.Stop();
        }
    }

    public void PMPPlayClip(AudioClip newClip)
    {
        PMPSource.clip = newClip;
        PMPSource.Play();
    }

    public void FlatlanderPlayClip(AudioClip newClip, int index)
    {
        AudioSource source = IndexToSource(index);

        source.clip = newClip;
        source.Play();
    }

    AudioSource IndexToSource(int index)
    {
        switch (index)
        {
            case 0:
                return p1;
            case 1:
                return p2;
            case 2:
                return p3;
            case 3:
                return p4;

            default:
                Debug.LogError("Invalid index given to Sound Manager");
                return null;
        }
    }
}
