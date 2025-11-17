using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInt : MonoBehaviour
{
    public Transform npc;
    public GameObject triggerObject;             // Dispara primeiro diálogo (default)
    public GameObject cycleWatchedObject;        // Dispara segundo diálogo após ativar/desativar

    DialogueSystem dialogueSystem;
    private bool hasInteracted = false;
    private bool hasSecondDialoguePlayed = false;

    void Awake()
    {
        dialogueSystem = FindObjectOfType<DialogueSystem>();
    }

    void Update()
    {
        // ---------- Primeiro diálogo (objeto ativado) ----------
        if (triggerObject.activeInHierarchy && !hasInteracted)
        {
            if (Vector2.Distance(transform.position, npc.position) < 2.0f)
            {
                dialogueSystem.SetDialogueData(dialogueSystem.defaultDialogueData); 
                dialogueSystem.Next();
                hasInteracted = true;
            }
        }

        if (!triggerObject.activeInHierarchy && hasInteracted)
        {
            hasInteracted = false;
        }

        // Dispara o segundo diálogo assim que o objeto for ativado (uma única vez)
        if (cycleWatchedObject.activeInHierarchy && !hasSecondDialoguePlayed)
        {
            if (Vector2.Distance(transform.position, npc.position) < 2.0f)
            {
                dialogueSystem.SetDialogueData(dialogueSystem.secondDialogueData);
                dialogueSystem.Next();
                hasSecondDialoguePlayed = true;
            }
        }

    }
}
