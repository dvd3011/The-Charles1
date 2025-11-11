using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Importa o namespace do TextMeshPro
using UnityEngine.SceneManagement;


public class Placar : MonoBehaviour
{
    public int plantaAtual;
    public int livroAtual;
    public int sapoAtual;
    public int beijaAtual;

    public TMP_Text plantaTxtMissao1;
    public TMP_Text sapoTxtMissao1;
    public TMP_Text livroTxtMissao2;
    public TMP_Text beijaTxtMissao3;
    public TMP_Text sapoTxtMissao3;
    public TMP_Text plantaMissao3;
    public TMP_Text livroMissao3;

    public InteractionsI missao;
    public TMP_Text timerMissao2;
    public TMP_Text timerMissao3;
    public GameObject timer2, timer3;

    void Start()
    {
        // Inicializa os textos
        UpdateUI();
    }

    void Update()
    {
        if (IsInScene("Fase5") && beijaAtual >= 2 && sapoAtual >= 2)
        {
            Time.timeScale = 0;
        }
        if (missao.missao2Ativa)
        {
            timer2.SetActive(true);
            timerMissao2.text = missao.timerMissao2.ToString("0");
        }
        else
        {
            timer2.SetActive(false);

        }
        if(missao.missao3Ativa)
        {
            timer3.SetActive(true);
            timerMissao3.text = missao.timerMissao3.ToString("0");
        }
        else
        {
            timer3.SetActive(false);
        }
        // Atualizações contínuas, se necessário
    }

    public void PlacarLivro(int quantidade)
    {
        livroAtual += quantidade;
        UpdateUI();
    }

    public void PlacarPlanta(int quantidade)
    {
        plantaAtual += quantidade;
        UpdateUI();
    }

    public void PlacarSapo(int quantidade)
    {
        sapoAtual += quantidade;
        UpdateUI();
    }

    public void PlacarBeija(int quantidade)
    {
        beijaAtual += quantidade;
        UpdateUI();
    }

    // Método para reiniciar as contagens
    public void ResetPlacar()
    {
        plantaAtual = 0;
        livroAtual = 0;
        sapoAtual = 0;
        beijaAtual = 0;
        UpdateUI(); // Atualiza os textos para refletir a reinicialização
    }

    // Método para atualizar a interface do usuário
    public void UpdateUI()
    {
        plantaTxtMissao1.text = plantaAtual.ToString();
        sapoTxtMissao1.text = sapoAtual.ToString();
        livroTxtMissao2.text = livroAtual.ToString();
        livroMissao3.text = livroAtual.ToString();
        sapoTxtMissao3.text = sapoAtual.ToString();
        beijaTxtMissao3.text = beijaAtual.ToString();
        plantaMissao3.text = plantaAtual.ToString();
    }
    private bool IsInScene(string sceneName)
    {
        // Obtém o nome da cena atual
        string currentSceneName = SceneManager.GetActiveScene().name;
        // Compara o nome da cena atual com o nome da cena desejada
        return currentSceneName == sceneName;
    }
}
