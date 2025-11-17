using UnityEngine;

public class Borboleta : MonoBehaviour
{
    [Header("Configura��o de Waypoints")]
    public Transform[] waypoints;

    [Header("Configura��es de Movimento")]
    public float moveSpeed; // Velocidade de movimento
    public float stoppingDistance = 0.5f; // Dist�ncia para considerar que chegou no waypoint
    public Vector2 waitTimeRange = new Vector2(0.5f, 2f); // Tempo de espera entre waypoints

    private int currentWaypointIndex = 0; // �ndice do waypoint atual
    public bool isWaiting = false; // Indica se o NPC est� esperando
    private float waitTimer = 0f; // Temporizador de espera
    private SpriteRenderer spriteRenderer; // Refer�ncia ao SpriteRenderer

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Obt�m o SpriteRenderer
        if (waypoints.Length > 0)
        {
            MoveToNextWaypoint();
        }
    }

    private void Update()
    {
        // Atualiza a anima��o com base na vari�vel isWaiting

        if (waypoints.Length == 0) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false; // O sapo n�o est� mais esperando
                MoveToNextWaypoint();
            }
        }
        else
        {
            MoveTowardsCurrentWaypoint();
        }

        // Controla a velocidade com base na barra
        
    }

    private void MoveTowardsCurrentWaypoint()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        float step = moveSpeed * Time.deltaTime; // C�lculo do passo

        // Move o NPC em dire��o ao waypoint atual
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, step);

        // Verifica se chegou ao waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) <= stoppingDistance)
        {
            StartWaiting();
        }
        else
        {
            // Verifica a dire��o do movimento
            if (transform.position.x < targetWaypoint.position.x)
            {
                // Se o sapo est� se movendo para a direita
                spriteRenderer.flipX = true; // N�o inverte o sprite
            }
            else if (transform.position.x > targetWaypoint.position.x)
            {
                // Se o sapo est� se movendo para a esquerda
                spriteRenderer.flipX = false; // Inverte o sprite
            }
        }
    }

    private void MoveToNextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        // Se voc� quiser parar ao chegar no �ltimo waypoint, descomente a linha abaixo:
        // if (currentWaypointIndex == 0) return; 
    }

    private void StartWaiting()
    {
        isWaiting = true; // O sapo agora est� esperando
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
