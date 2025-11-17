using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poeira : MonoBehaviour
{
    private string tagVassoura = "Vassoura";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
   
    // Tag que você deve colocar no GameObject da Vassoura no Unity

    void OnTriggerEnter2D(Collider2D col)
    {
        // Verifica se foi a vassoura que tocou na poeira
        // Você pode checar por tag ou se o objeto tem o script Vassora
        if (col.gameObject.GetComponent<Vassora>() != null || col.CompareTag(tagVassoura))
        {
            // Toca um som ou partícula aqui se quiser
            Destroy(gameObject); // Destroi a poeira
        }
    }
}