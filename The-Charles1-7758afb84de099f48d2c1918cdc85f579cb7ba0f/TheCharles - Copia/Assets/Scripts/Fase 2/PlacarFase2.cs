using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Importa o namespace do TextMeshPro
using UnityEngine.SceneManagement;


public class PlacarFase2 : MonoBehaviour
{
    public int folhaAtual;
    public int madeiraAtual;
    public int bananaAtual;
    public int sementeAtual;
    public int pedraAtual;
    public int borboletaAtual = 0;
    public int macacoAtual;
    public int passarinhoAtual;


    public TMP_Text folhaTxtMissao1;
    public TMP_Text madeiraTxtMissao1;
    public TMP_Text pedraTxtMissao1;
    public TMP_Text bananaTxtMissao1;
    public TMP_Text macacoTxtMissao11;


    public TMP_Text madeiraTxtMissao2;
    public TMP_Text folhaTxtMissao2;
    public TMP_Text borboletaTxtMissao21;


    public TMP_Text madeiraTxtMissao3;
    public TMP_Text sementeTxtMissao3;
    public TMP_Text pedraTxtMissao3;
    public TMP_Text folhaTxtMissao3;
    public TMP_Text passarinhoTxtMissao31;


    public InteractionsFase2 missao;
  
    void Start()
    {
        // Inicializa os textos
        UpdateUI();
    }

    void Update()
    {
  
        
    }

    public void PlacarSemente(int quantidade)
    {
        sementeAtual += quantidade;
        UpdateUI();
    }

    public void PlacarFolha(int quantidade)
    {
        folhaAtual += quantidade;
        UpdateUI();
    }

    public void PlacarBanana(int quantidade)
    {
        bananaAtual += quantidade;
        UpdateUI();
    }

    public void PlacarMadeira(int quantidade)
    {
        madeiraAtual += quantidade;
        UpdateUI();
    }
    public void PlacarPedra(int quantidade)
    {
        pedraAtual += quantidade;
        UpdateUI();
    }
    public void PlacarMacaco(int quantidade)
    {
        macacoAtual += quantidade;
        UpdateUI();
    }
    public void PlacarPassarinho(int quantidade)
    {
        passarinhoAtual += quantidade;
        UpdateUI();
    }
    public void PlacarBorboleta(int quantidade)
    {
        borboletaAtual += quantidade;
        UpdateUI();
    }

    // M�todo para reiniciar as contagens
    public void ResetPlacar()
    {
        folhaAtual = 0;
        pedraAtual = 0;
        sementeAtual = 0;
        madeiraAtual = 0;
        bananaAtual = 0;
        borboletaAtual = 0;
        macacoAtual = 0;
        passarinhoAtual = 0;
        UpdateUI(); // Atualiza os textos para refletir a reinicializa��o
    }

    // M�todo para atualizar a interface do usu�rio
    public void UpdateUI()
    {
        folhaTxtMissao1.text = folhaAtual.ToString();
        bananaTxtMissao1.text = bananaAtual.ToString();
        madeiraTxtMissao1.text = madeiraAtual.ToString();
        pedraTxtMissao1.text = pedraAtual.ToString();
        macacoTxtMissao11.text = macacoAtual.ToString();

        folhaTxtMissao2.text = folhaAtual.ToString();
        madeiraTxtMissao2.text = madeiraAtual.ToString();
        borboletaTxtMissao21.text = borboletaAtual.ToString();

        madeiraTxtMissao3.text = madeiraAtual.ToString();
        sementeTxtMissao3.text = sementeAtual.ToString();
        pedraTxtMissao3.text = pedraAtual.ToString();
        folhaTxtMissao3.text = folhaAtual.ToString();
        passarinhoTxtMissao31.text = passarinhoAtual.ToString();
    }
    private bool IsInScene(string sceneName)
    {
        // Obt�m o nome da cena atual
        string currentSceneName = SceneManager.GetActiveScene().name;
        // Compara o nome da cena atual com o nome da cena desejada
        return currentSceneName == sceneName;
    }
}
