using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;


public class cut : MonoBehaviour
{
     public bool isStartCutscene;  // Cutscene de começo de fase
    public VideoPlayer videoPlayer;


    void Start()
    {
       
       videoPlayer.loopPointReached += OnVideoEnd;
       videoPlayer.Play();
    }
    void Update()
    {
     // Quando o v�deo terminar, chama a fun��o OnVideoEnd
        
    }


    void OnVideoEnd(VideoPlayer vp)
    {
        int cenaAtual = SceneManager.GetActiveScene().buildIndex;


        if (isStartCutscene)
        {
            // Vai para a próxima fase
            SceneManager.LoadScene(cenaAtual + 1);
        }
        else
        {
            // Vai para a cena do Diário
            SceneManager.LoadScene("Diario");
        }
        
    }
}
