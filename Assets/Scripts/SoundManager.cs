using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
    public AudioSource efxSource;
    public AudioSource musicSource; // background sound
    public static SoundManager instance = null;

    public float lowPitchRange = .95f;  //The lowest a sound effect will be randomly pitched.
    public float highPitchRange = 1.05f; //The highest a sound effect will be randomly pitched.

    // using a singletone pattern
    void Awake () {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            // //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);
        }

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
	}
	
    // we can call it from our other scripts that are executing the actual game logic.
    public void PlaySingle (AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }

    // params keyword : allow us to parse in a comma separated list of arguments of the same type, as specified by the parameter.
    public void RandomizeSfx (params AudioClip [] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch; //Set the pitch of the audio source to the randomly chosen pitch.
        efxSource.clip = clips[randomIndex];    //Set the clip to the clip at our randomly chosen index.
        efxSource.Play(); 
    }
}
