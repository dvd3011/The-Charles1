using UnityEngine;

public class BirdVision : MonoBehaviour
{
    [Header("Visão")]
    public float visionRadius = 5f;       // distância máxima de visão
    public float visionAngle = 60f;       // abertura do cone em graus
    public LayerMask playerLayer;         // layer do player
    public LayerMask obstacleLayer;       // layer de cobertura (árvores, arbustos)
    public BirdController bird;

    private Vector2 lastRayStart;
    private Vector2 lastRayEnd;
    private bool showRay;

    // Componentes visuais criados dinamicamente (in-game)
    private LineRenderer circleLine;      // Círculo amarelo para raio de visão
    private LineRenderer leftConeLine;    // Linha esquerda do cone (vermelha)
    private LineRenderer rightConeLine;   // Linha direita do cone (vermelha)
    private LineRenderer detectionRayLine; // Raio de detecção (verde, só quando ativa)

    void Awake()
    {
        // Cria o LineRenderer para o círculo de visão (amarelo, wireframe aproximado)
        GameObject circleObj = new GameObject("VisionCircle");
        circleObj.transform.SetParent(transform);
        circleObj.transform.localPosition = Vector3.zero;
        circleLine = circleObj.AddComponent<LineRenderer>();
        SetupLineRenderer(circleLine, Color.yellow, true); // true para círculo (muitos pontos)
        DrawCircle(circleLine, visionRadius, 32); // 32 segmentos para círculo suave

        // Cria linha esquerda do cone (vermelha)
        GameObject leftObj = new GameObject("LeftConeLine");
        leftObj.transform.SetParent(transform);
        leftConeLine = leftObj.AddComponent<LineRenderer>();
        SetupLineRenderer(leftConeLine, Color.red, false); // false para linha reta (2 pontos)

        // Cria linha direita do cone (vermelha)
        GameObject rightObj = new GameObject("RightConeLine");
        rightObj.transform.SetParent(transform);
        rightConeLine = rightObj.AddComponent<LineRenderer>();
        SetupLineRenderer(rightConeLine, Color.red, false);

        // Cria linha do raio de detecção (verde, inicialmente desativada)
        GameObject rayObj = new GameObject("DetectionRayLine");
        rayObj.transform.SetParent(transform);
        detectionRayLine = rayObj.AddComponent<LineRenderer>();
        SetupLineRenderer(detectionRayLine, Color.green, false);
        detectionRayLine.enabled = false;

        showRay = false;
    }

    void Update()
    {
        showRay = false;

        // Calcula limites do cone (sempre visíveis)
        Vector3 rightLimit = Quaternion.Euler(0, 0, visionAngle / 2f) * transform.right * visionRadius;
        Vector3 leftLimit = Quaternion.Euler(0, 0, -visionAngle / 2f) * transform.right * visionRadius;

        // Atualiza linhas do cone (sempre ativas)
        UpdateLineRenderer(leftConeLine, transform.position, (Vector2)transform.position + (Vector2)leftLimit);
        UpdateLineRenderer(rightConeLine, transform.position, (Vector2)transform.position + (Vector2)rightLimit);

        Collider2D hit = Physics2D.OverlapCircle(transform.position, visionRadius, playerLayer);
        if (hit != null)
        {
            Vector2 directionToPlayer = (hit.transform.position - transform.position).normalized;
            Vector2 lookDirection = transform.right;
            float angle = Vector2.Angle(lookDirection, directionToPlayer);

            if (angle < visionAngle / 2f)
            {
                RaycastHit2D ray = Physics2D.Raycast(transform.position, directionToPlayer, visionRadius, obstacleLayer);
                lastRayStart = transform.position;
                lastRayEnd = (ray.collider == null) ? (Vector2)transform.position + directionToPlayer * visionRadius : ray.point;
                showRay = true;

                // Atualiza e ativa linha do raio de detecção
                UpdateLineRenderer(detectionRayLine, lastRayStart, lastRayEnd);
                detectionRayLine.enabled = true;

                if (ray.collider == null)
                {
                    Debug.Log("O tentilhão viu o jogador! Reiniciando...");
                    if (bird != null) bird.FlyAway();
                }
                else
                {
                    Debug.Log("Jogador escondido atrás de cobertura!");
                }
            }
            else
            {
                detectionRayLine.enabled = false;
            }
        }
        else
        {
            detectionRayLine.enabled = false;
        }
    }

    // Configura um LineRenderer básico (2D, sem material customizado)
    private void SetupLineRenderer(LineRenderer lr, Color lineColor, bool isCircle)
    {
        if (lr == null) return;

        if (isCircle)
        {
            lr.positionCount = 32; // Número de pontos para círculo
        }
        else
        {
            lr.positionCount = 2; // Para linhas retas
        }

        lr.useWorldSpace = true;
        lr.startWidth = 0.1f;  // Largura inicial (ajuste para visibilidade)
        lr.endWidth = 0.1f;    // Largura final
        lr.material = new Material(Shader.Find("Sprites/Default")); // Material 2D simples
        lr.startColor = lineColor;
        lr.sortingOrder = 10;  // Por cima de outros sprites 2D (ajuste se necessário)
    }

    // Desenha um círculo com LineRenderer (wireframe)
    private void DrawCircle(LineRenderer lr, float radius, int segments)
    {
        if (lr == null) return;

        lr.positionCount = segments + 1;
        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * 360f * Mathf.Deg2Rad;
            Vector3 pos = transform.position + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            lr.SetPosition(i, pos);
        }
        lr.SetPosition(segments, lr.GetPosition(0)); // Fecha o círculo
    }

    // Atualiza posições de um LineRenderer (para linhas retas)
    private void UpdateLineRenderer(LineRenderer lr, Vector2 start, Vector2 end)
    {
        if (lr == null) return;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    // Mantém gizmos no editor para depuração (lógica inalterada)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        Vector3 rightLimit = Quaternion.Euler(0, 0, visionAngle / 2f) * transform.right * visionRadius;
        Vector3 leftLimit = Quaternion.Euler(0, 0, -visionAngle / 2f) * transform.right * visionRadius;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + rightLimit);
        Gizmos.DrawLine(transform.position, transform.position + leftLimit);

        if (showRay)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(lastRayStart, lastRayEnd);
        }
    }
}
