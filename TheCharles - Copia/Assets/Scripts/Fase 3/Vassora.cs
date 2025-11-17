using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Vassora : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 lastMousePosition;
    private Vector3 deltaMouse;
    private Vector3 startPosition;

    // Offset para manter a vassoura na posição relativa onde você clicou
    private Vector3 dragOffset;

    void Start()
    {
        startPosition = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Calcula a diferença entre o centro da vassoura e onde o mouse clicou
        // Isso evita que a vassoura "pule" para centralizar no mouse
        dragOffset = transform.position - GetMousePositionInWorldSpace();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 currentMousePosition = Input.mousePosition;

        // Cálculo do Delta para a física (empurrar objetos)
        // Usamos coordenadas de tela para o delta, ou convertemos para mundo se preferir precisão
        deltaMouse = currentMousePosition - lastMousePosition;
        lastMousePosition = currentMousePosition;

        // Move a vassoura somando a posição do mouse + a compensação (offset) inicial
        transform.position = GetMousePositionInWorldSpace() + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        deltaMouse = Vector3.zero;
        // Se quiser que a vassoura volte pro lugar original ao soltar, descomente a linha abaixo:
        // transform.position = startPosition;
    }

    public Vector3 GetMousePositionInWorldSpace()
    {
        Vector3 screenPoint = Input.mousePosition;

        // CORREÇÃO CRÍTICA:
        // Define o Z como a distância absoluta da câmera até o objeto (assumindo Z=0 para o jogo)
        // Se sua câmera está em -10 e o jogo em 0, a distância é 10.
        screenPoint.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);
        worldPoint.z = 0; // Trava o Z em 0 para manter no plano 2D

        return worldPoint;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rig = col.gameObject.GetComponent<Rigidbody2D>();

        if (rig != null)
        {
            // Ajuste a força conforme necessário
            Vector3 forceDirection = deltaMouse.normalized;
            rig.AddForce(forceDirection * 4f, ForceMode2D.Impulse);
        }
    }
}