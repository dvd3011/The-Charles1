using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerFase3 : MonoBehaviour
{


    GameObject obj;
    public CollectFeedBack fb;
    private HudFase3 placar;

    // Assuma que você tem referência ao InteractionsI (arraste no Inspector ou use FindObjectOfType)
    public InteractionFase3 interactions; // Arraste o player ou manager
                                       // No final de Destruir(), após feedback:

    void Start()
    {
        placar = GameObject.Find("Hud").GetComponent<HudFase3>();

    }

    public void Destruir()
    {
       
        if (gameObject.CompareTag("RegEscava"))
        {
            fb.ShowFeedback("+1 Fossil", Color.yellow);
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            placar.PlacarFossil(1);
        }
    }
    public void DesativarDepois(GameObject objeto, float tempo)
    {
        StartCoroutine(DesativarObjeto(objeto, tempo));
    }

    private IEnumerator DesativarObjeto(GameObject objeto, float tempo)
    {
        yield return new WaitForSeconds(tempo);
        if (objeto != null)
            objeto.SetActive(false);
    }

}

