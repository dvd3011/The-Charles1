using UnityEngine;

public class DigSite : MonoBehaviour
{
    [Header("Tipos de Site")]
    public bool isFossil = true; // True: Fóssil real; False: Falso/objeto/clue
    public bool isDiscovered = false; // Já escavado? (setado pelo InteractionFase3)

    [Header("Escavação")]
    public int digsRequired = 25; // Cliques em 'F' necessários (varie por site)
    public GameObject fossilPrefab; // Prefab/UI do fóssil a instanciar se real

    public bool CanDig()
    {
        return !isDiscovered;
    }
}
