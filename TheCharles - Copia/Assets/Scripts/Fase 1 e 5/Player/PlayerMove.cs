using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    private float currentSpeed;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    [SerializeField] private Transform interactionBox;
    private Vector3 lastMoveDirection;
    Vector3 offset = Vector3.zero;
    float offsetDistance = 10;

    [SerializeField] private InteractionsI interactionsScript;

    private Animator animator;
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int MoveDirectionX = Animator.StringToHash("MoveDirectionX");
    private static readonly int MoveDirectionY = Animator.StringToHash("MoveDirectionY");

    private float timerDescanso = 10;
    private float tempoDescansando = 3;

    private EventInstance footstepInstance;
    private bool isFootstepPlaying = false;

    bool correndo;
    bool descansando;
    [Header("Footstep Settings")]
    [SerializeField] private float footstepPitchMultiplier = 1f; // Ajuste no Inspector

    void Start()
    {
        currentSpeed = normalSpeed;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        footstepInstance = RuntimeManager.CreateInstance(FMODEvents.instance.footstepEvent);
        footstepInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
    }

    void Update()
    {
       if(IsInScene("MapaAberto"))
        {
            animator.SetBool("Correndo", correndo);
        }

        if (IsInScene("Fase5"))
        {
                    animator.SetBool("Descansando", descansando);

            timerDescanso -= Time.deltaTime;
            if (timerDescanso <= 0)
            {
                currentSpeed = 0;
                descansando = true;
                tempoDescansando -= Time.deltaTime;
                if (tempoDescansando <= 0)
                {
                    descansando = false;
                    currentSpeed = normalSpeed;
                    tempoDescansando = 3;
                    timerDescanso = 10;
                }
            }
        }

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
        if (interactionsScript != null && interactionsScript.barActive)
            return;

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

        if (shouldMove && !isFootstepPlaying)
        {
            footstepInstance.setPitch((currentSpeed / normalSpeed) * footstepPitchMultiplier);
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
            // Atualiza pitch dinamicamente se estiver correndo/andando
            footstepInstance.setPitch((currentSpeed / normalSpeed) * footstepPitchMultiplier);
        }
    }

    private void UpdateInteractionBoxPosition()
    {
        if (Mathf.Abs(lastMoveDirection.x) > Mathf.Abs(lastMoveDirection.y))
        {
            offset = lastMoveDirection.x > 0 ? Vector3.right * offsetDistance : Vector3.left * offsetDistance;
        }
        else
        {
            offset = lastMoveDirection.y > 0 ? Vector3.up * offsetDistance : Vector3.down * offsetDistance;
        }
        interactionBox.localPosition = offset;
    }

    private void UpdateAnimation()
    {
        animator.SetBool(IsWalking, moveInput != Vector2.zero);
        if (moveInput != Vector2.zero)
        {
            animator.SetFloat(MoveDirectionX, moveInput.x);
            animator.SetFloat(MoveDirectionY, moveInput.y);
        }
    }

    public void OnCorrer(InputValue value)
    {
        currentSpeed = value.isPressed ? sprintSpeed : normalSpeed;
        if(value.isPressed== true)
        {
            correndo = true;
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = normalSpeed;
            correndo = false;
        }

    }


    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private bool IsInScene(string sceneName)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        return currentSceneName == sceneName;
    }

    private void OnDestroy()
    {
        footstepInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        footstepInstance.release();
    }
}
