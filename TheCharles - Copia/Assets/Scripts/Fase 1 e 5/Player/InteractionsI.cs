using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;

public class InteractionsI : MonoBehaviour
{
    [Header("UI References (assigned in inspector or dynamically)")]
    public RectTransform barContainer;
    public Image barBackground;
    public Image highlightArea;
    public BarMovement barMovement;

    [Header("Bar Highlight Settings")]
    [Range(0.1f, 0.8f)]
    public float highlightWidthNormalized = 0.2f;

    public bool barActive = false;
    private float barWidth;
    private InputAction clickAction;
    private bool apertouF;

    private float minHighlightPos = -281f;
    private float maxHighlightPos = 287f;

    private string caller;

    public  bool missaoAtiva = false;
    public bool esperandoNovaMissao = true;

    private Placar placar;
    private Destroyer destroi;
    private float ultimoTempoInteracao = -2f;
    private bool missaoCompletou;
    public int missaoAtual = -1;
    public GameObject objsMissao1, objsMissao2, objsMissao3;
    public GameObject canvaMissao1, canvaMissao2, canvaMissao3;
    public float timerMissao2 = 30f;
    public float timerMissao3 = 20f;
    public bool missao2Ativa = false;
    public bool missao3Ativa = false;
    public GameObject[] livros;
    public GameObject[] livrosMissao3;
    private GameObject objetoAtual;
    public BarraDeAfeto barra;
    public float afeto = 0;
    private Animator anim;
    private PlayerMove pl;
    bool coletando;
    public GameObject android;
    public CollectFeedBack fb;
    public GameObject minimapaUNI, minimapaABERTO, minimapaBIBLIO;
    public GameObject timer1, timer2;

    void Awake()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            android.SetActive(true);
        }
        
        pl = GetComponent<PlayerMove>();
        anim = GetComponent<Animator>();
        placar = GameObject.Find("Hud").GetComponent<Placar>();
        destroi = GetComponent<Destroyer>();

        barWidth = barContainer.rect.width;
        barMovement.Initialize(barWidth);
        HideBar();

        clickAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/leftButton");
        clickAction.performed += ctx => OnClick();
        clickAction.Enable();
    }

    void Update()
    {
        if(IsInScene("Fase5") && placar.sapoAtual>=2 && placar.beijaAtual>=2)
        {
                            SceneManager.LoadScene("Cutscene10");

        }
        anim.SetBool("Coletando", coletando);

        if (coletando == true)
        {
            StartCoroutine(StopCollectingAfterDelay(0.4f)); // Inicia a coroutine com um atraso de 1 segundo

        }
        barra.AlterarBarraDeAfeto(afeto);
        if (missao3Ativa)
        {
            timer2.SetActive(true);

            timerMissao3 -= Time.deltaTime;
            if (timerMissao3 <= 0 && placar.livroAtual < 2)
            {
                timerMissao3 = 20;

                foreach (var livro in livrosMissao3)
                {
                    livro.GetComponent<BoxCollider2D>().enabled = true;

                    livro.SetActive(true);
                    RestaurarAlfa(livro); // <-- aqui garante que o alfa volte a 100%
                }

                placar.livroAtual = 0;
                placar.UpdateUI();
                transform.position = new Vector3(-169.4f, 23.8f, 0);
            }

            if (placar.livroAtual >= 2)
            {
                timer2.SetActive(false);

                missao3Ativa = false;
            }
        }

        if (missao2Ativa)
        {
            timer1.SetActive(true);
            timerMissao2 -= Time.deltaTime;
            if (timerMissao2 <= 0 && placar.livroAtual < 4)
            {
                timerMissao2 = 30;

                foreach (var livro in livros)
                {
                    livro.GetComponent<BoxCollider2D>().enabled = true;

                    livro.SetActive(true);
                    RestaurarAlfa(livro); // <-- mesmo aqui
                }

                placar.livroAtual = 0;
                placar.UpdateUI();
                transform.position = new Vector3(-169.4f, 23.8f, 0);
            }
            if (placar.livroAtual >= 4)
            {
                timer1.SetActive(false);

                missao2Ativa = false;
            }
        }


        if (barActive)
        {
            barMovement.MovePointer();
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
    }

    void PlayInteractionSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.interactionSound, transform.position);
    }
    void PlayDocInt()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.interactionDocs, transform.position);
    }

    void OnClick()
    {
        if (!barActive)
            return;

        float highlightLeft = highlightArea.rectTransform.anchoredPosition.x - (highlightArea.rectTransform.sizeDelta.x / 2);
        float highlightRight = highlightLeft + highlightArea.rectTransform.sizeDelta.x;
        float pointerX = barMovement.pointer.rectTransform.anchoredPosition.x;

        if (pointerX >= highlightLeft && pointerX <= highlightRight)
        {
            PlayInteractionSound();

            if (caller == "Sapo")
            {

                Debug.Log("Você acertou o sapo!");
                objetoAtual.GetComponent<Destroyer>().Destruir();
                barMovement.pointerSpeed = 200;
                placar.PlacarSapo(1);
                coletando = true;

            }
            else if (caller == "BeijaFlor")
            {
                Debug.Log("Você acertou o beija flor!");
                objetoAtual.GetComponent<Destroyer>().Destruir();
                placar.PlacarBeija(1);
                coletando = true;
            }
        }
        else
        {
            Debug.Log("Missed!");
            fb.ShowFeedback("Errou!", Color.yellow);

        }

        HideBar();
    }

    public void ShowBar(string caller)
    {
        pl.enabled = false;
        anim.enabled = false;
        this.caller = caller;
        barWidth = barContainer.rect.width;
        SetHighlightArea();
        barMovement.ResetPointer();

        barContainer.gameObject.SetActive(true);
        barActive = true;

        if (caller == "BeijaFlor") barMovement.SpeedBeija();
        if (caller == "Sapo") barMovement.SpeedSapo();
    }

    public void HideBar()
    {

        anim.enabled = true;
        pl.enabled = true;
        barActive = false;
        barContainer.gameObject.SetActive(false);
    }

    void SetHighlightArea()
    {
        float width = highlightWidthNormalized * barWidth;
        float randomPosX = Random.Range(minHighlightPos + (width / 2), maxHighlightPos - (width / 2));
        highlightArea.rectTransform.sizeDelta = new Vector2(width, highlightArea.rectTransform.sizeDelta.y);
        highlightArea.rectTransform.anchoredPosition = new Vector2(randomPosX, highlightArea.rectTransform.anchoredPosition.y);
    }

    public void OnInteracts(InputValue value)
    {
        apertouF = value.isPressed;
    }

    public void DestruirObjetoAtual()
    {
        if (objetoAtual != null)
        {
            Destroyer destroyer = objetoAtual.GetComponent<Destroyer>();
            if (destroyer != null)
            {
                destroyer.Destruir();
                objetoAtual = null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PortaBiblio"))
        {
            if (missaoAtual == 1) missao2Ativa = true;
            if (missaoAtual == 2) missao3Ativa = true;
        }

        if (other.CompareTag("Planta") || other.CompareTag("Livro") || other.CompareTag("Sapo") || other.CompareTag("BeijaFlor") || other.CompareTag("Henslow"))
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
            if (other.CompareTag("Planta"))
            {  
                //PlayInteractionSound();
                other.GetComponent<Destroyer>().Destruir();
                placar.PlacarPlanta(1);
                coletando = true;
            }
            else if (other.CompareTag("Livro"))
            {

                PlayDocInt();
                other.GetComponent<Destroyer>().Destruir();
                placar.PlacarLivro(1);
                coletando = true;
                apertouF = false;
            }
            else if (other.CompareTag("Sapo") && barActive == false)
            {

                //PlayInteractionSound();
                ShowBar("Sapo");
                objetoAtual = other.gameObject;
            }
            else if (other.CompareTag("BeijaFlor") && barActive == false)
            {
                //PlayInteractionSound();
                ShowBar("BeijaFlor");
                objetoAtual = other.gameObject;
            }
            else if (other.CompareTag("Henslow") && Time.time - ultimoTempoInteracao >= 2f)
            {
                ultimoTempoInteracao = Time.time;
                CheckMissionProgress();

                if (!missaoAtiva && esperandoNovaMissao)
                {
                    // Começar nova missão
                    StartMission();
                }
                else if (missaoAtiva)
                {
                    // Jogador tem uma missão ativa
                    if (missaoCompletou)
                    {
                        EncerrarMissao();
                        afeto += 25;
                        fb.ShowFeedback("Missão concluída! Fale com Henslow novamente para a próxima.", Color.yellow);
                    }
                    else
                    {
                        fb.ShowFeedback("Você ainda não completou a missão atual.", Color.red);
                    }
                }
            }
        }
         }

    private void StartMission()
    {
        missaoAtual++; // avança missão
        missaoAtiva = true;
        esperandoNovaMissao = false;
        missaoCompletou = false;

        Debug.Log($"Missão {missaoAtual + 1} iniciada!");
        fb.ShowFeedback($"Missão {missaoAtual + 1} iniciada!", Color.yellow);

        // Ativar objetivos conforme o número da missão
        if (missaoAtual == 0)
        {
            objsMissao1.SetActive(true);
            canvaMissao1.SetActive(true);
        }
        else if (missaoAtual == 1)
        {
            objsMissao2.SetActive(true);
            canvaMissao2.SetActive(true);
        }
        else if (missaoAtual == 2)
        {
            objsMissao3.SetActive(true);
            canvaMissao3.SetActive(true);
        }

        CheckMissionProgress();
    }

    private void EncerrarMissao()
    {
        missaoAtiva = false;
        esperandoNovaMissao = true;
        missaoCompletou = false;

        Debug.Log($"Missão {missaoAtual + 1} encerrada. Fale com Henslow novamente para iniciar a próxima.");

        placar.ResetPlacar();

        // Desativa todos os objetos e canvas
        objsMissao1.SetActive(false);
        objsMissao2.SetActive(false);
        objsMissao3.SetActive(false);
        canvaMissao1.SetActive(false);
        canvaMissao2.SetActive(false);
        canvaMissao3.SetActive(false);
    }


    public void CheckMissionProgress()
    {
        if (missaoAtual == 0)
        {
            if (placar.plantaAtual >= 3 && placar.sapoAtual >= 2)
            {
                missaoCompletou = true;
            }
        }
        else if (missaoAtual == 1)
        {
            if (placar.livroAtual >= 4)
            {
                missaoCompletou = true;
                missao2Ativa = false;
            }
        }
        else if (missaoAtual == 2)
        {
            if (placar.sapoAtual >= 2 && placar.beijaAtual >= 2 && placar.plantaAtual >= 3 && placar.livroAtual >= 2)
            {
                missaoCompletou = true;
                missao3Ativa = false;
                SceneManager.LoadScene("Cutscene3");

            }
        }
    }

    // Método chamado quando um campo é alterado no Inspector
    private void OnValidate()
    {
        // Verifica se a missão atual foi alterada
        if (missaoAtual >= 0)
        {
            CheckMissionProgress();
        }
    }
    private IEnumerator StopCollectingAfterDelay(float delay)
    {
        pl.enabled = false;
        yield return new WaitForSeconds(delay); // Espera pelo tempo especificado
        coletando = false; // Define coletando como false
        pl.enabled = true;
    }
    private void RestaurarAlfa(GameObject objeto)
    {
        // Tenta pegar um SpriteRenderer
        SpriteRenderer sr = objeto.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color cor = sr.color;
            cor.a = 1f;
            sr.color = cor;
        }

        // Caso o livro tenha UI (por exemplo, uma imagem 2D no Canvas)
        Image img = objeto.GetComponent<Image>();
        if (img != null)
        {
            Color cor = img.color;
            cor.a = 1f;
            img.color = cor;
        }
    }
private bool IsInScene(string sceneName)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        return currentSceneName == sceneName;
    }
}
