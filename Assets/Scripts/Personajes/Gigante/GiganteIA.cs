using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(AudioSource))]
public class GiganteIA : MonoBehaviour
{ 
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float attackMovementSpeed;
    [SerializeField] private float chaseDistanceThreshold;
    [SerializeField] private float attackDistanceThreshold;
    [SerializeField] private float minFlipThreshold;

    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int attackFrontalDamage;
    [SerializeField] private int attackSlashDamage;

    [Header("Cooldown Settings")]
    [SerializeField] private float attackFrontalCooldown;
    [SerializeField] private float attackSlashCooldown;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip attackMissSound;
    [SerializeField] private AudioClip attackHitSound;

    //utils
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    //local variables
    private bool isFrontalOnCooldown = false;
    private bool isSlashOnCooldown = false;
    private float distanceToPlayer;

    //Nombre Animacion
    private string isRunning = "isRunning";
    private string frontal = "Frontal";
    private string slash = "Slash";


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

    private void Update()
    {
        if (player == null) return;

        float horizontalVelocity = rb.linearVelocity.x;
        float verticalVelocity = rb.linearVelocity.y;

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
        // Flip sprite basado en la posición del jugador
        spriteRenderer.flipX = player.position.x < transform.position.x;
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

        //si tengo cooldowns y está más cerca, ataco
        TryAttackPlayer();
        animator.SetBool(isRunning, false);

    }

    private void TryAttackPlayer()
    {
        if (!isFrontalOnCooldown && !animator.GetBool(slash))
        {
            StartCoroutine(PerformAttack(frontal, attackFrontalCooldown));
            return;
        }
        if (!isSlashOnCooldown && !animator.GetBool(frontal))
        {
            StartCoroutine(PerformAttack(slash, attackSlashCooldown));
            return;
        }
    }

    private void ChasePlayer()
    {
        //animator.SetBool(frontal, false);
        //animator.SetBool(slash, false);
        animator.SetBool(isRunning, true);

        Vector2 direction = (player.position - transform.position).normalized;
        if (!animator.GetBool(frontal) && !animator.GetBool(slash))
        {
            rb.MovePosition(rb.position + direction * movementSpeed * Time.fixedDeltaTime);
        } else
        {
            rb.MovePosition(rb.position + direction * attackMovementSpeed * Time.fixedDeltaTime);
        }
    }

    private void IdleBehavior()
    {
        animator.SetBool(isRunning, false);
    }

    private IEnumerator PerformAttack(string NombreAtaque, float Cooldown)
    {
        switch (NombreAtaque)
        {
            case "Frontal":
                isFrontalOnCooldown = true;
                animator.SetBool(NombreAtaque, true);

                yield return new WaitForSeconds(Cooldown);
                isFrontalOnCooldown = false;
                break;
            case "Slash":
                isSlashOnCooldown = true;
                animator.SetBool(NombreAtaque, true);

                yield return new WaitForSeconds(Cooldown);
                isSlashOnCooldown = false;
                break;
            default:
                break;
        }
    }

    // Método llamado desde la animación
    public void ExecuteFrontalAttack()
    {
        ExecuteAttack(attackFrontalDamage, attackMissSound, attackHitSound);
    }
    public void ExecuteSlashAttack()
    {
        ExecuteAttack(attackSlashDamage, attackMissSound, attackHitSound);
    }
    public void ExecuteAttack(float damage, AudioClip missSound, AudioClip hitSound)
    {
        Collider2D[] players = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayer);

        if (players.Length == 0)
        {
            audioSource.PlayOneShot(missSound);
            return;
        }

        foreach (Collider2D playerCollider in players)
        {
            Health playerHealth = playerCollider.GetComponent<Health>();
            if (playerHealth != null)
            {
                audioSource.PlayOneShot(hitSound);
                playerHealth.currentHealth -= damage;
            }
        }
    }

    // Método llamado desde la animación
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