using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class Settings : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    Resolution[] resolutions;
    List<int> optionsWidth = new List<int>();
    List<int> optionsHeight = new List<int>();

    public AudioMixer mixer;
    public TMP_Text masterLabel, musicLabel, sfxLabel;
    public Slider masterSlider, musicSlider, sfxSlider;


    // Start is called before the first frame update
    void Start()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            bool match = false;
            foreach (string j in options)
            {
                if (j.Equals(option))
                {
                    match = true;
                }
            }
            if (match)
            {
                continue;
            }
            options.Add(option);
            optionsWidth.Add(resolutions[i].width);
            optionsHeight.Add(resolutions[i].height);

         
        }
        for (int i = 0; i < options.Count; i++)
        {
            if (optionsWidth[i] == Screen.width &&
                optionsHeight[i] == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        float vol = 0;
        mixer.GetFloat("MasterVol", out vol);
        masterSlider.value = vol;
        mixer.GetFloat("MusicVol", out vol);
        musicSlider.value = vol;
        mixer.GetFloat("SFXVol", out vol);
        sfxSlider.value = vol;

        masterLabel.text = Mathf.RoundToInt(masterSlider.value + 80).ToString();
        musicLabel.text = Mathf.RoundToInt(musicSlider.value + 80).ToString();
        sfxLabel.text = Mathf.RoundToInt(sfxSlider.value + 80).ToString();

    }


    public void SetFullscreen()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void SetResolution()
    {
        int resolutionIndex = GameObject.Find("ResolutionDropdown").GetComponent<Dropdown>().value;
        Screen.SetResolution(optionsWidth[resolutionIndex], optionsHeight[resolutionIndex], Screen.fullScreen);
    }

    public void SetMasterVol()
    {
        masterLabel.text = Mathf.RoundToInt(masterSlider.value + 80).ToString();
        mixer.SetFloat("MasterVol", masterSlider.value);

        PlayerPrefs.SetFloat("MasterVol", masterSlider.value);
    }
    public void SetMusicVol()
    {
        musicLabel.text = Mathf.RoundToInt(musicSlider.value + 80).ToString();
        mixer.SetFloat("MusicVol", musicSlider.value);

        PlayerPrefs.SetFloat("MusicVol", musicSlider.value);
    }
    public void SetSfxVol()
    {
        sfxLabel.text = Mathf.RoundToInt(sfxSlider.value + 80).ToString();
        mixer.SetFloat("SFXVol", sfxSlider.value);

        PlayerPrefs.SetFloat("SFXVol", sfxSlider.value);
    }
}
