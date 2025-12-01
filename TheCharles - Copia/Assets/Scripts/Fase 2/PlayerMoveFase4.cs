using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class PlayerMoveFase4 : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] public float normalSpeed = 5f;
    [SerializeField] public float sprintSpeed = 10f;
    [SerializeField] public float stealthSpeed = 3f; // Velocidade lenta no stealth
    private float currentSpeed;

    [Header("Components")]
    private Rigidbody2D rb;
    private Vector2 moveInput;
    [SerializeField] private Transform interactionBox; // Opcional, como na refer�ncia
    public Vector3 lastMoveDirection;
    Vector3 offset = Vector3.zero;
    float offsetDistance = 10f; // Dist�ncia para interaction box
    [SerializeField] private Transform handPoint;


    private Animator animator;
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int MoveDirectionX = Animator.StringToHash("MoveDirectionX");
    private static readonly int MoveDirectionY = Animator.StringToHash("MoveDirectionY");

    [Header("Audio")]
    private EventInstance footstepInstance;
    private bool isFootstepPlaying = false;
    [SerializeField] private float footstepPitchMultiplier = 1f;
    [SerializeField] private float stealthFootstepPitchMultiplier = 0.5f; // Mais baixo no stealth para "silencioso"

    

    // Stealth e Sprint States
    bool isStealth = false;
    bool correndo = false;

    void Start()
    {
        currentSpeed = normalSpeed;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Inicializa footsteps (como na refer�ncia)
        footstepInstance = RuntimeManager.CreateInstance(FMODEvents.instance.footstepEvent);
        footstepInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
    }

    void Update()
    {
        animator.SetBool("Correndo", correndo && !isStealth); // Desabilita anima��o de corrida no stealth
        animator.SetBool("isStealth", isStealth);

        UpdateHandPoint();

        UpdateFootsteps();
        UpdateInteractionBoxPosition();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        Walk();
    }

    private void Walk()
    {
        // Atualiza dire��o de movimento
        if (moveInput.magnitude > 0.1f)
        {
            lastMoveDirection = moveInput.normalized;
        }

       

        Vector2 move = moveInput * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    private void UpdateHandPoint()
    {
        int playerOrder = GetComponent<SpriteRenderer>().sortingOrder;

        Vector3 pos = Vector3.zero;
        int layer = playerOrder + 1; // padr�o (acima do player)

        // --- prioridade: cima/baixo antes de esquerda/direita ---
        if (lastMoveDirection.y > 0) // CIMA tem prioridade
        {
            pos = new Vector3(1.2f, 1.5f, 0f);
            layer = playerOrder - 2; // por baixo do player
        }
        else if (lastMoveDirection.y < 0) // BAIXO
        {
            pos = new Vector3(1.2f, -10.8f, 0f);
            layer = playerOrder + 1; // por cima do player
        }
        else if (lastMoveDirection.x < 0) // ESQUERDA
        {
            pos = new Vector3(-6.3f, -8.9f, 0f);
            layer = playerOrder + 1;
        }
        else if (lastMoveDirection.x > 0) // DIREITA
        {
            pos = new Vector3(12.3f, -8.9f, 0f);
            layer = playerOrder + 1;
        }

        handPoint.localPosition = pos;

        // Atualiza TODOS os filhos ativos da m�o
        foreach (Transform child in handPoint)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr != null && child.gameObject.activeSelf)
            {
                // Atualiza layer
                sr.sortingOrder = layer;

                // Flip apenas se estiver indo S� para direita
                if (lastMoveDirection.x > 0 && lastMoveDirection.y == 0)
                {
                                        sr.flipX = false;

                    pos = new Vector3(12.3f, -8.9f, 0f);
                }


                else
                    sr.flipX = true;
            }
        }
    }

    private void UpdateFootsteps()
    {
        bool shouldMove = moveInput.magnitude > 0.1f;

        footstepInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

        float pitchMultiplier = isStealth ? stealthFootstepPitchMultiplier : footstepPitchMultiplier;
        float speedRatio = currentSpeed / normalSpeed;

        if (shouldMove && !isFootstepPlaying)
        {
            footstepInstance.setPitch(speedRatio * pitchMultiplier);
            footstepInstance.start();
            isFootstepPlaying = true;
        }
        else if (!shouldMove && isFootstepPlaying)
        {
            footstepInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isFootstepPlaying = false;
        }
        else if (shouldMove && isFootstepPlaying)
        {
            footstepInstance.setPitch(speedRatio * pitchMultiplier);
        }
    }

    private void UpdateInteractionBoxPosition()
    {
        // Como na refer�ncia: ajusta posi��o da interaction box baseada na dire��o
        if (Mathf.Abs(lastMoveDirection.x) > Mathf.Abs(lastMoveDirection.y))
        {
            offset = lastMoveDirection.x > 0 ? Vector3.right * offsetDistance : Vector3.left * offsetDistance;
        }
        else
        {
            offset = lastMoveDirection.y > 0 ? Vector3.up * offsetDistance : Vector3.down * offsetDistance;
        }
        if (interactionBox != null)
            interactionBox.localPosition = offset;
    }

    private void UpdateAnimation()
    {
        // Anima��o de caminhada, ajustada para stealth (pode adicionar "IsStealth" no Animator se quiser)
        animator.SetBool(IsWalking, moveInput.magnitude > 0.1f && !isStealth); // Usa magnitude aqui tamb�m para consist�ncia
        if (moveInput.magnitude > 0.1f)
        {
            animator.SetFloat(MoveDirectionX, moveInput.x);
            animator.SetFloat(MoveDirectionY, moveInput.y);
        }
    }

    // Input System: Movimento com WASD (CORRIGIDO: sem value.isPressed)
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Input System: Sprint (desabilitado no stealth)
    public void OnCorrer(InputValue value)
    {
        if (isStealth) return; // N�o permite sprint no stealth

        correndo = value.isPressed; // Aqui isPressed funciona porque "Correr" � uma a��o de Button
        currentSpeed = correndo ? sprintSpeed : normalSpeed;
    }

    // Modos Stealth
    public void EnterStealthMode()
    {
        isStealth = true;
        currentSpeed = stealthSpeed;
        correndo = false;
    }

    public void ExitStealthMode()
    {
        isStealth = false;
        currentSpeed = normalSpeed; 
    }

   

    

    private void OnDestroy()
    {
        footstepInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        footstepInstance.release();
    }
}
