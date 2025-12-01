using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class InteractionsFase4 : MonoBehaviour
{
    [Header("UI References (assigned in inspector or dynamically)")]
    private bool apertouF;
    private HudFase4 placar;
    private Destroyer destroi;
    private GameObject objetoAtual;
    private Animator anim;
    private PlayerMoveFase4 pl;
    bool coletando;
    public GameObject android;
    public int missao;
    public GameObject objMissao1, objMissao2;
    public GameObject canvaMissao1, canvaMissao21, canvaMissao31, canvaMissao2, canvaMissao3;
    public GameObject armadilha, armadilhaChao;
    public GameObject estilingue;
    float timer;
    bool comecaTimer;
    public GameObject stealtharea;
    public GameObject projetilPrefab;   // arrasta no Inspector
    public Transform firePoint;         // posi��o de spawn do tiro
    private float projectileSpeed = 5f; // velocidade do tiro
    public GameObject tentilhao1;
    public GameObject posIlha2, posIlha3;
    public GameObject posPlayerArmadilha;
    public GameObject cameraMacaco, mainCamera;
    public GameObject miniMapaCamera1, miniMapaCamera2, miniMapaCamera3;
    public GameObject regMacaco;
    bool tanailha2;
    void Awake()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            android.SetActive(true);
        }
        missao++;
        pl = GetComponent<PlayerMoveFase4>();
        anim = GetComponent<Animator>();
        placar = GameObject.Find("Hud").GetComponent<HudFase4>();
        destroi = GetComponent<Destroyer>();

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
            armadilhaChao.SetActive(true);

            if (timer>=4)
                {
                if (missao == 3) {
                    armadilhaChao.SetActive(false);
                    missao++;
                    cameraMacaco.SetActive(false);
                    mainCamera.SetActive(true);

                }
          regMacaco.SetActive(false);
            timer = 0;
            comecaTimer = false;
            pl.enabled = true;
            anim.enabled = true;

                 }
        }
         if(apertouF && estilingue.activeSelf)
            {
                GameObject proj = Instantiate(projetilPrefab, firePoint.position, Quaternion.identity);
                apertouF = false;
                // Pega o Rigidbody do projétil
                Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

                // Se não tiver direção, atira para a frente do player
                Vector2 direction;

                if (pl.lastMoveDirection != Vector3.zero) // você precisa expor essa variável no PlayerMoveFase4
                    direction = pl.lastMoveDirection.normalized;
                else
                    direction = Vector2.right; // fallback (direita)

                // Aplica velocidade física
                rb.velocity = direction * projectileSpeed;
            }
    }
   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cipo") || other.CompareTag("Madeira") || other.CompareTag("Fruta") || other.CompareTag("Borracha") || other.CompareTag("Tentilhao") || other.CompareTag("RegiaoMacaco") || other.CompareTag("Borboleta") || other.CompareTag("FitzRoy"))
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
            if (other.CompareTag("FitzRoy"))
            {
                if (missao > 1 && tanailha2 == false){
                    miniMapaCamera1.SetActive(false);
                    miniMapaCamera2.SetActive(true);
                    transform.position = posIlha2.transform.position;
                    tanailha2 = true;
                }
                if (missao>3 && tanailha2 == true )
                {
                    miniMapaCamera2.SetActive(false);
                    miniMapaCamera3.SetActive(true);
                    transform.position = posIlha3.transform.position;
                }
            }

            if (missao == 3 && other.CompareTag("RegiaoMacaco"))
            {
                armadilhaChao.SetActive(true);
                armadilha.SetActive(false);
                pl.enabled = false;
                comecaTimer = true;
                transform.position = posPlayerArmadilha.transform.position;
                armadilhaChao.SetActive(true);
                armadilha.SetActive(false);
                pl.enabled = false;
                comecaTimer = true;
                cameraMacaco.SetActive(true);
                mainCamera.SetActive(false);
            }
           
            if (other.CompareTag("Tentilhao"))
            {

                stealtharea.SetActive(false);
                other.GetComponent<DestroyerFase4>().Destruir();
                placar.PlacarTent(1);
                coletando = true;
                
            }


            else if (other.CompareTag("Cipo"))
            {

                Debug.Log("foi");
                other.GetComponent<DestroyerFase4>().Destruir();
                placar.PlacarCipo(1);
                coletando = true;
            }
            else if (other.CompareTag("Madeira"))
            {

                other.GetComponent<DestroyerFase4>().Destruir();
                placar.PlacarMadeira(1);
                coletando = true;
            }
        
            else if (other.CompareTag("Fruta"))
            {


                other.GetComponent<DestroyerFase4>().Destruir();
                placar.PlacarFruta(1);
                coletando = true;
            }
            else if (other.CompareTag("Borracha"))
            {
                Debug.Log("foi");

                other.GetComponent<DestroyerFase4>().Destruir();
                placar.PlacarBorracha(1);
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
            canvaMissao21.SetActive(false);
            canvaMissao31.SetActive(false);
            if(placar.tentilhaoAtual>=1)
            {
                tentilhao1.GetComponent<BirdVision>().enabled = false;
                tentilhao1.GetComponent<BirdController>().enabled = false;
                placar.ResetPlacar();
                placar.UpdateUI();
                missao++;
            }
        }
        if (missao == 2)
        {
            canvaMissao1.SetActive(false);
            canvaMissao2.SetActive(true);
            canvaMissao3.SetActive(false);
            canvaMissao21.SetActive(false);
            canvaMissao31.SetActive(false);
            if (placar.madeiraAtual >= 2 && placar.frutaAtual >= 1 && placar.cipoAtual>=2)
            {
                                            placar.ResetPlacar();

                missao++;
                regMacaco.SetActive(true);
            }
        }
        if (missao == 3)
        {
            armadilhaChao.SetActive(false);
            canvaMissao1.SetActive(false);
            canvaMissao2.SetActive(false);
            canvaMissao3.SetActive(false);
            canvaMissao21.SetActive(true);
            canvaMissao31.SetActive(false);
            armadilha.SetActive(true);

            if (placar.tentilhaoAtual >=1)
            {
                placar.ResetPlacar();
                missao++;
            }
        }

        if (missao == 4)
        {
            canvaMissao21.SetActive(false);
            canvaMissao1.SetActive(false);
            canvaMissao2.SetActive(false);
            canvaMissao3.SetActive(true);   
            armadilha.SetActive(false);
            if(placar.madeiraAtual>=2 && placar.borrachaAtual>=2)
            {
                                            placar.ResetPlacar();

                missao++;
            }

        }
        if (missao == 5)
        {
            estilingue.SetActive(true);
            canvaMissao1.SetActive(false);
            canvaMissao2.SetActive(false);
            canvaMissao31.SetActive(true);
            canvaMissao3.SetActive(false);

            if (placar.tentilhaoAtual>=2)
            {
                SceneManager.LoadScene("Diario");
            }
        }
    

    }
   
    private IEnumerator StopCollectingAfterDelay(float delay)
    {
        pl.enabled = false;
        yield return new WaitForSeconds(delay); // Espera pelo tempo especificado
        coletando = false; // Define coletando como false
        pl.enabled = true;
    }
    public void OnInteracts(InputValue value)
    {
        apertouF = value.isPressed;
    }
    public void DestruirObjetoAtual()
    {
        if (objetoAtual != null)
        {
            DestroyerFase4 destroyer = objetoAtual.GetComponent<DestroyerFase4>();
            if (destroyer != null)
            {
                destroyer.Destruir();
                objetoAtual = null;
            }
        }
    }



}
