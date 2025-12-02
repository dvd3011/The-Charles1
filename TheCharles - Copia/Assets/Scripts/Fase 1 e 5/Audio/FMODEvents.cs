using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("MX")]
    [field: SerializeField] public EventReference music { get; private set; }

    [field: Header("BGFX")]
    [field: SerializeField] public EventReference backg { get; private set; }
    [field: SerializeField] public EventReference wallafx { get; private set; }
    [field: SerializeField] public EventReference botafogo { get; private set; }

    [field: Header("FS")]
    [field: SerializeField] public EventReference footstepEvent { get; private set; }

    [field: Header("HFX")]
    [field: SerializeField] public EventReference macacoSound { get; private set; }
    [field: SerializeField] public EventReference beijaSound { get; private set; }
    [field: SerializeField] public EventReference frogSound { get; private set; }
    [field: SerializeField] public EventReference tent1Sound { get; private set; }
    [field: SerializeField] public EventReference tent2Sound { get; private set; }
    [field: SerializeField] public EventReference tent3Sound { get; private set; }
    [field: SerializeField] public EventReference ararara { get; private set; }

    [field: Header("SFX")]
    [field: SerializeField] public EventReference doorSound { get; private set; }
    [field: SerializeField] public EventReference interactionSound { get; private set; }
    [field: SerializeField] public EventReference interactionDocs { get; private set; }
    [field: SerializeField] public EventReference collectSound { get; private set; }
    [field: SerializeField] public EventReference captuSound { get; private set; }
    [field: SerializeField] public EventReference armarSound { get; private set; }
    [field: SerializeField] public EventReference pageSound { get; private set; }

    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene");
        }
        instance = this;
    }
}
