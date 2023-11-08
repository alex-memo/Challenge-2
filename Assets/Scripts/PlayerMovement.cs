using UnityEngine;
/// <summary>
/// @alex-memo 2023
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private const float speed = 3f;

    private const float turnSmoothTime = .2f;
    private float turnSmoothVelocity;

    private const float jumpHeight = 1.5f;

    private Animator anim;

    private Transform cam;

    private Vector3 moveVelocity;

    private const float groundDistance = .2f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    private bool isGrounded => Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        anim = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        if (isGrounded && moveVelocity.y < 0)
        {
            moveVelocity.y = -2f;
        }
        movePlayer();
    }
    private void movePlayer()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 _dir = new Vector3(horizontal, 0f, vertical).normalized;

        if (isGrounded)
        {
            if (_dir == Vector3.zero)
            {
                idle();
            }
        }

        if (_dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(_dir.x, _dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 movedir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            if (isGrounded && movedir != Vector3.zero)
            {
                walk();
            }

            controller.Move(speed * Time.deltaTime * movedir.normalized);

        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            moveVelocity.y = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
            triggerAnim("Jump");
        }

        moveVelocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(moveVelocity * Time.deltaTime);
    }

    private void walk()
    {
        if (controller.isGrounded)
        {
            animate(1f);
        }
    }
    private void idle()
    {
        if (controller.isGrounded)
        {
            animate(0);
        }
    }

    private void animate(float _value, float _dampTime = .1f)
    {
        if (!hasAnimator()) { return; }
        try
        {
            anim.SetFloat("Speed", _value, _dampTime, Time.deltaTime);
        }
        catch { print("Error Trying to Set "+_value); }
    }
    private void triggerAnim(string _trigger)
    {
        if (!hasAnimator()) { return; }
        try
        {
            anim.SetTrigger(_trigger);
        }
        catch { print("Error Trying to Set " + _trigger); }
    }
    private bool hasAnimator()
    {
        if (anim != null) { return true; }
        Debug.LogWarning("There is no Animator Component!");
        return false;
    }
}
