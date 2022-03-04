using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManagerScript : MonoBehaviour
{
    
    public static AudioClip crackSound, thudSound;
    static AudioSource audioSrc;

    public AudioMixer mixer;

    // Start is called before the first frame update
    void Start()
    {
        crackSound = Resources.Load<AudioClip> ("Crack");
        thudSound = Resources.Load<AudioClip> ("Thud");
        audioSrc = GetComponent<AudioSource> ();

        if (PlayerPrefs.HasKey("MasterVol"))
        {
            mixer.SetFloat("MasterVol", PlayerPrefs.GetFloat("MasterVol"));
        }

        if (PlayerPrefs.HasKey("MusicVol"))
        {
            mixer.SetFloat("MusicVol", PlayerPrefs.GetFloat("MusicVol"));
        }
        if (PlayerPrefs.HasKey("MasterVol"))

        {
            mixer.SetFloat("SFXVol", PlayerPrefs.GetFloat("SFXVol"));
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static void PlaySound (string clip)
    {
        switch (clip) {
            case "Crack":
                audioSrc.PlayOneShot (crackSound);
                break;
            case "Thud":
                audioSrc.PlayOneShot (thudSound);
                break;
        }
    }
}
