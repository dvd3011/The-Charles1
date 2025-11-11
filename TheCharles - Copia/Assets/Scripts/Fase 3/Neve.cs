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

    void Start()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        telaMax = canvasRect.rect.size;

        velocidades = new float[imagens.Length];
        for (int i = 0; i < imagens.Length; i++)
        {
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
        float x = Random.Range(0, telaMax.x);
        float y = telaMax.y + Random.Range(50f, 200f);
        imagens[i].anchoredPosition = new Vector2(x, y);
    }
}
