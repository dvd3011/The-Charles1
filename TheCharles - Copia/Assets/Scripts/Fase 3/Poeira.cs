// Poeira.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poeira : MonoBehaviour
{
    private string tagVassoura = "Vassoura";

    // Novas variáveis para os limites de destruição
    private Vector3 spawnCenter;
    private float destroyLimitX;
    private float destroyLimitY;

    // Novo método para configurar os limites
    public void SetDestroyLimits(Vector3 center, float limitX, float limitY)
    {
        spawnCenter = center;
        destroyLimitX = limitX;
        destroyLimitY = limitY;
    }

    // Update is called once per frame
    void Update()
    {
        // Se os limites foram definidos, verifica se a poeira está fora
        if (destroyLimitX > 0 && IsOutsideBounds())
        {
            Destroy(gameObject);
        }
    }

    // Checa se a poeira está fora da área. A área é o dobro de 'larguraSpawn' e 'alturaSpawn'.
    private bool IsOutsideBounds()
    {
        // Calcula o deslocamento em relação ao centro de spawn
        float offsetX = Mathf.Abs(transform.position.x - spawnCenter.x);
        float offsetY = Mathf.Abs(transform.position.y - spawnCenter.y);

        // Se o deslocamento for maior que o limite (largura/altura), destrói
        return offsetX > destroyLimitX || offsetY > destroyLimitY;
    }

    // Tag que você deve colocar no GameObject da Vassoura no Unity

    void OnTriggerEnter2D(Collider2D col)
    {
        // Verifica se foi a vassoura que tocou na poeira
        // Você pode checar por tag ou se o objeto tem o script Vassora
        if (col.gameObject.GetComponent<Vassora>() != null || col.CompareTag("Vassoura"))
        {
            // Toca um som ou partícula aqui se quiser
            Destroy(gameObject); // Destroi a poeira
        }
    }

}