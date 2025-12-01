using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
  

    GameObject obj;
    public CollectFeedBack fb;

    // Assuma que voc� tem refer�ncia ao InteractionsI (arraste no Inspector ou use FindObjectOfType)
    public InteractionsI interactions; // Arraste o player ou manager
    // No final de Destruir(), ap�s feedback:
    
void Start()
    {
     
    }

    public void Destruir()
    {
        if (gameObject.CompareTag("Tentilhao"))
        {
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            fb.ShowFeedback("+1 Tentilh�o", Color.yellow);
            DesativarDepois(gameObject, 0f);
        }
        if (gameObject.CompareTag("Planta"))
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            fb.GetComponent<CollectFeedBack>().ShowFeedback( "+1 Planta", Color.yellow);

            DesativarDepois(gameObject, 0f);
            interactions.CheckMissionProgress();
        }
        if (gameObject.CompareTag("Livro"))
        {

            fb.ShowFeedback( "+1 Livro", Color.yellow);
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            DesativarDepois(gameObject, 0f);
            interactions.CheckMissionProgress();
        }
        if (gameObject.CompareTag("BeijaFlor"))
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            fb.ShowFeedback("+1 Beija Flor", Color.yellow);
            interactions.CheckMissionProgress();
            DesativarDepois(gameObject, 0f);
        }
        if (gameObject.CompareTag("Sapo"))
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            fb.ShowFeedback("+1 Sapo", Color.yellow);
            interactions.CheckMissionProgress();
            DesativarDepois(gameObject, 0f);
        }
        if (gameObject.CompareTag("Semente"))
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            interactions.CheckMissionProgress();
            fb.ShowFeedback("+1 Semente", Color.yellow);

            DesativarDepois(gameObject, 0f);
        }
        if (gameObject.CompareTag("Folha"))
        {
            interactions.CheckMissionProgress();
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            fb.ShowFeedback("+1 Folha", Color.yellow);

            DesativarDepois(gameObject, 0f);
        }
        if (gameObject.CompareTag("Banana"))
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            interactions.CheckMissionProgress();
            fb.ShowFeedback( "+1 Banana", Color.yellow);

            DesativarDepois(gameObject, 0f);
        }
        if (gameObject.CompareTag("Madeira"))
        {
            interactions.CheckMissionProgress();
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            fb.ShowFeedback( "+1 Madeira", Color.yellow);

            DesativarDepois(gameObject, 0f);
        }
        if (gameObject.CompareTag("Pedra"))
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            interactions.CheckMissionProgress();
            fb.ShowFeedback("+1 Pedra", Color.yellow);

            DesativarDepois(gameObject, 0f);
        }
        if (gameObject.CompareTag("Borracha"))
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            fb.ShowFeedback( "+1 Borracha", Color.yellow);
            interactions.CheckMissionProgress();
            DesativarDepois(gameObject, 0f);
        }
        if (gameObject.CompareTag("Fruta"))
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            fb.ShowFeedback("+1 Fruta", Color.yellow);
            interactions.CheckMissionProgress();
            DesativarDepois(gameObject, 0f);
        }
        if (gameObject.CompareTag("Cipo"))
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            fb.ShowFeedback("+1 Cipo", Color.yellow);
            interactions.CheckMissionProgress();
            DesativarDepois(gameObject, 0f);
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

