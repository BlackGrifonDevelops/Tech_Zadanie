using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    // ��������� ��� �������� ���������
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

    // ��������� ��� �������� ��������������� ID

    int IsWalkingHash;
    int IsRunningHash;

    // ��������� ��� �������� PlayerInput
    public PlayerInput input;


    // ��������� ��� ������� �������� ��������
    Vector2 currentMovement;
    bool movementPressed;
    bool runPressed;
    public bool interactPressed;
    public bool LMBPressed;
    public bool InDialog = false;




    private void Awake()
    {
        input = new PlayerInput();

        // ������������� �������� �������� ��� ������ ��� ������ ���������

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
        // ������������� ����� �� ��������
        animator = GetComponent<Animator>();

        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;

        // ������������� ����� �� ID

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
        // �������� �������� ������ �� ���������
        bool isRunning = animator.GetBool(IsRunningHash);
        bool isWalking = animator.GetBool(IsWalkingHash);

        // �������� ������ ���� ������ �������� ������ � ����� �� ����� ���
        if(movementPressed && !isWalking)
        {
            animator.SetBool(IsWalkingHash, true);
        }
        // ����������� ������ ���� ������ �������� �� ������ � ��� ����� 
        if (!movementPressed && isWalking)
        {
            animator.SetBool(IsWalkingHash, false);
        }
        // �������� ��� ���� ������ �������� ������ � ������ ���� ������ � ����� �� ����� ���
        if ((movementPressed && runPressed) && !isRunning)
        {
            animator.SetBool(IsRunningHash, true);
        }
        // ����������� ��� ���� ������ �������� �� ������ ��� ������ ���� �� ������ � ����� ��� ����� 
        if ((!movementPressed || !runPressed) && isRunning)
        {
            animator.SetBool(IsRunningHash, false);
        }
    }

    void RotationHandler()
    {
        //�������� ������� ��������� �� ������ �������
        Vector3 curentPosition =  transform.position;
        // �������� �������� ���� ������ ��������� ��������
        Vector3 newPosition = new Vector3(currentMovement.x, 0, currentMovement.y);
        // ��������� ������� ��� �� �������� ������� ��� LookAt
        Vector3 positionToLookAt = curentPosition + newPosition;
        //������������ ��������� � ������������ � ���������
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
