using UnityEngine;
using FMODUnity;

public class PlaySoundOnXEnabled : MonoBehaviour
{
    [Tooltip("Lista de objetos que serão monitorados")]
    [SerializeField] private GameObject[] objectsToWatch;

    private bool[] wasActive;

    private void Start()
    {
        wasActive = new bool[objectsToWatch.Length];

        
        for (int i = 0; i < objectsToWatch.Length; i++)
        {
            if (objectsToWatch[i] != null)
                wasActive[i] = objectsToWatch[i].activeSelf;
        }
    }

    private void Update()
    {
        for (int i = 0; i < objectsToWatch.Length; i++)
        {
            GameObject obj = objectsToWatch[i];
            if (obj == null) continue;

            bool isCurrentlyActive = obj.activeSelf;

            
            if (!wasActive[i] && isCurrentlyActive)
            {
                PlayEnableSound(obj.transform.position);
            }

            
            wasActive[i] = isCurrentlyActive;
        }
    }

    private void PlayEnableSound(Vector3 position)
    {
        RuntimeManager.PlayOneShot(FMODEvents.instance.armarSound, position);
    }
}
