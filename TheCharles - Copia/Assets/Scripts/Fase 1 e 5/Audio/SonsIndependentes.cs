using UnityEngine;
using FMODUnity;
using FMOD.Studio;

[RequireComponent(typeof(Transform))]
public class SonsIndependentes : MonoBehaviour
{
    [Header("Escolha o som (FMODEvents)")]
    [Tooltip("Selecione qual som do FMODEvents será reproduzido")]
    public SoundType soundType;

    [Header("Referência do Player")]
    [Tooltip("Objeto que representa o jogador (ou câmera)")]
    [SerializeField] private Transform player;

    [Header("Configurações de Distância")]
    [Tooltip("Distância em que o som é ouvido no volume máximo")]
    [SerializeField] private float minDistance = 2f;
    
    [Tooltip("Distância máxima em que o som fica inaudível")]
    [SerializeField] private float maxDistance = 10f;

    [Header("Volume Base")]
    [Range(0f, 1f)]
    [SerializeField] private float baseVolume = 1f;

    private EventInstance soundInstance;
    private bool isPlaying = false;
    private EventReference selectedEvent;

    private void Start()
    {
        if (FMODEvents.instance == null)
        {
            Debug.LogError("FMODEvents.instance não encontrado na cena!");
            return;
        }

        // Seleciona o evento com base no enum
        selectedEvent = GetEventReferenceFromType(soundType);

        if (selectedEvent.IsNull)
        {
            Debug.LogWarning($"Nenhum evento FMOD encontrado para {soundType}");
            return;
        }

        soundInstance = RuntimeManager.CreateInstance(selectedEvent);

        // ⚠ Importante: definir atributos 3D ANTES de start()
        soundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));

        soundInstance.start();
        isPlaying = true;
    }

    private void Update()
    {
        if (!isPlaying || player == null)
            return;

        // Atualiza atributos 3D corretamente
        soundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

        // Calcula volume
        float distance = Vector3.Distance(player.position, transform.position);
        float volume = CalculateVolume(distance);
        soundInstance.setVolume(volume);
    }

    private float CalculateVolume(float distance)
    {
        if (distance <= minDistance)
            return baseVolume;

        if (distance >= maxDistance)
            return 0f;

        float t = (distance - minDistance) / (maxDistance - minDistance);
        return Mathf.Lerp(baseVolume, 0f, t);
    }

    private EventReference GetEventReferenceFromType(SoundType type)
    {
        switch (type)
        {
            case SoundType.Macaco: return FMODEvents.instance.macacoSound;
            case SoundType.BeijaFlor: return FMODEvents.instance.beijaSound;
            case SoundType.Sapo: return FMODEvents.instance.frogSound;
            case SoundType.Tent1: return FMODEvents.instance.tent1Sound;
            case SoundType.Tent2: return FMODEvents.instance.tent2Sound;
            case SoundType.Tent3: return FMODEvents.instance.tent3Sound;
            case SoundType.Botafogo: return FMODEvents.instance.botafogo;
            case SoundType.Ararara: return FMODEvents.instance.ararara;
            default: return new EventReference();
        }
    }

    private void OnDisable()
    {
        // ⚠ obrigatório: para sons que continuam após o objeto sumir
        if (soundInstance.isValid())
        {
            soundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            soundInstance.release();
        }
    }

    private void OnDestroy()
    {
        if (soundInstance.isValid())
        {
            soundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            soundInstance.release();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
#endif
}

public enum SoundType
{
    Macaco,
    BeijaFlor,
    Sapo,
    Tent1,
    Tent2,
    Tent3,
    Botafogo,
    Ararara
}
