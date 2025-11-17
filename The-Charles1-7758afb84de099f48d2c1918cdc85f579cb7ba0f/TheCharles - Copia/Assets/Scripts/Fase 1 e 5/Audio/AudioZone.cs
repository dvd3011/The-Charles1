using UnityEngine;

public class AudioZone : MonoBehaviour
{
    public enum AmbienteTipo
    {
        Ambiente1,
        Ambiente2,
        Ambiente3,
        Ambiente4
    }

    [SerializeField] private AmbienteTipo tipoAmbiente;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Certifique-se de que o Player tem a Tag "Player"
        {
            switch (tipoAmbiente)
            {
                case AmbienteTipo.Ambiente1:
                    AudioManager.instance.SetBGVolume(0f);
                    AudioManager.instance.SetMXVolume(1f);
                    break;

                case AmbienteTipo.Ambiente2:
                    AudioManager.instance.SetBGVolume(0f);
                    AudioManager.instance.SetMXVolume(1f);
                    break;

                case AmbienteTipo.Ambiente3:
                    AudioManager.instance.SetBGVolume(1f);
                    AudioManager.instance.SetMXVolume(1f);
                    break;

                case AmbienteTipo.Ambiente4:
                    AudioManager.instance.SetBGVolume(0f);
                    AudioManager.instance.SetMXVolume(0.5f); // abaixa MX
                    break;
            }
        }
    }
}
