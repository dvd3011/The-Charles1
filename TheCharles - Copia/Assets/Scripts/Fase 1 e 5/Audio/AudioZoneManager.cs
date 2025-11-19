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

    public GameObject objBG;
    public GameObject objMusic;
    public GameObject objWalla;

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
                objBG.SetActive(false);
                objWalla.SetActive(false);
                objMusic.SetActive(true);
                break;

            case AudioZone.AmbienteTipo.Ambiente2:
                objBG.SetActive(false);
                objWalla.SetActive(false);
                objMusic.SetActive(true);
                break;

            case AudioZone.AmbienteTipo.Ambiente3:
                objBG.SetActive(true);
                objWalla.SetActive(false);
                objMusic.SetActive(true);
                break;

            case AudioZone.AmbienteTipo.Ambiente4:
                objBG.SetActive(false);
                objMusic.SetActive(false);
                objWalla.SetActive(true);
                break;
        }
    }

    public void ReaplicarAmbiente()
    {
        AplicarAmbiente(ambienteAtual);
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
