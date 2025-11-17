using UnityEngine;
using UnityEngine.UI;

public class AudioUIController : MonoBehaviour
{
    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        // Valores iniciais
        masterSlider.value = 1f;
        musicSlider.value = AudioManager.instance.initialMXVolume;
        sfxSlider.value = 1f;
    }

    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetMasterVolume(value);
    }

    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetMXVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        AudioManager.instance.SetBGVolume(value);
        AudioManager.instance.SetWallafxVolume(value);
    }
}
