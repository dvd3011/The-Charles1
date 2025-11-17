using UnityEngine;

public class AudioZoneManager : MonoBehaviour
{
    [System.Serializable]
    public class AmbienteArea
    {
        public string nome;
        public AudioZone.AmbienteTipo tipoAmbiente;
        public Transform center; 
        public Vector3 size = new Vector3(5, 5, 5);
    }

    public Transform player;
    public AmbienteArea[] ambientes;

    private AudioZone.AmbienteTipo ambienteAtual;

    private void Update()
    {
        foreach (var ambiente in ambientes)
        {
            if (EstaDentroDoAmbiente(player.position, ambiente))
            {
                if (ambienteAtual != ambiente.tipoAmbiente)
                {
                    ambienteAtual = ambiente.tipoAmbiente;
                    AplicarAmbiente(ambienteAtual);
                    Debug.Log($"[AudioZone] Player entrou no {ambiente.nome} ({ambienteAtual})");
                }
                return;
            }
        }

        // Player fora de todos os ambientes
        if (ambienteAtual != 0)
        {
            ambienteAtual = 0;
        }
    }

    private bool EstaDentroDoAmbiente(Vector3 pos, AmbienteArea ambiente)
    {
        Vector3 min = ambiente.center.position - ambiente.size / 2;
        Vector3 max = ambiente.center.position + ambiente.size / 2;

        return (pos.x >= min.x && pos.x <= max.x &&
                pos.y >= min.y && pos.y <= max.y &&
                pos.z >= min.z && pos.z <= max.z);
    }

    private void AplicarAmbiente(AudioZone.AmbienteTipo tipo)
    {
        switch (tipo)
        {
            case AudioZone.AmbienteTipo.Ambiente1:
                AudioManager.instance.SetBGVolume(0f);
                AudioManager.instance.SetMXVolume(1f);
                AudioManager.instance.SetWallafxVolume(0f);
                break;

            case AudioZone.AmbienteTipo.Ambiente2:
                AudioManager.instance.SetBGVolume(0f);
                AudioManager.instance.SetMXVolume(1f);
                AudioManager.instance.SetWallafxVolume(0f);
                break;

            case AudioZone.AmbienteTipo.Ambiente3:
                AudioManager.instance.SetBGVolume(1f);
                AudioManager.instance.SetMXVolume(1f);
                AudioManager.instance.SetWallafxVolume(0f);
                break;

            case AudioZone.AmbienteTipo.Ambiente4:
                AudioManager.instance.SetBGVolume(0f);
                AudioManager.instance.SetMXVolume(0.5f);
                AudioManager.instance.SetWallafxVolume(1f); // toca wallafx
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (ambientes != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            foreach (var ambiente in ambientes)
            {
                if (ambiente.center != null)
                    Gizmos.DrawCube(ambiente.center.position, ambiente.size);
            }
        }
    }
}
