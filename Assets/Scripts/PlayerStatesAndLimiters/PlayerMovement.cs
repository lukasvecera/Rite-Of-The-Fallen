using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public Animator animator;

    [Header("Fall injury")]
    public float fallInjuryTimeThreshold = 2f;
    public float sprintLockDuration = 3f;

    [Header("Injury UI")]
    public GameObject injuryTextRoot;
    public TextMeshProUGUI injuryText;
    public string injuryMessage = "You're injured! You can't sprint.";
    public float injuryInfoDuration = 3f;

    public enum GroundType
{
    Dirt,
    Concrete,
    Cathedral
}

    [Header("Footsteps")]
    public LayerMask groundMask;
    public float groundCheckDistance = 2f;

    GroundType currentGround;

    float injuryInfoTimer = 0f;

    float fallTimer = 0f;
    bool wasGrounded = true;

    bool sprintLocked = false;
    float sprintLockTimer = 0f;

    [Header("Move")]
    public float walkSpeed = 6f;
    public float runSpeed = 10f;
    public float jumpPower = 7f;
    public float gravity = 10f;

    [Header("Look")]
    public float lookSpeed = 2f;
    public float lookXLimit = 85f;

    [Header("Crouch")]
    public KeyCode crouchKey = KeyCode.R;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    [Header("Collider tuning")]
    public float centerYOffset = 0f;

    [Header("Stamina / Sprint")]
    public KeyCode runKey = KeyCode.LeftShift;
    public float maxStamina = 5f;
    public float staminaDrainPerSecond = 1f;
    public float staminaRegenPerSecond = 0.5f;

    [Header("Stamina UI")]
    public GameObject staminaUIRoot;
    public Slider staminaSlider;

    CharacterController cc;
    float yaw;
    float pitch;
    float verticalVelocity;
    bool canMove = true;

    float currentStamina;
    bool isRunningThisFrame;

    void Start()
    {
        cc = GetComponent<CharacterController>();

            if (animator == null)
        animator = GetComponentInChildren<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;
        pitch = 0f;
        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.identity;

        cc.height = defaultHeight;
        cc.center = new Vector3(0f, defaultHeight * 0.5f + centerYOffset, 0f);

        currentStamina = maxStamina;

        if (staminaSlider != null)
        {
            staminaSlider.minValue = 0f;
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }

        UpdateStaminaUI();
    }

    void Update()
    {
        Look();
        Move();
        HandleCrouch();
        UpdateAnimator();
        UpdateStaminaUI();
    }


    void Look()
    {
        if (!canMove) return;

        float mx = Input.GetAxis("Mouse X") * lookSpeed;
        float my = Input.GetAxis("Mouse Y") * lookSpeed;

        yaw   += mx;
        pitch -= my;
        pitch  = Mathf.Clamp(pitch, -lookXLimit, lookXLimit);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        if (playerCamera) playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

void Move()
{
    bool groundedNow = cc.isGrounded;

    float x = canMove ? Input.GetAxisRaw("Horizontal") : 0f;
    float z = canMove ? Input.GetAxisRaw("Vertical")   : 0f;

    Vector3 f = transform.forward; f.y = 0f; f.Normalize();
    Vector3 r = transform.right;   r.y = 0f; r.Normalize();
    Vector3 planar = (r * x + f * z).normalized;

    bool sprintInput   = canMove &&
                         Input.GetKey(runKey) &&
                         planar.sqrMagnitude > 0.01f &&
                         !sprintLocked;

    bool canSprint = sprintInput && currentStamina > 0.01f;

    if (canSprint)
    {
        currentStamina -= staminaDrainPerSecond * Time.deltaTime;
    }
    else
    {
        currentStamina += staminaRegenPerSecond * Time.deltaTime;
    }

    currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

    if (currentStamina <= 0.01f)
        canSprint = false;

    isRunningThisFrame = canSprint;

    float speed = isRunningThisFrame ? runSpeed : walkSpeed;

    if (cc.isGrounded)
    {
        verticalVelocity = -2f;
        if (Input.GetButtonDown("Jump") && canMove)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.jumpSound, 0.3f);
            verticalVelocity = jumpPower;

            if (animator != null)
            animator.SetTrigger("Jump");
        }

    }
    else
    {
        verticalVelocity -= gravity * Time.deltaTime;
    }

    Vector3 velocity = planar * speed + Vector3.up * verticalVelocity;
    cc.Move(velocity * Time.deltaTime);

    bool groundedAfterMove = cc.isGrounded;

    if (!groundedAfterMove && verticalVelocity < 0f)
    {
        fallTimer += Time.deltaTime;
    }
    else
    {
        if (!wasGrounded && groundedAfterMove)
        {
            if (fallTimer >= fallInjuryTimeThreshold)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.fallGrunt, 0.7f);
                sprintLocked = true;
                sprintLockTimer = sprintLockDuration;

                ShowInjuryInfo();
            }
        }

        fallTimer = 0f;
    }

    if (sprintLocked)
    {
        sprintLockTimer -= Time.deltaTime;
        if (sprintLockTimer <= 0f)
        {
            sprintLocked = false;
            sprintLockTimer = 0f;
        }
    }

    wasGrounded = groundedAfterMove;

    if (injuryInfoTimer > 0f)
    {
        injuryInfoTimer -= Time.deltaTime;

        if (injuryInfoTimer <= 0f || !sprintLocked)
        {
            HideInjuryInfo();
        }
    }
    else
    {
        if (!sprintLocked)
            HideInjuryInfo();
    }

        UpdateGroundType();

    Vector3 horizontal = cc.velocity;
    horizontal.y = 0f;
    bool isMovingHorizontally = horizontal.magnitude > 0.1f;

    UpdateFootstepSounds(isMovingHorizontally && cc.isGrounded);
}

    void HandleCrouch()
    {
        if (Input.GetKey(crouchKey) && canMove)
        {
            if (Mathf.Abs(cc.height - crouchHeight) > 0.001f)
            {
                cc.height = crouchHeight;
                cc.center = new Vector3(0f, crouchHeight * 0.5f + centerYOffset, 0f);
            }
            walkSpeed = crouchSpeed;
            runSpeed  = crouchSpeed;
        }
        else
        {
            if (Mathf.Abs(cc.height - defaultHeight) > 0.001f)
            {
                cc.height = defaultHeight;
                cc.center = new Vector3(0f, defaultHeight * 0.5f + centerYOffset, 0f);
            }
            walkSpeed = 6f;
            runSpeed  = 10f;
        }
    }

    void UpdateStamina(bool isRunning)
    {
        if (isRunning)
        {
            currentStamina -= staminaDrainPerSecond * Time.unscaledDeltaTime;
        }
        else
        {
            currentStamina += staminaRegenPerSecond * Time.unscaledDeltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    void UpdateStaminaUI()
    {
        if (staminaSlider != null)
            staminaSlider.value = currentStamina;

        if (staminaUIRoot != null)
        {
            bool full = Mathf.Approximately(currentStamina, maxStamina);
            staminaUIRoot.SetActive(!full);
        }
    }

    public void ResetStaminaToMax()
{
    currentStamina = maxStamina;

    if (staminaSlider != null)
        staminaSlider.value = currentStamina;

    if (staminaUIRoot != null)
        staminaUIRoot.SetActive(false);

    sprintLocked = false;
    sprintLockTimer = 0f;
    fallTimer = 0f;
    wasGrounded = true;

    HideInjuryInfo();
}

void ShowInjuryInfo()
{
    injuryInfoTimer = injuryInfoDuration;

    if (injuryText != null)
        injuryText.text = injuryMessage;

    if (injuryTextRoot != null)
        injuryTextRoot.SetActive(true);
}

void HideInjuryInfo()
{
    injuryInfoTimer = 0f;

    if (injuryTextRoot != null)
        injuryTextRoot.SetActive(false);
}


void UpdateAnimator()
{
    if (animator == null) return;

    Vector3 horizontal = cc.velocity;
    horizontal.y = 0f;
    float speed = horizontal.magnitude;

    bool isMoving = speed > 0.1f;

    animator.SetBool("IsMoving", isMoving);
    animator.SetBool("IsRunning", isMoving && isRunningThisFrame);
    animator.SetBool("IsGrounded", cc.isGrounded);
}

void UpdateGroundType()
{
    Vector3 origin = transform.position + Vector3.up * 0.1f;
    Ray ray = new Ray(origin, Vector3.down);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))

    {
        if (hit.collider.CompareTag("GroundDirt"))
            currentGround = GroundType.Dirt;
        else if (hit.collider.CompareTag("GroundConcrete"))
            currentGround = GroundType.Concrete;
        else if (hit.collider.CompareTag("GroundCathedral"))
            currentGround = GroundType.Cathedral;
    }
}

void UpdateFootstepSounds(bool shouldPlay)
{
    if (AudioManager.Instance == null)
        return;

    if (!shouldPlay)
    {
        AudioManager.Instance.StopWalking();
        return;
    }

    AudioClip clip = null;

    switch (currentGround)
    {
        case GroundType.Dirt:
            clip = isRunningThisFrame
                ? AudioManager.Instance.runningOnDirtGravel
                : AudioManager.Instance.walkingOnDirtGravel;
            break;

        case GroundType.Concrete:
            clip = isRunningThisFrame
                ? AudioManager.Instance.runningOnConcrete
                : AudioManager.Instance.walkingOnConcrete;
            break;

        case GroundType.Cathedral:
            clip = isRunningThisFrame
                ? AudioManager.Instance.runningCathedral
                : AudioManager.Instance.walkingCathedral;
            break;
    }

    if (clip != null)
        AudioManager.Instance.PlayWalking(clip);
}


}
