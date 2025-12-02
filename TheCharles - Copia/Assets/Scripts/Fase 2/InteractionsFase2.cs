using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class InteractionsFase2 : MonoBehaviour
{
    [Header("UI References (assigned in inspector or dynamically)")]
    private bool apertouF;
    private PlacarFase2 placar;
    private DestroyFase2 destroi;
    private GameObject objetoAtual;
    private Animator anim;
    private PlayerMoveFase2 pl;
    bool coletando;
    public GameObject android;
    public int missao;
    public GameObject objMissao1, objMissao2, objMissao3;
    public GameObject canvaMissao1, canvaMissao11, canvaMissao21, canvaMissao31, canvaMissao2, canvaMissao3;
    public GameObject armadilha, armadilhaChao;
    public GameObject redeBorbo;
    public GameObject armadilhaChaoPassaro, armadilhaPassaro;
    public GameObject regMacaco, regPassaro;
    float timer;
    bool comecaTimer;
    public CollectFeedBack fb;
    public GameObject posPlayerArmadilha, posPlayerArmadilhaPassaro;
    public GameObject cameraMacaco, cameraPassaro, mainCamera;
    public RedeBorboleta ativacoleta;
    void Awake()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            android.SetActive(true);
        }
        missao++;
        pl = GetComponent<PlayerMoveFase2>();
        anim = GetComponent<Animator>();
        placar = GameObject.Find("Hud").GetComponent<PlacarFase2>();
        destroi = GetComponent<DestroyFase2>();

    }

    void Update()
    {
        anim.SetBool("Coletando", coletando);

        if (coletando == true)
        {

            StartCoroutine(StopCollectingAfterDelay(0.4f)); // Inicia a coroutine com um atraso de 1 segundo

        }
         VerificaMissao();
         if(comecaTimer)
         {
            anim.enabled = false;
            timer +=Time.deltaTime;
             Debug.Log(timer);
                if(timer>=4)
                {
                if (missao == 2) { 
                    armadilhaChao.SetActive(false);
                    missao++;
                    cameraMacaco.SetActive(false);
                    mainCamera.SetActive(true);
                 }
           

                if(missao == 6)
            {
                    armadilhaChaoPassaro.SetActive(false);
                    missao++;
                    cameraPassaro.SetActive(false);
                    mainCamera.SetActive(true);
                }
            timer = 0;
            comecaTimer = false;
            pl.enabled = true;
            anim.enabled = true;

                 }
        }
         
    }
   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Folha") || other.CompareTag("Madeira") || other.CompareTag("Semente") || other.CompareTag("Pedra") || other.CompareTag("Banana") || other.CompareTag("RegiaoMacaco") || other.CompareTag("Borboleta") || other.CompareTag("RegiaoPassaro"))
        {
            objetoAtual = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == objetoAtual)
        {
            objetoAtual = null;
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (apertouF && objetoAtual != null)
        {

            if (missao == 2 && other.CompareTag("RegiaoMacaco"))
            {
                transform.position = posPlayerArmadilha.transform.position;
                armadilhaChao.SetActive(true);
                armadilha.SetActive(false);
                pl.enabled = false;
                comecaTimer = true;
                cameraMacaco.SetActive(true);
                mainCamera.SetActive(false);
            }
            if (missao == 6 && other.CompareTag("RegiaoPassaro"))
            {
                Debug.Log("laele");
                transform.position = posPlayerArmadilhaPassaro.transform.position;

                armadilhaChaoPassaro.SetActive(true);
                armadilhaPassaro.SetActive(false);
                comecaTimer = true;
                pl.enabled = false;
                cameraPassaro.SetActive(true);
                mainCamera.SetActive(false);

            }
            if (missao == 4 && other.CompareTag("Borboleta"))
            {
                other.GetComponent<DestroyFase2>().Destruir();
                StartCoroutine(StopCollectingAfterDelayBorboleta(0.5f));
                placar.PlacarBorboleta(1);
                ativacoleta.coletaBorbo = true;
            }


            if (other.CompareTag("Folha"))
            {

                other.GetComponent<DestroyFase2>().Destruir();
                placar.PlacarFolha(1);
                coletando = true;

            }
            else if (other.CompareTag("Madeira"))
            {

                other.GetComponent<DestroyFase2>().Destruir();
                placar.PlacarMadeira(1);
                coletando = true;

            }
            else if (other.CompareTag("Pedra"))
            {

                other.GetComponent<DestroyFase2>().Destruir();
                placar.PlacarPedra(1);
                coletando = true;

            }
            else if (other.CompareTag("Banana"))
            {

                other.GetComponent<DestroyFase2>().Destruir();
                placar.PlacarBanana(1);
                coletando = true;

            }
            else if (other.CompareTag("Semente"))
            {

                other.GetComponent<DestroyFase2>().Destruir();
                placar.PlacarSemente(1);
                coletando = true;

            }
        }
       
    }

    void VerificaMissao()
    {
        if(missao == 1)
        {
            canvaMissao1.SetActive(true);
            canvaMissao2.SetActive(false);
            canvaMissao3.SetActive(false);
            objMissao1.SetActive(true);
            objMissao2.SetActive(false);
            objMissao3.SetActive(false);
            if(placar.madeiraAtual >= 2 && placar.bananaAtual >= 1 && placar.folhaAtual >= 2 && placar.pedraAtual >= 2)
            {
                placar.madeiraAtual= 0;
                placar.folhaAtual= 0;
                placar.pedraAtual= 0;
                placar.bananaAtual= 0;
                placar.sementeAtual = 0;
                placar.UpdateUI();
                armadilha.SetActive(true);
                missao++;
            }
        }
        if (missao == 2)
        {
            canvaMissao11.SetActive(true);
            canvaMissao1.SetActive(false);
            canvaMissao2.SetActive(false);
            canvaMissao3.SetActive(false);
            objMissao1.SetActive(false);
            objMissao2.SetActive(false);
            objMissao3.SetActive(false);
            regMacaco.SetActive(true);
        }
        if (missao == 3)
        {
            anim.enabled = true;
            armadilhaChao.SetActive(false);
            canvaMissao1.SetActive(false);
            canvaMissao2.SetActive(true);
            canvaMissao3.SetActive(false);
            objMissao1.SetActive(false);
            objMissao2.SetActive(true);
            objMissao3.SetActive(false);
            regMacaco.SetActive(false);
            if(placar.madeiraAtual >= 2 && placar.folhaAtual >= 2)
            {
                placar.ResetPlacar();
                missao++;
            }
        }
        if (missao == 4)
        {
            canvaMissao21.SetActive(true);
            canvaMissao1.SetActive(false);
            canvaMissao2.SetActive(false);
            canvaMissao3.SetActive(false);
            objMissao1.SetActive(false);
            objMissao2.SetActive(false);
            objMissao3.SetActive(false);
            redeBorbo.SetActive(true);
           
            

        }
        if (missao == 5)
        {
            canvaMissao1.SetActive(false);
            canvaMissao2.SetActive(false);
            canvaMissao3.SetActive(true);
            objMissao1.SetActive(false);
            objMissao2.SetActive(false);
            objMissao3.SetActive(true);
            if(placar.madeiraAtual >= 2 && placar.sementeAtual >= 1 && placar.folhaAtual >= 2 && placar.pedraAtual >= 2)
            {
                missao++;
            }
        }
        if (missao == 6)
        {
            armadilhaPassaro.SetActive(true);

            regPassaro.SetActive(true);
            canvaMissao31.SetActive(true);
            canvaMissao1.SetActive(false);
            canvaMissao2.SetActive(false);
            canvaMissao3.SetActive(false);
            objMissao1.SetActive(false);
            objMissao2.SetActive(false);
            objMissao3.SetActive(false);
        }
        if (missao == 7)
        {
            SceneManager.LoadScene(8);

        }

    }
   
    private IEnumerator StopCollectingAfterDelay(float delay)
    {
        
        pl.enabled = false;
        yield return new WaitForSeconds(delay); // Espera pelo tempo especificado
        coletando = false; // Define coletando como false
        pl.enabled = true;
    }
    private IEnumerator StopCollectingAfterDelayBorboleta(float delay)
    {
        if (placar.borboletaAtual == 1)
        {
            pl.enabled = false;
            yield return new WaitForSeconds(delay); // Espera pelo tempo especificado
            pl.enabled = true;
            redeBorbo.SetActive(false);
            missao++;

        }
        if (placar.borboletaAtual == 0)
        {
            pl.enabled = false;
            yield return new WaitForSeconds(delay); // Espera pelo tempo especificado
            pl.enabled = true;
            
        }

    }
    public void OnInteracts(InputValue value)
    {
        apertouF = value.isPressed;
    }
 

}
