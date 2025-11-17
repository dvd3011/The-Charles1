using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Importa o namespace do TextMeshPro
using UnityEngine.SceneManagement;


public class HudFase3 : MonoBehaviour
{
    public int frutaAtual;
    public int fossilAtual;
    public int tentilhaoAtual;
    public int madeiraAtual;
    public int borrachaAtual;


    public TMP_Text tentMissao1;


    public TMP_Text madeiraTxtMissao2;
    public TMP_Text fossilTxt;
    public TMP_Text frutaTxtMissao2;
    public TMP_Text tentMissao21;



    public TMP_Text madeiraTxtMissao3;
    public TMP_Text borrachaTxtMissao3;
    public TMP_Text tentTxtMissao31;


    public InteractionFase3 missao;

    void Start()
    {
        // Inicializa os textos
        UpdateUI();
    }

    void Update()
    {


    }

    public void PlacarFossil(int quantidade)
    {
        fossilAtual += quantidade;
        UpdateUI();
    }

    public void PlacarTent(int quantidade)
    {
        tentilhaoAtual += quantidade;
        UpdateUI();
    }

    public void PlacarFruta(int quantidade)
    {
        frutaAtual += quantidade;
        UpdateUI();
    }

    public void PlacarMadeira(int quantidade)
    {
        madeiraAtual += quantidade;
        UpdateUI();
    }
    public void PlacarBorracha(int quantidade)
    {
        borrachaAtual += quantidade;
        UpdateUI();
    }


    // M�todo para reiniciar as contagens
    public void ResetPlacar()
    {
        tentilhaoAtual = 0;
        borrachaAtual = 0;
        frutaAtual = 0;
        madeiraAtual = 0;
        fossilAtual = 0;
        UpdateUI(); // Atualiza os textos para refletir a reinicializa��o
    }

    // M�todo para atualizar a interface do usu�rio
    public void UpdateUI()
    {
        fossilTxt.text = fossilAtual.ToString();

        /* tentMissao1.text = tentilhaoAtual.ToString();

         madeiraTxtMissao2.text = madeiraAtual.ToString();
         frutaTxtMissao2.text = frutaAtual.ToString();

         madeiraTxtMissao3.text = madeiraAtual.ToString();
         borrachaTxtMissao3.text = borrachaAtual.ToString();
         tentTxtMissao31.text = tentilhaoAtual.ToString();*/
    }
    private bool IsInScene(string sceneName)
    {
        // Obt�m o nome da cena atual
        string currentSceneName = SceneManager.GetActiveScene().name;
        // Compara o nome da cena atual com o nome da cena desejada
        return currentSceneName == sceneName;
    }
}
