using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projetil : MonoBehaviour
{
    public FadeController fadeController;
    private HudFase4 placar;

    private bool jaColidiu = false; // Flag para evitar múltiplas colisões

    void Start()
    {
        placar = GameObject.Find("Hud").GetComponent<HudFase4>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (jaColidiu) return; // Sai se já colidiu
        if(!jaColidiu)
        {
            fadeController.StartFade(gameObject,2f,true);
            Destroy(this.gameObject, 1f);
        }
        if (other.CompareTag("TentE"))
        {
            jaColidiu = true; // Marca como já colidido
            fadeController.StartFade(gameObject,2f,true);
            Destroy(this.gameObject,1f);
            fadeController.StartFade(other.gameObject, 2f, true);

            other.GetComponent<DestroyerFase4>().Destruir();
            placar.PlacarTent(1);
        }
    }
}
