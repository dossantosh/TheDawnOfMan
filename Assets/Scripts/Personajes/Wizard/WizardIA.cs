using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(AudioSource))]
public class WizardIA : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float attackMovementSpeed;
    [SerializeField] private float runAwayMovementSpeed;

    [SerializeField] private float chaseDistanceThreshold;
    [SerializeField] private float attackDistanceThreshold;
    [SerializeField] private float minFlipThreshold;

    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int attackDamage;

    [Header("Cooldown Settings")]
    [SerializeField] private float attackComboCooldown;
    [SerializeField] private float betweenAttackCooldown;
    [SerializeField] private float damageCooldown;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip attackHitSound;
    [SerializeField] private AudioClip runAwaySound;

    //utils
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    //local variables
    private float distanceToPlayer;
    private bool isComboOnCooldown = false;
    private bool isComboActive = false;
    private bool isDamageOnCooldown = false;
    private bool soundplayed = false;

    //Nombre Animacion
    private readonly string isRunning = "isRunning";
    private readonly string frontal = "Frontal";
    private readonly string slash = "Slash";
    private readonly string combo = "Combo";


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        // Buscar al jugador si no está asignado
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }
    private void Start()
    {
        soundplayed = false;
    }

    private void Update()
    {
        if (player == null) return;

        UpdateDistanceToPlayer();
        UpdateSpriteFlip();
        UpdateEnemyBehavior();
    }


    private void UpdateDistanceToPlayer()
    {
        distanceToPlayer = Vector2.Distance(player.position, transform.position);
    }

    private void UpdateSpriteFlip()
    {
        spriteRenderer.flipX = player.position.x < transform.position.x; // Flip sprite basado en la posición del jugador
    }

    private void UpdateEnemyBehavior()
    {
        if (distanceToPlayer > chaseDistanceThreshold) //si está lejos, me quedo quieto
        {
            IdleBehavior();
            return;
        }

        if (distanceToPlayer > attackDistanceThreshold) //si se acerca, le persigo
        {
            ChasePlayer();
            return;
        }

        if (isComboOnCooldown && !isComboActive)// si estoy en cooldowns, huyo
        {
            ChasePlayer();
            return;
        }

        //si tengo cooldowns y está más cerca, ataco
        TryAttackPlayer();
    }

    private void TryAttackPlayer()
    {
        
        if (!isComboOnCooldown)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + attackMovementSpeed * Time.fixedDeltaTime * direction); //si atacas. te mueves mas rapido

            StartCoroutine(PerformAttack(combo));
        }
    }

    private void ChasePlayer()
    {

        Vector2 direction = (player.position - transform.position).normalized;

        if (isComboOnCooldown && !isComboActive) // si está en cooldown y no está atacando, huye
        {
            animator.SetBool(isRunning, true);

            if (!soundplayed)
            {
                audioSource.PlayOneShot(runAwaySound); // sonido de huida
                soundplayed = true;
            }

            spriteRenderer.flipX = player.position.x > transform.position.x; // invertir sprite

            direction = (transform.position - player.position).normalized; // dirección de huida

            rb.MovePosition(rb.position + runAwayMovementSpeed * Time.fixedDeltaTime * direction);
            return;
        }

        if (isComboActive)
        {
            rb.MovePosition(rb.position + attackMovementSpeed * Time.fixedDeltaTime * direction); //si atacas te mueves mas rapido
            return;
        }

        animator.SetBool(isRunning, true);
        rb.MovePosition(rb.position + movementSpeed * Time.fixedDeltaTime * direction); //si no atacas, te mueves normal

    }
    private void IdleBehavior()
    {
        animator.SetBool(isRunning, false);
    }

    private IEnumerator PerformAttack(string NombreAtaque)
    {
        switch (NombreAtaque)
        {
            case "Combo":
                isComboOnCooldown = true;

                isComboActive = true;

                animator.SetBool(frontal, true);
                audioSource.PlayOneShot(attackSound);
                yield return new WaitForSeconds(betweenAttackCooldown);

                animator.SetBool(slash, true);
                audioSource.PlayOneShot(attackSound);
                yield return new WaitForSeconds(betweenAttackCooldown);

                animator.SetBool(frontal, true);
                audioSource.PlayOneShot(attackSound);
                yield return new WaitForSeconds(betweenAttackCooldown);

                animator.SetBool(slash, true);
                audioSource.PlayOneShot(attackSound);
                yield return new WaitForSeconds(betweenAttackCooldown);

                isComboActive = false;

                yield return new WaitForSeconds(attackComboCooldown);
                isComboOnCooldown = false;
                break;
            default:
                break;
        }
    }

    // Métodos llamados desde la animación
    public void ExecuteAttack()
    {
        if (isDamageOnCooldown)
        {
            return;
        }

        Collider2D[] players = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayer);
        if (players.Length == 0)
        {
            return;
        }
        foreach (Collider2D playerCollider in players)
        {
            Health playerHealth = playerCollider.GetComponent<Health>();
            if (playerHealth != null)
            {
                audioSource.PlayOneShot(attackHitSound);
                playerHealth.currentHealth -= attackDamage;
                StartCoroutine(IsDamageOnCooldownMethod());
            }
        }
    }
    public IEnumerator IsDamageOnCooldownMethod()
    {
        isDamageOnCooldown = true;
        yield return new WaitForSeconds(damageCooldown);
        isDamageOnCooldown = false;
    }
    public void EndFrontalAnimation()
    {
        animator.SetBool(frontal, false);
    }
    public void EndSlashAnimation()
    {
        animator.SetBool(slash, false);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}