using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{

    public static AudioClip crackSound, thudSound;
    static AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        crackSound = Resources.Load<AudioClip> ("Crack");
        thudSound = Resources.Load<AudioClip> ("Thud");
        audioSrc = GetComponent<AudioSource> ();
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
