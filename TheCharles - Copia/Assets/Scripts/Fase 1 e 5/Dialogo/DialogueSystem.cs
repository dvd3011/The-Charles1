using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum STATE
{
    DISABLED,
    WAITING,
    TYPING
}

public class DialogueSystem : MonoBehaviour
{
    [Header("Diálogos disponíveis")]
    public DialogueData defaultDialogueData;
    public DialogueData secondDialogueData;

    [Header("Configuração")]
    public MonoBehaviour playerMovementScript;

    private DialogueData currentDialogueData;

    int currentText = 0;
    bool finished = false;
    TypeTextAnimation typeText;
    DialogueUI dialogueUI;
    STATE state;

    private InputAction advanceAction;

    void Awake()
    {
        typeText = FindObjectOfType<TypeTextAnimation>();
        dialogueUI = FindObjectOfType<DialogueUI>();
        typeText.TypeFinished = OnTypeFinished;

        advanceAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/enter");
        advanceAction.performed += ctx => HandleAdvance();
        advanceAction.Enable();

        currentDialogueData = defaultDialogueData; // Inicializa com o padrão
    }

    void Start()
    {
        state = STATE.DISABLED;
    }

    void Update()
    {
        if (state == STATE.DISABLED) return;

        // Detecta cliques do mouse para avançar o diálogo
        if (Input.GetMouseButtonDown(0))
        {
            HandleAdvance();
        }
    }

    void HandleAdvance()
    {
        if (state == STATE.TYPING)
        {
            typeText.Skip();
            state = STATE.WAITING;
        }
        else if (state == STATE.WAITING)
        {
            if (!finished)
            {
                Next();
            }
            else
            {
                dialogueUI.Disable();
                state = STATE.DISABLED;
                currentText = 0;
                finished = false;

                if (playerMovementScript != null)
                    playerMovementScript.enabled = true;
            }
        }
    }

    public void SetDialogueData(DialogueData data)
    {
        currentDialogueData = data;
        currentText = 0;
        finished = false;
    }

    public void Next()
    {
        if (currentDialogueData == null || currentDialogueData.talkScript == null || currentDialogueData.talkScript.Count == 0)
        {
            Debug.LogWarning("DialogueData ou talkScript não configurados.");
            return;
        }

        if (currentText == 0)
        {
            dialogueUI.Enable();

            if (playerMovementScript != null)
                playerMovementScript.enabled = false;
        }

        dialogueUI.SetName(currentDialogueData.talkScript[currentText].name);
        typeText.fullText = currentDialogueData.talkScript[currentText++].text;

        if (currentText == currentDialogueData.talkScript.Count) finished = true;

        typeText.StartTyping();
        state = STATE.TYPING;
    }

    void OnTypeFinished()
    {
        state = STATE.WAITING;
    }
}
