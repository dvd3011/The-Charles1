using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public enum PlayerState { Idle, Digging, Cleaning }

public class InteractionFase3 : MonoBehaviour
{
    [Header("UI References (assigned in inspector or dynamically)")]
    private bool apertouF;
    private HudFase3 placar;
    private DestroyerFase3 destroi;
    public GameObject objetoAtual;
    private Animator anim;
    private PlayerMoveFase3 pl;
    public bool coletando;
    public GameObject android;
    public int missao;
    public GameObject objMissao1, objMissao2;
    public GameObject fossil1, fossil2, fossil3, fossil4;
    float timer;
    bool comecaTimer;
    [SerializeField] private float maxHeat = 100f;
    [SerializeField] private float heatLossRate = 5f; // por segundo
    [SerializeField] private float heatGainRate = 10f;
    [SerializeField] private Slider heatBar;

    private float currentHeat;
    private bool nearFire = false;
       // Refer�ncia para o sprite do player
    private SpriteRenderer spriteRenderer;
    private Color baseColor; // cor original do sprite
    private Color coldColor = new Color(0.3f, 0.3f, 1f, 1f); // azul frio

    // L�gica de Escava��o (integrada)
    [Header("Escava��o Geral")]
    [SerializeField] private int digsRequired = 25; // Cliques em 'F' para completar (pode variar por site)
    private int currentDigs = 0;
    public PlayerState currentState = PlayerState.Idle;
    private DigSite currentDigSite; // Refer�ncia ao site atual sendo escavado
    private GameObject currentFossil; // CORRE��O: Vari�vel de inst�ncia para escopo global (f�ssil instanciado)
    public GameObject minigameLimpezaObject;
    private SpawPoeira spoeira;
    public GameObject canvasMinigame;
    public CollectFeedBack fb;
    public Vassora vassouraScript;
    private BoxCollider2D playerCollider;
    void Awake()
    {
        currentHeat = maxHeat;  
        spoeira = FindObjectOfType<SpawPoeira>();
        if (Application.platform == RuntimePlatform.Android)
        {
            android.SetActive(true);
        }
        missao++;
        pl = GetComponent<PlayerMoveFase3>();
        anim = GetComponent<Animator>();
        placar = GameObject.Find("Hud").GetComponent<HudFase3>();
        // REMOVIDO: scriptFossil = ... (usar Instance em vez disso para robustez)

        destroi = GetComponent<DestroyerFase3>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            baseColor = spriteRenderer.color;
        playerCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if(placar.fossilAtual>=4)
        {
            SceneManager.LoadScene("Diario");

        }

        
        // Gerenciamento de Calor (pausado durante Digging ou Cleaning)
        if (currentState == PlayerState.Idle)
        {
            if (nearFire)
                currentHeat = Mathf.Min(maxHeat, currentHeat + heatGainRate * Time.deltaTime);
            else
                currentHeat -= heatLossRate * Time.deltaTime;
        }
        if (currentState == PlayerState.Cleaning)
        {
            return;
        }
        // Atualiza barra
        heatBar.value = currentHeat / maxHeat;

        // Atualiza cor do player com base no calor
        if (spriteRenderer != null)
        {
            float t = 1f - (currentHeat / maxHeat);
            // t = 0 => cheio de calor (normal)
            // t = 1 => sem calor (azulado)
            spriteRenderer.color = Color.Lerp(baseColor, coldColor, t);
        }

        if (currentHeat <= 0)
        {
            RestartLevel();
        }

        anim.SetBool("Coletando", coletando);

        if (coletando == true)
        {
            StartCoroutine(StopCollectingAfterDelay(0.4f)); // Inicia a coroutine com um atraso de 1 segundo
        }

        // L�gica de Escava��o: Presses repetidos de 'F' durante Digging
        if (currentState == PlayerState.Digging && apertouF)
        {
            Dig();
        }

        // Opcional: Feedback visual durante digging (ex.: barra de progresso no placar)
         if (currentState == PlayerState.Digging)
         {
             // Exemplo: placar.UpdateDigProgress(currentDigs, digsRequired); // Implemente no HudFase4 se quiser
             anim.SetBool("Escavando", true); // Trigger anima��o de escava��o (ajuste no Animator)
         }
         else
         {
             anim.SetBool("Escavando", false);
         }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fogueira"))
            nearFire = true;
        if (other.CompareTag("Cipo") || other.CompareTag("Madeira") || other.CompareTag("Fruta") || other.CompareTag("Borracha") || other.CompareTag("Tentilhao") || other.CompareTag("RegEscava") || other.CompareTag("Borboleta"))
        {
            objetoAtual = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Fogueira"))
            nearFire = false;
        if (other.gameObject == objetoAtual)
        {
            objetoAtual = null;
            // Se saindo de um site durante digging, cancele
            if (currentState == PlayerState.Digging && other.CompareTag("RegEscava"))
            {
            }
        }
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (apertouF && objetoAtual != null)
        {
            if (other.CompareTag("RegEscava"))
            {
                if (playerCollider != null)
                {
                    playerCollider.isTrigger = true;
                }
                // Inicia escava��o se poss�vel
                DigSite site = objetoAtual.GetComponent<DigSite>();
                if (site != null && site.CanDig())
                {
                    StartDigging(site);
                    apertouF = false; // Reset input para evitar spam
                }
            }
             }
    }

    private IEnumerator StopCollectingAfterDelay(float delay)
    {
        PausePlayerMovement(true);
        yield return new WaitForSeconds(delay); // Espera pelo tempo especificado
        coletando = false; // Define coletando como false
        PausePlayerMovement(false);
    }

    public void OnInteracts(InputValue value)
    {
        apertouF = value.isPressed;
    }

    // M�todo p�blico para pausar/reativar movimento (chamado pelo FossilCleaning)
    public void PausePlayerMovement(bool pause)
    {
        if (pl != null)
        {
            // pl.enabled = !pause; // REMOVA ESTA LINHA
            pl.isMovementPaused = pause; // USE A NOVA FLAG
        }
        Debug.Log(pause ? "Movimento do jogador pausado..." : "Movimento do jogador reativado.");
    }

    public void DestruirObjetoAtual()
    {
        if (objetoAtual != null)
        {
            DestroyerFase3 destroyer = objetoAtual.GetComponent<DestroyerFase3>();
            if (destroyer != null)
            {
                destroyer.Destruir();
                objetoAtual = null;
            }
        }
    }

    void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    // M�todos de Escava��o
    public bool CanDig() // Pode ser chamado externamente se necess�rio
    {
        return currentDigSite != null && !currentDigSite.isDiscovered;
    }

    public void StartDigging(DigSite site)
    {
        if (currentState != PlayerState.Idle || site == null) return;

        currentState = PlayerState.Digging;
        currentDigSite = site;
        currentDigs = 0;
        digsRequired = site.digsRequired; // Usa o valor do site (pode variar)
        PausePlayerMovement(true);
        Debug.Log("Iniciando escava��o...");
        if (minigameLimpezaObject != null)
        {
            minigameLimpezaObject.SetActive(false);
            canvasMinigame.SetActive(false);
            fossil1.SetActive(false);
            fossil2.SetActive(false);
            fossil3.SetActive(false);
            fossil4.SetActive(false);

        }
        // Opcional: Ative part�culas de neve ou UI de progresso aqui
    }
   

    public void Dig() // Chamado a cada press�o de 'F' durante Digging
    {
        if (currentState != PlayerState.Digging || currentDigSite == null) return;

        currentDigs++;
        Debug.Log("Escavando... Progresso: " + currentDigs + "/" + digsRequired);

        // --- NOVA L�GICA: VERIFICAR FIM DA ESCAVA��O ---
        if (currentDigs >= digsRequired)
        {
            CompleteDigging();
        }

    }
   
    void CompleteDigging()
    {
        Debug.Log("Escava��o conclu�da! Iniciando limpeza...");

        // Marca o site como descoberto
        if (currentDigSite != null)
        {
            currentDigSite.isDiscovered = true;
        }

        // Muda o estado para Cleaning (Limpeza)
        currentState = PlayerState.Cleaning;

        // Ativa o Minigame da Vassoura
        if (minigameLimpezaObject != null)
        {
            minigameLimpezaObject.SetActive(true);
            canvasMinigame.SetActive(true);
            if (placar.fossilAtual == 0)
            {
                fossil1.SetActive(true);
            }
            if (placar.fossilAtual == 1)
            {
                fossil2.SetActive(true);
            }
            if (placar.fossilAtual == 2)
            {
                fossil3.SetActive(true);
            }
            if (placar.fossilAtual == 3)
            {
                fossil4.SetActive(true);
            }
            // REMOVA esta linha, pois o spawn j� acontece no OnEnable() do SpawPoeira.cs
            // spoeira.SpawnarPoeira(); 
        }
    }
    public void FinishCleaning()
    {
        Debug.Log("Limpeza Conclu�da!");
        if (playerCollider != null)
        {
            playerCollider.isTrigger = false;
        }
        // Desativa o minigamea
        if (minigameLimpezaObject != null)
        {
            if (vassouraScript != null)
            {
                vassouraScript.ResetPosition();
            }
            fb.ShowFeedback("+1 Fossil", Color.yellow);
            placar.PlacarFossil(1);
            coletando = true;
            
            minigameLimpezaObject.SetActive(false);
            canvasMinigame.SetActive(false);
        }

        // Retorna o jogador ao estado normal
        currentState = PlayerState.Idle;
        PausePlayerMovement(true); // Reativa movimento

        // Aqui voc� pode instanciar o pr�mio/f�ssil final
        // Instantiate(currentDigSite.fossilPrefab, ...);
    }


}
