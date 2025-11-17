using UnityEngine;

public class BeijaFlor : MonoBehaviour
{
    [Header("Configuração de Waypoints")]
    public Transform[] waypoints;

    [Header("Configurações de Movimento")]
    public float moveSpeed; // Velocidade de movimento
    public float stoppingDistance = 0.5f; // Distância para considerar que chegou no waypoint
    public Vector2 waitTimeRange = new Vector2(0.5f, 2f); // Tempo de espera entre waypoints

    private int currentWaypointIndex = 0; // Índice do waypoint atual
    public bool isWaiting = false; // Indica se o NPC está esperando
    private float waitTimer = 0f; // Temporizador de espera
    public GameObject barra;
    private SpriteRenderer spriteRenderer; // Referência ao SpriteRenderer

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Obtém o SpriteRenderer
        if (waypoints.Length > 0)
        {
            MoveToNextWaypoint();
        }
    }

    private void Update()
    {
        // Atualiza a animação com base na variável isWaiting

        if (waypoints.Length == 0) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false; // O sapo não está mais esperando
                MoveToNextWaypoint();
            }
        }
        else
        {
            MoveTowardsCurrentWaypoint();
        }

        // Controla a velocidade com base na barra
        if (barra.activeSelf)
        {
            moveSpeed = 0;
            isWaiting = true; // Se a barra está ativa, o sapo deve esperar
        }
        else
        {
            moveSpeed = 10;
        }
    }

    private void MoveTowardsCurrentWaypoint()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        float step = moveSpeed * Time.deltaTime; // Cálculo do passo

        // Move o NPC em direção ao waypoint atual
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, step);

        // Verifica se chegou ao waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) <= stoppingDistance)
        {
            StartWaiting();
        }
        else
        {
            // Verifica a direção do movimento
            if (transform.position.x < targetWaypoint.position.x)
            {
                // Se o sapo está se movendo para a direita
                spriteRenderer.flipX = true; // Não inverte o sprite
            }
            else if (transform.position.x > targetWaypoint.position.x)
            {
                // Se o sapo está se movendo para a esquerda
                spriteRenderer.flipX = false; // Inverte o sprite
            }
        }
    }

    private void MoveToNextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        // Se você quiser parar ao chegar no último waypoint, descomente a linha abaixo:
        // if (currentWaypointIndex == 0) return; 
    }

    private void StartWaiting()
    {
        isWaiting = true; // O sapo agora está esperando
        waitTimer = Random.Range(waitTimeRange.x, waitTimeRange.y);
    }

    // Desenha gizmos para visualizar os waypoints no Editor
    private void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            // Desenha esfera no waypoint
            Gizmos.DrawWireSphere(waypoints[i].position, 0.5f);
            // Desenha linha entre waypoints
            if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
            else if (i == waypoints.Length - 1 && waypoints[0] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
            }
        }
    }
}
