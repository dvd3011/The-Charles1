using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;

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
    public GameObject canvaMissao1, canvaMissao21, canvaMissao31, canvaMissao2, canvaMissao3;
    float timer;
    bool comecaTimer;
    [SerializeField] private float maxHeat = 100f;
    [SerializeField] private float heatLossRate = 5f; // por segundo
    [SerializeField] private float heatGainRate = 10f;
    [SerializeField] private Slider heatBar;

    private float currentHeat;
    private bool nearFire = false;
       // Referência para o sprite do player
    private SpriteRenderer spriteRenderer;
    private Color baseColor; // cor original do sprite
    private Color coldColor = new Color(0.3f, 0.3f, 1f, 1f); // azul frio

    // Lógica de Escavação (integrada)
    [Header("Escavação Geral")]
    [SerializeField] private int digsRequired = 25; // Cliques em 'F' para completar (pode variar por site)
    private int currentDigs = 0;
    public PlayerState currentState = PlayerState.Idle;
    private DigSite currentDigSite; // Referência ao site atual sendo escavado
    private GameObject currentFossil; // CORREÇÃO: Variável de instância para escopo global (fóssil instanciado)

    void Awake()
    {
        currentHeat = maxHeat;

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
    }

    void Update()
    {
        // Gerenciamento de Calor (pausado durante Digging ou Cleaning)
        if (currentState == PlayerState.Idle)
        {
            if (nearFire)
                currentHeat = Mathf.Min(maxHeat, currentHeat + heatGainRate * Time.deltaTime);
            else
                currentHeat -= heatLossRate * Time.deltaTime;
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

        // Lógica de Escavação: Presses repetidos de 'F' durante Digging
        if (currentState == PlayerState.Digging && Keyboard.current.fKey.wasPressedThisFrame)
        {
            Dig();
        }

        // Opcional: Feedback visual durante digging (ex.: barra de progresso no placar)
         if (currentState == PlayerState.Digging)
         {
             // Exemplo: placar.UpdateDigProgress(currentDigs, digsRequired); // Implemente no HudFase4 se quiser
             anim.SetBool("Escavando", true); // Trigger animação de escavação (ajuste no Animator)
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
                EndDigging(false); // Cancela sem recompensa
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (apertouF && objetoAtual != null)
        {
            if (other.CompareTag("RegEscava"))
            {
                // Inicia escavação se possível
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
        pl.enabled = false;
        yield return new WaitForSeconds(delay); // Espera pelo tempo especificado
        coletando = false; // Define coletando como false
        pl.enabled = true;
    }

    public void OnInteracts(InputValue value)
    {
        apertouF = value.isPressed;
    }

    // Método público para pausar/reativar movimento (chamado pelo FossilCleaning)
    public void PausePlayerMovement(bool pause)
    {
        if (pl != null)
        {
            pl.enabled = !pause; // true = pausa (desabilita), false = reativa
        }
        // Opcional: Pause outros inputs aqui (ex.: desabilite PlayerInput se tiver)
        Debug.Log(pause ? "Movimento do jogador pausado para limpeza." : "Movimento do jogador reativado.");
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

    // Métodos de Escavação
    public bool CanDig() // Pode ser chamado externamente se necessário
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
        pl.enabled = false; // Pausa movimento, similar à coleta
        anim.SetTrigger("StartDigging"); // Opcional: Trigger animação inicial
        Debug.Log("Iniciando escavação...");
        // Opcional: Ative partículas de neve ou UI de progresso aqui
    }

    public void Dig() // Chamado a cada pressão de 'F' durante Digging
    {
        if (currentState != PlayerState.Digging || currentDigSite == null) return;

        currentDigs++;
        // Feedback: Som de escavação, vibração no Android, ou UI
        Debug.Log("Escavando... Progresso: " + currentDigs + "/" + digsRequired);
        // Opcional: placar.ShowDigProgress(currentDigs, digsRequired);

        if (currentDigs >= digsRequired)
        {
            CompleteDigging();
        }
    }

    private void CompleteDigging()
    {
        if (currentDigSite == null) return;

        currentDigSite.isDiscovered = true; // Marca como descoberto no site
        bool isFossilFound = currentDigSite.isFossil;

        if (isFossilFound)
        {
            // Instancie o fóssil para limpeza (armazena em variável de instância)
            currentFossil = Instantiate(currentDigSite.fossilPrefab, transform.position, Quaternion.identity);

            Debug.Log("Fóssil descoberto! Preparando para limpeza.");
        }
        else
        {
            // Falso: Mensagem e destrua site
            Debug.Log("Apenas um objeto falso ou já descoberto...");
            currentFossil = null; // Sem fóssil
            EndDigging(false);
            Destroy(currentDigSite.gameObject); // Destrói o site falso
            currentDigSite = null;
            return; // Sai cedo sem ir para Cleaning
        }

        EndDigging(true); // Inicia transição para Cleaning (chama StartCleaning lá)
        currentDigSite = null;
    }

    public void EndDigging(bool foundFossil)
    {
        currentState = PlayerState.Idle; // Reset inicial para Idle
        pl.enabled = true; // Reativa movimento temporariamente
        anim.SetBool("Escavando", false);

        if (foundFossil && currentFossil != null)
        {
            currentState = PlayerState.Cleaning; // CORREÇÃO: Seta estado ANTES de chamar StartCleaning
            // CORREÇÃO: Chamada única e robusta para iniciar limpeza
            if (FossilCleaning.Instance != null)
            {
                FossilCleaning.Instance.StartCleaning(currentFossil);
                Debug.Log("Modo de limpeza ativado. Use ferramentas no fóssil.");
            }
            else
            {
                Debug.LogError("Falha ao iniciar limpeza: FossilCleaning.Instance é null! Verifique se CleaningManager está na cena.");
                // Fallback: Volta a Idle se falhar
                currentState = PlayerState.Idle;
                Destroy(currentFossil); // Limpa o fóssil instanciado
                currentFossil = null;
            }
        }
        else
        {
            // Reset para Idle diretamente (sem fóssil)
            Debug.Log("Escavação concluída (sem fóssil).");
            if (currentFossil != null)
            {
                Destroy(currentFossil); // Limpa se algo sobrou
                currentFossil = null;
            }
        }
    }

    // Método para finalizar limpeza (chamado pelo FossilCleaning quando completo)
    public void EndCleaning()
    {
        currentState = PlayerState.Idle;
        // Adicione fóssil ao inventário/placar
        if (currentFossil != null)
        {
            Destroy(currentFossil); // Destrói o fóssil após limpeza (ou adicione ao inventário)
            currentFossil = null;
        }
        Debug.Log("Limpeza concluída! Fóssil coletado.");
        // Avance missão ou fase se necessário
    }
}
