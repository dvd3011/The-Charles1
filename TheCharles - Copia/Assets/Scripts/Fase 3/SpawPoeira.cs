using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawPoeira : MonoBehaviour
{
    public GameObject PrefabPoeira;
    public Transform poeiraPai;
    public int numParticulas;

    private InteractionFase3 playerInteraction; // Referência ao player
    private bool jogoIniciado = false;

    void OnEnable() // Usamos OnEnable para garantir que rode toda vez que o objeto for ativado
    {
        playerInteraction = FindObjectOfType<InteractionFase3>();
        SpawnarPoeira();
        jogoIniciado = true;
    }

    void SpawnarPoeira()
    {
        // Limpa poeiras antigas se houver (segurança)
        foreach (Transform child in poeiraPai)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < numParticulas; i++)
        {
            Instantiate(PrefabPoeira, new Vector3(Random.Range(-5f, 5f), Random.Range(-3.5f, 3.5f), 0), Quaternion.identity, poeiraPai);
        }
    }

    void Update()
    {
        if (jogoIniciado)
        {
            // Verifica se não há mais filhos (poeiras) dentro do objeto pai
            if (poeiraPai.childCount == 0)
            {
                FinalizarMinigame();
            }
        }
    }

    void FinalizarMinigame()
    {
        jogoIniciado = false;
        Debug.Log("Todas as poeiras foram limpas!");

        if (playerInteraction != null)
        {
            playerInteraction.FinishCleaning(); // Chama o método no script principal para fechar o jogo
        }
    }
}