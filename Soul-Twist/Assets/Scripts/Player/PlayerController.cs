using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [Header("Player")]
    [Tooltip("Velocidad de movimiento del personaje en m/s")]
    [SerializeField] float walkSpeed = 2.0f;

    [Tooltip("Velocidad de sprint del personaje en m/s")]
    [SerializeField] float SprintSpeed = 5.335f;

    [Tooltip("Aceleración y desaceleración")]
    public float SpeedChangeRate = 10.0f;

    [Tooltip("La rapidez con el que el personaje se gira para mirar la dirección del movimiento")]
    [Range(0.0f, 0.3f)]
    [SerializeField] float RotationSmoothTime = 0.12f;

    [Space(10)]
    [Tooltip("La altura a la que puede saltar el jugador")]
    [SerializeField] float JumpHeight = 1.2f;

    [Tooltip("El personaje usa su propio valor de gravedad. El valor predeterminado del motor es -9.81f")]
    [SerializeField] float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Tiempo necesario para poder saltar de nuevo. Establézcalo en 0f para volver a saltar instantáneamente")]
    [SerializeField] float JumpTimeout = 0.50f;

    [Tooltip("Tiempo necesario para pasar antes de entrar en el estado de caida. Útil para bajar escaleras")]
    [SerializeField] float FallTimeout = 0.15f;

    ////////////////////////////////

    [Header("Player Grounded")]
    [Tooltip("Si el personaje está en tierra o no. No forma parte de la comprobación conectada a tierra integrada de CharacterController")]
    [SerializeField] bool Grounded = true;

    [Tooltip("Útil para terrenos accidentados")]
    [SerializeField] float GroundedOffset = -0.14f;

    [Tooltip("El radio de la comprobación puesta a tierra. Debe coincidir con el radio de CharacterController")]
    [SerializeField] float GroundedRadius = 0.4f;

    [Tooltip("Qué capas usa el personaje como suelo")]
    [SerializeField] LayerMask GroundLayers;

    ////////////////////////////////////


    private CharacterController _characterController;
    private Animator _animator;

    private Vector2 moveValue;

    private bool sprint;
    public bool canMove = true;
    public bool canAttack = true;

    private float _speed;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = -53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    //Input
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    Camera mainCamera;
    [SerializeField] Transform targetCamera;


    private void Start()
    {
        if (!IsOwner) return;

        //Application.targetFrameRate = 60;

        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        jumpAction = InputSystem.actions.FindAction("Jump");

        sprintAction.started += SprintAction_started;

        FindFirstObjectByType<CinemachineCamera>().Target.TrackingTarget = transform;
        mainCamera = Camera.main;
    }

    private void SprintAction_started(InputAction.CallbackContext obj)
    {
        sprint = !sprint;
    }

    private void Update()
    {
        if (!IsOwner) return;

        GroundedCheck();
        JumpAndGravity();
        Move();
    }

    private void Move()
    {
        moveValue = moveAction.ReadValue<Vector2>();

        if(!canMove) moveValue = Vector2.zero;

        // Establezca la velocidad objetivo en función de si se presiona Sprint
        float targetSpeed = sprint ? SprintSpeed : walkSpeed;

        // Si no hay entrada, establezca la velocidad objetivo en 0
        if (moveValue == Vector2.zero)
        {
            targetSpeed = 0.0f;
        }

        //Guarda la magnitud de la velocidad del CharacterController.
        float currentHorizontalSpeed = new Vector3(_characterController.velocity.x, 0.0f, _characterController.velocity.z).magnitude;
        
        float speedOffset = 0.1f;
        float inputMagnitude = moveValue.magnitude < 1 ? moveValue.magnitude : 1f;

        // Acelerar o desacelerar hasta la velocidad objetivo.
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * (Grounded ? SpeedChangeRate : 1));

            // Redondear la velocidad a 3 decimales
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        Vector3 moveDir = new Vector3(moveValue.x, 0, moveValue.y);

        // Si hay una entrada de movimiento, gire al jugador cuando el jugador se esté moviendo
        if (moveValue != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg +
                              mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // Girar a la dirección de entrada de la cara en relación con la posición de la cámara
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // Mover el jugador
        if (_characterController.enabled)
        {
            _characterController.Move(targetDirection * (_speed * Time.deltaTime) +
                         new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        // Actualizar Animator si se usa el personaje
        _animator.SetFloat("Speed", _speed);
    }

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // Restablecer el temporizador de tiempo de espera de caída
            _fallTimeoutDelta = FallTimeout;

            // Actualizar Animator si se usa el personaje
            _animator.SetBool("Jump", false);
            _animator.SetBool("FreeFall", false);

            // Detener nuestra velocidad cayendo infinitamente cuando estamos conectados a tierra
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Saltar
            if (jumpAction.IsPressed() && _jumpTimeoutDelta <= 0.0f)
            {
                // Actualizar Animator si se usa el personaje
                //_animator.SetBool("Jump", true);
            }

            // Tiempo de espera de salto
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // Restablecer el temporizador de tiempo de espera de salto
            _jumpTimeoutDelta = JumpTimeout;

            // Tiempo de espera de caida
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                _animator.SetBool("FreeFall", true);
            }
        }

        // Aplique la gravedad a lo largo del tiempo si está por debajo del terminal (multiplique por el tiempo delta dos veces para acelerar linealmente con el tiempo)
        if (_verticalVelocity > _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    

    private void GroundedCheck()
    {
        // Establecer la posición de la esfera, con  compensacion
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        _animator.SetBool("Grounded", Grounded);
    }

    private void OnJump(AnimationEvent animationEvent)
    {
        // la raíz cuadrada de H * -2 * G = cuánta velocidad se necesita para alcanzar la altura deseada
        _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        Debug.Log("OnJump");
    }

    private void OnFreeFall(AnimationEvent animationEvent)
    {
        canMove = false;
    }
    private void OnLand(AnimationEvent animationEvent)
    {
        canMove = true;
    }
    private void OnFinishAttack(AnimationEvent animationEvent)
    {
        canMove = true;
        canAttack = true;
    }
}

