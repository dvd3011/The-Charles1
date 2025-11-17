using UnityEngine;

public class Planta : MonoBehaviour
{
    [Header("Área de Posicionamento")]
    [Tooltip("Canto inferior esquerdo da área")]
    public Vector2 areaMin = new Vector2(-5, -5);

    [Tooltip("Canto superior direito da área")]
    public Vector2 areaMax = new Vector2(5, 5);

    [Header("Configurações")]
    [Tooltip("Se marcado, randomiza também no eixo Z")]
    public bool includeZAxis = false;

    [Tooltip("Se marcado, randomiza a cada vez que o objeto é ativado")]
    public bool randomizeOnEnable = false;

    [Tooltip("Tamanho da área de verificação de colisão (em unidades do mundo)")]
    public Vector2 overlapSize = new Vector2(1f, 1f);

    [Tooltip("Camadas a evitar (adicione 'Arvores' aqui se necessário)")]
    public LayerMask layersToAvoid = -1; // Por padrão, todas as camadas

    Vector3 newPosition; // Declare a variável aqui

    private void Start()
    {
        RandomizePosition();
    }

    private void OnEnable()
    {
        if (randomizeOnEnable)
        {
            RandomizePosition();
        }
    }

    public void RandomizePosition()
    {
        bool validPosition = false;
        int maxAttempts = 100; // Limite de tentativas para evitar loop infinito
        int attempts = 0;

        // Tenta encontrar uma posição válida
        while (!validPosition && attempts < maxAttempts)
        {
            newPosition = new Vector3(
                Random.Range(areaMin.x, areaMax.x),
                Random.Range(areaMin.y, areaMax.y),
                includeZAxis ? Random.Range(areaMin.x, areaMax.x) : transform.position.z
            );

            // Verifica colisões usando OverlapBoxAll em todas as camadas (ou layersToAvoid)
            Collider2D[] overlappingColliders = Physics2D.OverlapBoxAll(newPosition, overlapSize, 0f, layersToAvoid);

            validPosition = true;
            foreach (var col in overlappingColliders)
            {
                if (col != null)
                {
                    // Evita se estiver na camada "Arvores" (ou qualquer camada em layersToAvoid, mas especificamente checa nome para compatibilidade)
                    if (LayerMask.LayerToName(col.gameObject.layer) == "Arvores")
                    {
                        validPosition = false;
                        break;
                    }

                    // Evita se for um BoxCollider2D com IsTrigger ativado
                    if (col is BoxCollider2D && col.isTrigger)
                    {
                        validPosition = false;
                        break;
                    }
                }
            }

            attempts++;
        }

        if (!validPosition)
        {
            Debug.LogWarning("Não foi possível encontrar uma posição válida para a planta após " + maxAttempts + " tentativas. Usando posição padrão.");
            // Opcional: fallback para posição original ou centro da área
            newPosition = new Vector3((areaMin.x + areaMax.x) / 2, (areaMin.y + areaMax.y) / 2, transform.position.z);
        }

        transform.position = newPosition;
    }

    // Desenha um gizmo para visualizar a área no Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3(
            (areaMin.x + areaMax.x) / 2,
            (areaMin.y + areaMax.y) / 2,
            transform.position.z
        );

        Vector3 size = new Vector3(
            Mathf.Abs(areaMax.x - areaMin.x),
            Mathf.Abs(areaMax.y - areaMin.y),
            0.1f
        );

        Gizmos.DrawWireCube(center, size);

        // Desenha a área de overlap como gizmo vermelho (para debug)
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(newPosition, new Vector3(overlapSize.x, overlapSize.y, 0.1f));
    }
}
