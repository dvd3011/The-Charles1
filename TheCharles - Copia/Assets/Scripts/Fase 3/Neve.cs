using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neve : MonoBehaviour
{
    public RectTransform[] imagens; 
    public float velocidadeMin = 100f;
    public float velocidadeMax = 200f;

    private Vector2 telaMax;
    private float[] velocidades;
    public RectTransform PrefabFlocoDeNeve; // 
    public int quantidadeFlocos = 25; // 
    private List<RectTransform> flocos; //
    void Start()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        telaMax = canvasRect.rect.size;

        flocos = new List<RectTransform>(); // Inicializa a lista
        velocidades = new float[quantidadeFlocos]; // Define o array de velocidades

        for (int i = 0; i < quantidadeFlocos; i++)
        {
            // Instancia o novo floco
            RectTransform newFloco = Instantiate(PrefabFlocoDeNeve, transform);
            flocos.Add(newFloco);

            // Note que agora o índice 'i' corresponde ao índice na lista 'flocos' e 'velocidades'
            ResetarImagem(i);
        }
    }

    void Update()
    {
        for (int i = 0; i < imagens.Length; i++)
        {
            imagens[i].anchoredPosition -= new Vector2(0, velocidades[i] * Time.deltaTime);

            if (imagens[i].anchoredPosition.y < -imagens[i].rect.height)
            {
                ResetarImagem(i);
            }
        }
    }

    void ResetarImagem(int i)
    {
        velocidades[i] = Random.Range(velocidadeMin, velocidadeMax);

        // --- NOVO CÁLCULO X: PARA CENTRALIZAR O SPAWN ---
        // Random.Range(-telaMax.x / 2) garante que o floco nasce na metade esquerda
        // e (telaMax.x / 2) garante que o floco nasce na metade direita.
        float x = Random.Range(-telaMax.x / 2, telaMax.x / 2);

        // --- CÁLCULO Y: SPAWN ACIMA DA TELA ---
        // telaMax.y / 2 é o topo da tela. 
        // Adicionar um valor garante que o floco comece fora da tela (acima).
        float y = (telaMax.y / 2);

        imagens[i].anchoredPosition = new Vector2(x, y);
    }
}
