using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    private EventInstance musicEventInstance; 
    private EventInstance backgEventInstance;
    private EventInstance wallafxEventInstance;

    [Header("Volumes iniciais")]
    [Range(0f, 1f)]
    public float initialMXVolume = 1f;
    [Range(0f, 1f)]
    public float initialBGVolume = 1f;
    [Range(0f, 1f)]
    public float initialWallafxVolume = 0f;

    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("Found more than one Audio Manager in the scene.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        InitializeMusic(FMODEvents.instance.music);
        InitializeBG(FMODEvents.instance.backg);
        InitializeWallafx(FMODEvents.instance.wallafx);

        // Aplica volumes iniciais do Inspector
        SetMXVolume(initialMXVolume);
        SetBGVolume(initialBGVolume);
        SetWallafxVolume(initialWallafxVolume);
    }

    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = RuntimeManager.CreateInstance(musicEventReference);
        musicEventInstance.start();
    }

    private void InitializeBG(EventReference backgEventReference)
    {
        backgEventInstance = RuntimeManager.CreateInstance(backgEventReference);
        backgEventInstance.start();
    }

    private void InitializeWallafx(EventReference wallafxEventReference)
    {
        wallafxEventInstance = RuntimeManager.CreateInstance(wallafxEventReference);
        wallafxEventInstance.start();
    }

    // --- Controle de volumes ---
    public void SetMXVolume(float volume)
    {
        if (musicEventInstance.isValid()) musicEventInstance.setVolume(volume);
    }

    public void SetBGVolume(float volume)
    {
        if (backgEventInstance.isValid()) backgEventInstance.setVolume(volume);
    }

    public void SetWallafxVolume(float volume)
    {
        if (wallafxEventInstance.isValid()) wallafxEventInstance.setVolume(volume);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
}
