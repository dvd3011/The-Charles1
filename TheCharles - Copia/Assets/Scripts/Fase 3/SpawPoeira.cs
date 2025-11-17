using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawPoeira : MonoBehaviour
{
    public GameObject PrefabPoeira;
    public Transform poeiraPai;
    public int numParticulas = 10;

    [Header("Tamanho da Área")]
    public float larguraSpawn = 5f; // X
    public float alturaSpawn = 3.5f; // Y

    private InteractionFase3 playerInteraction;
    public bool jogoIniciado = false;

    void OnEnable()
    {
        playerInteraction = FindObjectOfType<InteractionFase3>();
        SpawnarPoeira();
        jogoIniciado = true;
    }

   public void SpawnarPoeira()
    {
       
            // Limpa poeiras antigas
            foreach (Transform child in poeiraPai)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < numParticulas; i++)
            {
                // 1. Calcula uma posição aleatória local (deslocamento)
                float randomX = Random.Range(-larguraSpawn, larguraSpawn);
                float randomY = Random.Range(-alturaSpawn, alturaSpawn);
                Vector3 offset = new Vector3(randomX, randomY, 0);

                // 2. Soma com a posição atual deste objeto (SpawPoeira)
                // Isso garante que a poeira nasça ONDE o objeto está na cena
                Vector3 spawnPosition = transform.position + offset;

                // 3. Instancia a poeira
                GameObject newPoeira = Instantiate(PrefabPoeira, spawnPosition, Quaternion.identity, poeiraPai);

                // 4. CHAMA O NOVO MÉTODO PARA PASSAR OS LIMITES
                Poeira poeiraScript = newPoeira.GetComponent<Poeira>();
                if (poeiraScript != null)
                {
                    // Passa a posição central (deste GameObject) e os limites de X e Y
                    poeiraScript.SetDestroyLimits(transform.position, larguraSpawn, alturaSpawn);
                }
            }
        
    }

    void Update()
    {
        if (jogoIniciado)
        {
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
            playerInteraction.FinishCleaning();
        }
    }

    // --- AJUDA VISUAL NA CENA ---
    // Isso desenha um quadrado amarelo na Scene para você ver onde a poeira vai nascer
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        // Desenha o quadrado baseado na posição atual e nos tamanhos definidos
        Gizmos.DrawWireCube(transform.position, new Vector3(larguraSpawn * 2, alturaSpawn * 2, 1));
    }
}