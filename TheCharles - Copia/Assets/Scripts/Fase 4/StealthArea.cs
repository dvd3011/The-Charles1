using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthArea : MonoBehaviour
{
    public GameObject mainCamera, cameraStealth;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            mainCamera.SetActive(false);
            cameraStealth.SetActive(true);
            other.GetComponent<PlayerMoveFase4>().EnterStealthMode();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            mainCamera.SetActive(true);
            cameraStealth.SetActive(false);
            other.GetComponent<PlayerMoveFase4>().ExitStealthMode();
        }
    }
}

