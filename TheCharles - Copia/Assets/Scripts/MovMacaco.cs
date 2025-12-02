using UnityEngine;
using System.Collections; // Necess�rio para usar Coroutines

public class MovMacaco : MonoBehaviour
{
    [Header("Alvo")]
    [Tooltip("O objeto (Transform) para o qual este objeto se mover�.")]
    public Transform target;

    [Header("Configura��es de Movimento")]
    [Tooltip("Velocidade de movimento em unidades por segundo.")]
    public float speed = 5f;

    [Header("Configura��es de Colis�o")]
    [Tooltip("A tag dos objetos a serem coletados/destru�dos.")]
    private const string CollectTag = "Semente";
    public bool taNaarmadilha = false;

    // Variavel para controlar a Coroutine
    private Coroutine movementCoroutine;

    // --- M�TODOS DE CICLO DE VIDA ---

    private void Awake()
    {
    }

    private void OnEnable()
    {
        // Inicia a coroutine de movimento apenas quando o objeto � ativado
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }
        // Armazena a refer�ncia para poder par�-la
        movementCoroutine = StartCoroutine(MoveTowardsTarget());
    }

    private void OnDisable()
    {
        // Para a coroutine de movimento quando o objeto � desativado
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }
    }

    // --- COROUTINE DE MOVIMENTO ---

    IEnumerator MoveTowardsTarget()
    {
        // Este loop substitui o Update() para o movimento
        while (true)
        {

            // 1. Verifica se h� um alvo definido
            if (target == null)
            {
                Debug.LogWarning("O alvo (Target) n�o est� definido para " + gameObject.name + ". Movimento parado.");
                break; // Sai do loop e encerra a coroutine
            }

            // 2. Calcula a dire��o e a dist�ncia at� o alvo
            Vector3 direction = target.position - transform.position;
            direction.Normalize();

            // 3. Move o objeto em dire��o ao alvo
            transform.position += direction * speed * Time.deltaTime;

            // Espera pelo pr�ximo frame para continuar o movimento
            yield return null;
        }
    }

    // --- L�GICA DE ANIMA��O E COLIS�O ---

    // O m�todo Start foi substitu�do por Awake e a l�gica de anima��o movida para a Coroutine

    private void Update()
    {
        // Opcional: Se houver alguma l�gica que PRECISA ser executada em todo frame
        // mesmo quando o movimento estiver desativado ou o objeto estiver ativo.
        // Se a coroutine j� cuida de tudo, este m�todo pode ser removido.
    }

    private void OnTriggerEnter2D(Collider2D other) // Para colis�es 2D
    {
        // 4. Verifica se o objeto colidido tem a tag "Semente"
        if (other.CompareTag(CollectTag))
        {
            Debug.Log($"Coletando/Destruindo {other.gameObject.name} (Semente).");
            Destroy(other.gameObject);
        }

        // CORRE��O: A tag "armadilha" estava sendo checada dentro da tag "Semente".
        // Presumindo que "armadilha" � uma tag diferente, o bloco foi movido para fora do if da Semente:
        if (other.CompareTag("armadilha"))
        {
            taNaarmadilha = true;
            // Opcional: Para o movimento ao cair na armadilha
            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
                movementCoroutine = null;
            }
        }
    }
}