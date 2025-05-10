using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 8f;
    [SerializeField] private float minFlipThreshold = 0.1f;
    [SerializeField] private float attackDamping = 150f; // Resistencia al atacar

    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.55f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float frontalDamage = 7f;
    [SerializeField] private float slashDamage = 10f;

    [Header("Cooldown Settings")]
    [SerializeField] private float frontalCooldown = 0.7f;
    [SerializeField] private float slashCooldown = 0.7f;
    [SerializeField] private float dashCooldown = 0.7f;


    [Header("Audio Settings")]
    [SerializeField] private AudioClip frontalMissSound;
    [SerializeField] private AudioClip frontalHitSound;
    [SerializeField] private AudioClip slashMissSound;
    [SerializeField] private AudioClip slashHitSound;

    [Header("Dash Settings")]
    [SerializeField] private float dashDamping = 0f; // Resistencia durante el dash
    [SerializeField] private float normalDamping = 35f; // Resistencia normal


    //utils
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;

    //local movement
    private Vector2 movementDirection;
    private bool isFrontalOnCooldown = false;
    private bool isSlashOnCooldown = false;
    private bool isDashOnCooldown = false;

    public bool isDashing = false;

    //flip sprit
    private static readonly Vector3 FACE_RIGHT = new Vector3(2.7f, 2.537461f, 1);
    private static readonly Vector3 FACE_LEFT = new Vector3(-2.7f, 2.537461f, 1);

    //Nombre Animacion
    private readonly string isRunning = "isRunning";
    private readonly string dash = "Dash";
    private readonly string frontal = "Frontal";
    private readonly string slash = "Slash";

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = normalDamping;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        HandleMovementInput();
        HandleAttackInput();
        UpdateAnimations();
        HandleDash();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovementInput()
    {
        if (isDashing)
        {
            return;
        }
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        movementDirection = new Vector2(horizontal, vertical).normalized;
    }

    private void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0) && !isFrontalOnCooldown && !animator.GetBool(dash))
        {
            StartCoroutine(PerformAttack(frontal, frontalCooldown));
        }

        if (Input.GetMouseButtonDown(1) && !isSlashOnCooldown && !animator.GetBool(dash))
        {
            StartCoroutine(PerformAttack(slash, slashCooldown));
        }
    }

    private void UpdateAnimations()
    {
        float horizontalVelocity = rb.linearVelocity.x;
        float verticalVelocity = rb.linearVelocity.y;

        bool isRunning;
        if (Mathf.Abs(horizontalVelocity) > minFlipThreshold || Mathf.Abs(verticalVelocity) > minFlipThreshold) isRunning= true;
        else isRunning= false;

        animator.SetBool(this.isRunning, isRunning);

        if (horizontalVelocity > minFlipThreshold)
        {
            transform.localScale = FACE_RIGHT;
        }
        else if (horizontalVelocity < -minFlipThreshold)
        {
            transform.localScale = FACE_LEFT;
        }
    }

    private void HandleMovement()
    {
        rb.linearVelocity = movementDirection * movementSpeed;
    }

    private void HandleDash()
    {
        animator.SetBool(dash, isDashing);

        if (Input.GetKeyDown(KeyCode.Space) && !isDashOnCooldown && movementDirection != Vector2.zero && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }
    private IEnumerator PerformDash()
    {
        animator.SetBool(frontal, false);
        animator.SetBool(slash, false);

        // Aplicamos física del dash
        rb.linearDamping = dashDamping; // Disminuimos la resistencia para ganar velocidad
        isDashing = true;
        //Empezamos cooldown del dash
        isDashOnCooldown = true;
        yield return new WaitForSeconds(dashCooldown);
        isDashOnCooldown = false;
    }

    // Called from animation events
    public void endDash()
    {
        isDashing = false;
        rb.linearDamping = normalDamping;

    }


    private IEnumerator PerformAttack(string attackType, float cooldown)
    {
        rb.linearDamping = attackDamping;
        animator.SetBool(attackType, true);

        //Set cooldown
        if (attackType == frontal) {
            isFrontalOnCooldown = true;
            yield return new WaitForSeconds(cooldown);
            isFrontalOnCooldown = false;
        }
        else
        {
            isSlashOnCooldown = true;
            yield return new WaitForSeconds(cooldown);
            isSlashOnCooldown = false;
        }
    }

    // Called from animation events
    public void ExecuteFrontalAttack()
    {
        ExecuteAttack(frontalDamage, frontalMissSound, frontalHitSound);
    }
    public void ExecuteSlashAttack()
    {
        ExecuteAttack(slashDamage, slashMissSound, slashHitSound);
    }

    private void ExecuteAttack(float damage, AudioClip missSound, AudioClip hitSound)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);

        if (hitEnemies.Length == 0)
        {
            audioSource.PlayOneShot(missSound);
            return;
        }

        foreach (Collider2D enemy in hitEnemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            audioSource.PlayOneShot(hitSound);
            if (enemyHealth != null)
            {
                enemyHealth.currentHealth -= damage;
            }
        }
    }

    // Called from animation event
    void endFrontal()
    {
        rb.linearDamping = normalDamping;
        animator.SetBool(frontal, false);
    }
    void endSlash()
    {
        rb.linearDamping = normalDamping;
        animator.SetBool(slash, false);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}