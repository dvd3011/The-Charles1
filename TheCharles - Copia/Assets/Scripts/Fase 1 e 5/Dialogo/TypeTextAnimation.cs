using System.Collections;
using UnityEngine;
using TMPro;
using System;
using FMODUnity;
using FMOD.Studio;

public class TypeTextAnimation : MonoBehaviour
{
    public Action TypeFinished;
    public float typeDelay = 0.05f;
    public TextMeshProUGUI textObject;
    public string fullText;

    public string typingSoundEvent = "event:/SFX/Typing"; // Caminho do seu evento FMOD
    EventInstance typingSoundInstance;

    Coroutine coroutine;

    public void StartTyping()
    {
        typingSoundInstance = RuntimeManager.CreateInstance(typingSoundEvent);
        typingSoundInstance.start();

        coroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        textObject.text = fullText;
        textObject.maxVisibleCharacters = 0;

        for (int i = 0; i <= fullText.Length; i++)
        {
            textObject.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typeDelay);
        }

        typingSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        typingSoundInstance.release();

        TypeFinished?.Invoke();
    }

    public void Skip()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        textObject.maxVisibleCharacters = textObject.text.Length;

        typingSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        typingSoundInstance.release();
    }
}
