using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    // Переменая для хранения аниматора
    Animator animator;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private float jumpSpeed;

    [SerializeField]
    private float jumpButtonGracePeriod;

    [SerializeField]
    private Transform cameraTransform;

    private CharacterController characterController;
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;

    // Переменые для хранения соответствующих ID

    int IsWalkingHash;
    int IsRunningHash;

    // Переменая для хранения PlayerInput
    public PlayerInput input;


    // Переменые для ханения входяших значений
    Vector2 currentMovement;
    bool movementPressed;
    bool runPressed;
    public bool interactPressed;
    public bool LMBPressed;
    public bool InDialog = false;




    private void Awake()
    {
        input = new PlayerInput();

        // устанавливаем входяшие значения для игрока при помощи приёмников

        input.PlayerActions.Movement.performed += ctx =>
        {
            currentMovement = ctx.ReadValue<Vector2>();
            movementPressed = currentMovement.x != 0 || currentMovement.y != 0;
        };

        

        input.PlayerActions.Run.performed += ctx => runPressed = ctx.ReadValueAsButton();
        input.PlayerActions.Interact.performed += ctx => interactPressed = ctx.performed;
        input.PlayerActions.LeftClick.performed += ctx => LMBPressed = ctx.performed;

    }

    void Start()
    {
        // устанавливаем сылку на аниматор
        animator = GetComponent<Animator>();

        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;

        // устанавливаем сылки на ID

        IsWalkingHash = Animator.StringToHash("IsWalking");
        IsRunningHash = Animator.StringToHash("IsRunning");
    }

    
    void Update()
    {
        // MovementHandler();
        // RotationHandler();
        FullMovementHandler();
        //Debug.Log(currentMovement);
        Debug.Log(runPressed + " run");
        Debug.Log(interactPressed + " int");
        //Debug.Log(input.PlayerActions.Interact.WasPerformedThisFrame());
        //Debug.Log(characterController.isGrounded);

        
    }

    void MovementHandler()
    {
        // получаем значение данных из аниматора
        bool isRunning = animator.GetBool(IsRunningHash);
        bool isWalking = animator.GetBool(IsWalkingHash);

        // начинаем ходьбу если кнопки движения нажаты и игрок не ходит уже
        if(movementPressed && !isWalking)
        {
            animator.SetBool(IsWalkingHash, true);
        }
        // заканчиваем ходьбу если кнопки движения не нажаты и уже ходит 
        if (!movementPressed && isWalking)
        {
            animator.SetBool(IsWalkingHash, false);
        }
        // начинаем бег если кнопки движения нажаты и кнопка бега нажата и игрок не бежит уже
        if ((movementPressed && runPressed) && !isRunning)
        {
            animator.SetBool(IsRunningHash, true);
        }
        // заканчиваем бег если кнопки движения не нажаты или кнопка бега не нажата и игрок уже бежит 
        if ((!movementPressed || !runPressed) && isRunning)
        {
            animator.SetBool(IsRunningHash, false);
        }
    }

    void RotationHandler()
    {
        //Получаем позицию персонажа на момент запроса
        Vector3 curentPosition =  transform.position;
        // получаем смешение куда должен стремится персонаж
        Vector3 newPosition = new Vector3(currentMovement.x, 0, currentMovement.y);
        // обеденяем позиции что бы получить позицую для LookAt
        Vector3 positionToLookAt = curentPosition + newPosition;
        //поворачиваем персонажа в соответствии с позициями
        transform.LookAt(positionToLookAt);


    }

    void FullMovementHandler()
    {
        float horizontalInput = currentMovement.x;
        float verticalInput = currentMovement.y;

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);

        if(runPressed)
        {
            inputMagnitude /= 2;
        }

        animator.SetFloat("Input Magnitude", inputMagnitude, 0.05f, Time.deltaTime);

        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        JumpHandler();

        if (movementDirection != Vector3.zero)
        {
            animator.SetBool("IsMoving", true);

            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    private void OnAnimatorMove()
    {
        Vector3 velocity = animator.deltaPosition;
        velocity.y = ySpeed * Time.deltaTime;

        characterController.Move(velocity);
    }

    //private void OnApplicationFocus(bool focus)
    //{
    //    if (focus && !InDialog)
    //    {
    //        Cursor.lockState = CursorLockMode.Locked;
    //    }
    //    else
    //    {
    //        Cursor.lockState = CursorLockMode.None;
    //    }
    //}

    void JumpHandler()
    {
        ySpeed += Physics.gravity.y * Time.deltaTime;

        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpButtonPressedTime = Time.time;
        }

        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = jumpSpeed;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            characterController.stepOffset = 0;
        }
    }

    private void OnEnable()
    {
        input.PlayerActions.Enable();
    }

    private void OnDisable()
    {
        input.PlayerActions.Disable();
    }
}
