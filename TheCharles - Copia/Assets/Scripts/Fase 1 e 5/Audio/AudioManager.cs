using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    private EventInstance musicEventInstance;
    private EventInstance backgEventInstance;
    private EventInstance wallafxEventInstance;

    private List<EventInstance> dynamicEvents = new List<EventInstance>();

    [Header("Volumes iniciais")]
    [Range(0f, 1f)] public float initialMXVolume = 1f;
    [Range(0f, 1f)] public float initialBGVolume = 1f;
    [Range(0f, 1f)] public float initialWallafxVolume = 0f;

    public static AudioManager instance { get; private set; }

    // MASTER
    private float masterVolume = 1f;

    // Volumes individuais antes do master
    private float currentMXVolume;
    private float currentBGVolume;
    private float currentWallafxVolume;

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

    // MASTER VOLUME
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;

        // Atualizar todos os canais com o master novo
        SetMXVolume(currentMXVolume);
        SetBGVolume(currentBGVolume);
        SetWallafxVolume(currentWallafxVolume);

        // Atualiza eventos dinâmicos
        foreach (var e in dynamicEvents)
        {
            if (e.isValid())
            {
                float vol;
                e.getVolume(out vol);
                e.setVolume(vol * masterVolume);
            }
        }
    }

    public float GetMasterVolume()
    {
        return masterVolume;
    }

    // REGISTRAR
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

    // VOLUMES INDIVIDUAIS
    public void SetMXVolume(float volume)
    {
        currentMXVolume = volume;

        if (musicEventInstance.isValid())
            musicEventInstance.setVolume(volume * masterVolume);
    }

    public void SetBGVolume(float volume)
    {
        currentBGVolume = volume;

        if (backgEventInstance.isValid())
            backgEventInstance.setVolume(volume * masterVolume);
    }

    public void SetWallafxVolume(float volume)
    {
        currentWallafxVolume = volume;

        if (wallafxEventInstance.isValid())
            wallafxEventInstance.setVolume(volume * masterVolume);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    // PAUSAR
    public void PauseAll()
    {
        if (musicEventInstance.isValid()) musicEventInstance.setPaused(true);
        if (backgEventInstance.isValid()) backgEventInstance.setPaused(true);
        if (wallafxEventInstance.isValid()) wallafxEventInstance.setPaused(true);

        foreach (var e in dynamicEvents)
            if (e.isValid()) e.setPaused(true);
    }

    // RETOMAR
    public void ResumeAll()
    {
        if (musicEventInstance.isValid()) musicEventInstance.setPaused(false);
        if (backgEventInstance.isValid()) backgEventInstance.setPaused(false);
        if (wallafxEventInstance.isValid()) wallafxEventInstance.setPaused(false);

        foreach (var e in dynamicEvents)
            if (e.isValid()) e.setPaused(false);
    }

    // PARAR TUDO
    public void StopAll()
    {
        if (musicEventInstance.isValid()) 
            musicEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        if (backgEventInstance.isValid()) 
            backgEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        if (wallafxEventInstance.isValid()) 
            wallafxEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        foreach (var e in dynamicEvents)
            if (e.isValid()) 
                e.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        dynamicEvents.Clear();
    }
}
