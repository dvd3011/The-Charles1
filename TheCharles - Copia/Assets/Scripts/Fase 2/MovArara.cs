using UnityEngine;
using System.Collections; // Necessário para usar Coroutines

public class MovArara : MonoBehaviour
{
    [Header("Alvo")]
    [Tooltip("O objeto (Transform) para o qual este objeto se moverá.")]
    public Transform target;

    [Header("Configurações de Movimento")]
    [Tooltip("Velocidade de movimento em unidades por segundo.")]
    public float speed = 5f;

    [Header("Configurações de Colisão")]
    [Tooltip("A tag dos objetos a serem coletados/destruídos.")]
    private const string CollectTag = "Semente";
    public bool taNaarmadilha = false;
    private Animator anim;

    // Variavel para controlar a Coroutine
    private Coroutine movementCoroutine;

    // --- MÉTODOS DE CICLO DE VIDA ---

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // Inicia a coroutine de movimento apenas quando o objeto é ativado
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }
        // Armazena a referência para poder pará-la
        movementCoroutine = StartCoroutine(MoveTowardsTarget());
    }

    private void OnDisable()
    {
        // Para a coroutine de movimento quando o objeto é desativado
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
            anim.SetBool("taNaarmadilha", taNaarmadilha);

            // 1. Verifica se há um alvo definido
            if (target == null)
            {
                Debug.LogWarning("O alvo (Target) não está definido para " + gameObject.name + ". Movimento parado.");
                break; // Sai do loop e encerra a coroutine
            }

            // 2. Calcula a direção e a distância até o alvo
            Vector3 direction = target.position - transform.position;
            direction.Normalize();

            // 3. Move o objeto em direção ao alvo
            transform.position += direction * speed * Time.deltaTime;

            // Espera pelo próximo frame para continuar o movimento
            yield return null;
        }
    }

    // --- LÓGICA DE ANIMAÇÃO E COLISÃO ---

    // O método Start foi substituído por Awake e a lógica de animação movida para a Coroutine

    private void Update()
    {
        // Opcional: Se houver alguma lógica que PRECISA ser executada em todo frame
        // mesmo quando o movimento estiver desativado ou o objeto estiver ativo.
        // Se a coroutine já cuida de tudo, este método pode ser removido.
    }

    private void OnTriggerEnter2D(Collider2D other) // Para colisões 2D
    {
        // 4. Verifica se o objeto colidido tem a tag "Semente"
        if (other.CompareTag(CollectTag))
        {
            Debug.Log($"Coletando/Destruindo {other.gameObject.name} (Semente).");
            Destroy(other.gameObject);
        }

        // CORREÇÃO: A tag "armadilha" estava sendo checada dentro da tag "Semente".
        // Presumindo que "armadilha" é uma tag diferente, o bloco foi movido para fora do if da Semente:
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