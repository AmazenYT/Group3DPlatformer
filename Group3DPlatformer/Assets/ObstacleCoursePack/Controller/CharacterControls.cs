using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class CharacterControls : MonoBehaviour
{
    public float speed = 10.0f;
    public float airVelocity = 8f;
    public float gravity = 10.0f;
    public float maxVelocityChange = 10.0f;
    public float jumpHeight = 2.0f;
    public float maxFallSpeed = 20.0f;
    public float rotateSpeed = 25f;
    public GameObject cam;
    private Vector3 moveDir;
    private Rigidbody rb;

    private float distToGround;
    private bool canMove = true;
    private bool isStuned = false;
    private bool wasStuned = false;
    private float pushForce;
    private Vector3 pushDir;
    public Vector3 checkPoint;
    private bool slide = false;

    void Start()
    {
        distToGround = GetComponent<Collider>().bounds.extents.y;
        checkPoint = transform.position;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            if (moveDir.magnitude > 0)
            {
                Vector3 targetDir = moveDir;
                targetDir.y = 0;

                if (targetDir == Vector3.zero)
                    targetDir = transform.forward;

                Quaternion tr = Quaternion.LookRotation(targetDir);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, Time.deltaTime * rotateSpeed);
                transform.rotation = targetRotation;
            }

            Vector3 targetVelocity = moveDir * (IsGrounded() ? speed : airVelocity);
            Vector3 velocity = rb.velocity;

            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            if (IsGrounded())
            {
                if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
                    rb.AddForce(velocityChange, ForceMode.VelocityChange);

                if (Input.GetButton("Jump"))
                {
                    rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                }
            }
            else if (!slide && Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
            {
                rb.AddForce(velocityChange, ForceMode.VelocityChange);
            }
        }
        else
        {
            rb.velocity = pushDir * pushForce;
        }

        rb.AddForce(new Vector3(0, -gravity * rb.mass, 0));
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 v2 = v * cam.transform.forward;
        Vector3 h2 = h * cam.transform.right;
        moveDir = (v2 + h2).normalized;

        if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, distToGround + 0.1f))
        {
            slide = hit.transform.CompareTag("Slide");
        }

        // Reset player if they fall below y = -5
        if (transform.position.y < -5f)
        {
            LoadCheckPoint();
            rb.velocity = Vector3.zero;
        }
    }

    float CalculateJumpVerticalSpeed()
    {
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    public void HitPlayer(Vector3 velocityF, float time)
    {
        rb.velocity = velocityF;
        pushForce = velocityF.magnitude;
        pushDir = Vector3.Normalize(velocityF);
        StartCoroutine(Decrease(velocityF.magnitude, time));
    }

    public void LoadCheckPoint()
    {
        transform.position = checkPoint;
    }

    private IEnumerator Decrease(float value, float duration)
    {
        if (isStuned)
            wasStuned = true;

        isStuned = true;
        canMove = false;

        float delta = value / duration;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            yield return null;

            if (!slide)
            {
                pushForce -= Time.deltaTime * delta;
                pushForce = Mathf.Max(0, pushForce);
            }
        }

        if (wasStuned)
        {
            wasStuned = false;
        }
        else
        {
            isStuned = false;
            canMove = true;
        }
    }
}
