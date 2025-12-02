using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class cut : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
       videoPlayer.Play();
    }
    void Update()
    {
     // Quando o vídeo terminar, chama a função OnVideoEnd
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // Pega o index da cena atual
        int cenaAtual = SceneManager.GetActiveScene().buildIndex;

        // Carrega a próxima cena
        SceneManager.LoadScene(cenaAtual + 1);
    }
}
