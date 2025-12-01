using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class PlayerMoveFase3 : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] public float normalSpeed = 2f;
    [SerializeField] public float sprintSpeed = 5f;
    [SerializeField] public float snowDrag = 3f; // Velocidade lenta no stealth
    private float currentSpeed;

    [Header("Components")]
    private Rigidbody2D rb;
    private Vector2 moveInput;
    [SerializeField] private Transform interactionBox; // Opcional, como na referência
    private Vector3 lastMoveDirection;
    Vector3 offset = Vector3.zero;
    float offsetDistance = 10f; // Distância para interaction box

    private Animator animator;
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int MoveDirectionX = Animator.StringToHash("MoveDirectionX");
    private static readonly int MoveDirectionY = Animator.StringToHash("MoveDirectionY");

    [Header("Audio")]
    private EventInstance footstepInstance;
    private bool isFootstepPlaying = false;
    [SerializeField] private float footstepPitchMultiplier = 1f;
    [SerializeField] private float stealthFootstepPitchMultiplier = 0.5f; // Mais baixo no stealth para "silencioso"
    [HideInInspector] public bool isMovementPaused = false;



    // Stealth e Sprint States
    bool isStealth = false;
    bool correndo = false;

    void Start()
    {
        currentSpeed = normalSpeed;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Inicializa footsteps (como na referência)
        footstepInstance = RuntimeManager.CreateInstance(FMODEvents.instance.footstepEvent);
        footstepInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
    }

    void Update()
    {
        animator.SetBool("Correndo", correndo); ; // Desabilita animação de corrida no stealth



        UpdateFootsteps();
        UpdateInteractionBoxPosition();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (!isMovementPaused)
        {
            Walk();
        }
    }

    private void Walk()
    {
        // Atualiza direção de movimento
        if (moveInput.magnitude > 0.1f)
        {
            lastMoveDirection = moveInput.normalized;
        }



        Vector2 move = moveInput * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
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
        // Como na referência: ajusta posição da interaction box baseada na direção
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
        // Animação de caminhada, ajustada para stealth (pode adicionar "IsStealth" no Animator se quiser)
        animator.SetBool(IsWalking, moveInput.magnitude > 0.1f && !isStealth); // Usa magnitude aqui também para consistência
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
        //if (isStealth) return; // Mantenha esta linha se 'stealth' ainda bloquear sprint
        if (isStealth || isMovementPaused) return; // Adicione o bloqueio da pausa aqui

        correndo = value.isPressed;
        currentSpeed = correndo ? sprintSpeed : normalSpeed;
    }

    // Modos Stealth





    private void OnDestroy()
    {
        footstepInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        footstepInstance.release();
    }
}
