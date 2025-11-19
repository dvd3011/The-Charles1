using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 1;
    [Range(0, 1)]
    public float musicVolume = 1;
    [Range(0, 1)]
    public float ambienceVolume = 1;
    [Range(0, 1)]
    public float SFXVolume = 1;

    private Bus masterBus;
    private Bus musicBus;
    private Bus ambienceBus;
    private Bus sfxBus;

    private EventInstance musicEventInstance; 
    private EventInstance backgEventInstance;
    private EventInstance wallafxEventInstance;

    private List<EventInstance> dynamicEvents = new List<EventInstance>();

    [Header("Volumes iniciais")]
    [Range(0f, 1f)] public float initialMXVolume = 1f;
    [Range(0f, 1f)] public float initialBGVolume = 1f;
    [Range(0f, 1f)] public float initialWallafxVolume = 0f;

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
        DontDestroyOnLoad(gameObject);

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }


    private void Start()
    {
        InitializeMusic(FMODEvents.instance.music);
        InitializeBG(FMODEvents.instance.backg);
        InitializeWallafx(FMODEvents.instance.wallafx);

        SetMXVolume(initialMXVolume);
        SetBGVolume(initialBGVolume);
        SetWallafxVolume(initialWallafxVolume);
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        ambienceBus.setVolume(ambienceVolume);
        sfxBus.setVolume(SFXVolume);
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

    // Registrar e remover eventos dinâmicos
    public void RegisterDynamicEvent(EventInstance instance)
    {
        if (instance.isValid())
            dynamicEvents.Add(instance);
    }

    public void UnregisterDynamicEvent(EventInstance instance)
    {
        if (instance.isValid())
            dynamicEvents.Remove(instance);
    }

    // Controle de volumes
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

    // PAUSAR
    public void PauseAll()
    {
        if (backgEventInstance.isValid()) backgEventInstance.setPaused(true);
        if (wallafxEventInstance.isValid()) wallafxEventInstance.setPaused(true);

        foreach (var e in dynamicEvents)
            if (e.isValid()) e.setPaused(true);
    }

    // RETOMAR
    public void ResumeAll()
    {
        if (backgEventInstance.isValid()) backgEventInstance.setPaused(false);
        if (wallafxEventInstance.isValid()) wallafxEventInstance.setPaused(false);

        foreach (var e in dynamicEvents)
            if (e.isValid()) e.setPaused(false);
    }

    // PARAR TODOS (mudança de cena)
    public void StopAll()
    {
        if (musicEventInstance.isValid()) musicEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if (backgEventInstance.isValid()) backgEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if (wallafxEventInstance.isValid()) wallafxEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        foreach (var e in dynamicEvents)
            if (e.isValid()) e.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        dynamicEvents.Clear();
    }
}
