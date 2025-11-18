using UnityEngine;
using UnityEngine.EventSystems;

public class Vassora : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private RectTransform canvasRect; // O RectTransform do Painel/Canvas Pai

    // Offset para garantir que o arrasto comece exatamente onde o mouse clicou (UX)
    private Vector2 dragOffset;

    // Variáveis mantidas para a física (empurrar a Poeira)
    private Vector3 deltaMouse;
    private Vector3 lastMousePosition;
    private Vector3 startPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // Tenta encontrar o RectTransform do objeto pai
        if (rectTransform.parent != null)
        {
            canvasRect = rectTransform.parent.GetComponent<RectTransform>();
        }

        // Se o pai não tiver, tenta o Canvas raiz
        if (canvasRect == null)
        {
            canvasRect = rectTransform.root.GetComponent<RectTransform>();
        }
    }

    void Start()
    {
        // Salva a posição local inicial do UI (localPosition)
        startPosition = rectTransform.localPosition;
        lastMousePosition = Input.mousePosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Calcula o offset no espaço local da UI
        Vector2 localCursor;
        if (canvasRect != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out localCursor))
        {
            // O offset é a diferença entre onde o mouse está e o centro do objeto
            dragOffset = localCursor - (Vector2)rectTransform.localPosition;
        }

        lastMousePosition = Input.mousePosition; // Reseta para cálculo de força
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 1. CÁLCULO DE POSIÇÃO (Movimento UI)
        Vector2 localCursor;
        // Converte a posição da tela (mouse) para a posição local do Canvas
        if (canvasRect != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out localCursor))
        {
            // Aplica a nova posição, subtraindo o offset
            Vector2 newPosition = localCursor - dragOffset;
            rectTransform.localPosition = new Vector3(newPosition.x, newPosition.y, rectTransform.localPosition.z);
        }

        // 2. CÁLCULO DO DELTA (Para aplicar força na Poeira)
        Vector3 currentMousePosition = Input.mousePosition;
        deltaMouse = (currentMousePosition - lastMousePosition) / Time.deltaTime; // Velocidade em pixels/s
        lastMousePosition = currentMousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        deltaMouse = Vector3.zero;
        rectTransform.localPosition = startPosition; // Retorna à posição inicial do UI
    }

    // Este método é mantido apenas para a interação com a Poeira.
    void OnTriggerEnter2D(Collider2D col)
    {
        // ATENÇÃO: ESTE CÓDIGO SÓ FUNCIONARÁ se a Canvas estiver configurada como "World Space".
        // Caso contrário, a física (Collider2D/Rigidbody2D) é ignorada.

        Rigidbody2D rig = col.gameObject.GetComponent<Rigidbody2D>();

        if (rig != null)
        {
            Vector2 force = new Vector2(deltaMouse.x, deltaMouse.y) * 0.1f;
            rig.AddForce(force, ForceMode2D.Impulse);
        }
    }
    public void ResetPosition()
    {
        // Reseta para a posição inicial local (startPosition)
        // startPosition foi salva no método Start()
        rectTransform.localPosition = startPosition;

        // Garante que a velocidade (deltaMouse) seja zero, parando qualquer movimento de "empurrão"
        deltaMouse = Vector3.zero;
    }
}