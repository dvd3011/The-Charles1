using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedeBorboleta : MonoBehaviour
{
  public bool coletaBorbo = false;
    private Animator anim;
    public int contador;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("Coletando", coletaBorbo);
        if(coletaBorbo)
            {
            Debug.Log("ficou true");
            StartCoroutine(StopCollectingAfterDelay(0.5f));
        }
    }
    public void ColetaBorboleta()
    {
        coletaBorbo = true;

    }
    public IEnumerator StopCollectingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Espera pelo tempo especificado
        Debug.Log("ficou false");
        coletaBorbo = false; // Define coletando como false
    }
}
