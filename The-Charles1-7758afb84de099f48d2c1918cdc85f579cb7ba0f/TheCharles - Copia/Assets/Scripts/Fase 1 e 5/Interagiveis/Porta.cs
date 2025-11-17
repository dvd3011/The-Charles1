using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Porta : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player, pontoUniDentro, pontoUniFora, pontoBiDentro, pontoBiFora, pontoSalaFora, pontoSalaDentro;
    private InteractionsI jogador;
    public GameObject dialogo;
    public GameObject minimapaUNI, minimapaABERTO, minimapaBIBLIO;

    void Start()
    {
        minimapaUNI.SetActive(false);
        minimapaBIBLIO.SetActive(false);
        minimapaABERTO.SetActive(false);
        jogador = GameObject.Find("Player").GetComponent<InteractionsI>(); // Obt�m a refer�ncia ao script Placar
    }

    // Update is called once per frame
    void Update()
    {

        
    }
     private void OnTriggerStay2D(Collider2D other)
    {
        if (gameObject.CompareTag("PortaBiblio") )
        {
            minimapaBIBLIO.SetActive(true);
            minimapaABERTO.SetActive(false);

            player.transform.position = pontoBiDentro.transform.position;
        }
        if (gameObject.CompareTag("PortaUniversidade"))
        {
            minimapaUNI.SetActive(true);
            minimapaABERTO.SetActive(false);
            player.transform.position = pontoUniDentro.transform.position;
        }
        if (gameObject.CompareTag("BiblioDentro") && jogador.missao2Ativa == false && jogador.missao3Ativa == false)
        {

            minimapaBIBLIO.SetActive(false);
            minimapaABERTO.SetActive(true);
            player.transform.position = pontoBiFora.transform.position;
        }
        
        if (gameObject.CompareTag("UniversidadeDentro"))
        {

            minimapaUNI.SetActive(false);
            minimapaABERTO.SetActive(true);
            player.transform.position = pontoUniFora.transform.position;
        }
        if (gameObject.CompareTag("SalaDentro"))
        {
            minimapaUNI.SetActive(true);
            player.transform.position = pontoSalaFora.transform.position;
            dialogo.GetComponent<QuestionDialogueController>().enabled = false;
        }
        
    }
}
