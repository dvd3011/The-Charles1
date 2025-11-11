using UnityEngine;

public class ConditionalDialogueTrigger : MonoBehaviour
{
    public GameObject observedObject;      // O objeto que será ativado e depois desativado
    public GameObject targetToActivate;    // O objeto que deve ser ativado no final

    private bool wasActivated = false;
    private bool hasTriggered = false;

    void Update()
    {
        if (hasTriggered) return;

        // Detecta se o objeto foi ativado em algum momento
        if (observedObject.activeInHierarchy)
        {
            wasActivated = true;
        }

        // Se já foi ativado uma vez e agora está desativado → ativar o alvo
        if (wasActivated && !observedObject.activeInHierarchy)
        {
            targetToActivate.SetActive(true);
            hasTriggered = true; // Garante que isso só acontece uma vez
        }
    }
}
