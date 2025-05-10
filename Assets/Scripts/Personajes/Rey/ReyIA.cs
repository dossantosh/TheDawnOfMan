using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(AudioSource))]
public class ReyIA : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float attackMovementSpeed;
    [SerializeField] private float comboMovementSpeed;

    [SerializeField] private float chaseDistanceThreshold;
    [SerializeField] private float attackDistanceThreshold;
    [SerializeField] private float minFlipThreshold;

    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int attackDamage;

    [Header("Cooldown Settings")]
    [SerializeField] private float attackFrontalCooldown;
    [SerializeField] private float attackSlashCooldown;
    [SerializeField] private float attackDashCooldown;
    [SerializeField] private float attackComboCooldown;

    [SerializeField] private float betweenAttackCooldown;

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
    private float distanceToPlayer;
    private bool isFrontalOnCooldown = false;
    private bool isSlashOnCooldown = false;
    private bool isDashOnCooldown = false;
    private bool isComboOnCooldown = false;
    //private bool isOnCooldown = false;

    private bool isFrontalActive = false;
    private bool isSlashActive = false;
    private bool isDashActive = false;
    private bool isComboActive = false;
    private bool isAttacking = false;

    //Nombre Animacion
    private readonly string isRunning = "isRunning";
    private readonly string frontal = "Frontal";
    private readonly string slash = "Slash";
    private readonly string dash = "Dash";
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

    private void Update()
    {
        if (player == null) return;

        Variables();

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

        if (attackDistanceThreshold >= distanceToPlayer)
        {
            TryAttackPlayer();
        }
    }

    private void TryAttackPlayer()
    {
        if (isAttacking)
        {
            return;
        }
        if (!isSlashOnCooldown)
        {
            StartCoroutine(PerformAttack(slash));
            return;
        }
        if (!isDashOnCooldown)
        {
            StartCoroutine(PerformAttack(dash));
            return;
        }
        if (!isFrontalOnCooldown)
        {
            StartCoroutine(PerformAttack(frontal));
            return;
        }
        if (!isComboOnCooldown)
        {
            StartCoroutine(PerformAttack(combo));
            return;
        }
    }

    private void ChasePlayer()
    {

        Vector2 direction = (player.position - transform.position).normalized;

        if (!isAttacking)
        {
            rb.MovePosition(rb.position + movementSpeed * Time.fixedDeltaTime * direction); //si no atacas, te mueves normal
            animator.SetBool(isRunning, true);
            return;
        }
        if (isComboActive)
        {
            rb.MovePosition(rb.position + comboMovementSpeed * Time.fixedDeltaTime * direction);
            return;
        }

        rb.MovePosition(rb.position + attackMovementSpeed * Time.fixedDeltaTime * direction); //si atacas. te mueves mas lento

    }

    private void IdleBehavior()
    {
        animator.SetBool(isRunning, false);
    }

    private IEnumerator PerformAttack(string NombreAtaque)
    {
        switch (NombreAtaque)
        {
            case "Frontal":

                isFrontalOnCooldown = true;

                isFrontalActive = true;
                animator.SetBool(NombreAtaque, true);
                Debug.Log("Frontal");
                yield return new WaitForSeconds(betweenAttackCooldown);
                isFrontalActive = false;

                yield return new WaitForSeconds(attackFrontalCooldown);
                isFrontalOnCooldown = false;
                break;
            case "Slash":
                isSlashOnCooldown = true;

                isSlashActive = true;
                animator.SetBool(NombreAtaque, true);
                Debug.Log("Slash");
                yield return new WaitForSeconds(betweenAttackCooldown);
                isSlashActive = false;

                yield return new WaitForSeconds(attackSlashCooldown);
                isSlashOnCooldown = false;
                break;
            case "Dash":
                isDashOnCooldown = true;

                isDashActive = true;
                animator.SetBool(NombreAtaque, true);
                Debug.Log("Dash");
                yield return new WaitForSeconds(betweenAttackCooldown);
                isDashActive = false;

                yield return new WaitForSeconds(attackDashCooldown);
                isDashOnCooldown = false;
                break;
            case "Combo":
                isComboActive = true;
                animator.SetBool(combo, true);
                Debug.Log("Combo");
                yield return new WaitForSeconds(betweenAttackCooldown);
                yield return new WaitForSeconds(betweenAttackCooldown);
                yield return new WaitForSeconds(betweenAttackCooldown);
                isComboActive = false;

                isComboOnCooldown = true;
                yield return new WaitForSeconds(attackComboCooldown);
                isComboOnCooldown = false;
                break;
            default:
                break;
        }
    }

    // Método llamado desde la animación
    public void ExecuteFrontalAttack()
    {
        ExecuteAttack(attackDamage, attackMissSound, attackHitSound);
    }
    public void ExecuteSlashAttack()
    {
        ExecuteAttack(attackDamage, attackMissSound, attackHitSound);
    }
    public void ExecuteDashAttack()
    {
        ExecuteAttack(attackDamage, attackMissSound, attackHitSound);
    }
    public void ExecuteAttack(float damage, AudioClip missSound, AudioClip hitSound)
    {
        Collider2D[] players = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayer);

        if (players.Length == 0)
        {
            audioSource.PlayOneShot(attackMissSound);
            return;
        }

        foreach (Collider2D playerCollider in players)
        {
            Health playerHealth = playerCollider.GetComponent<Health>();
            if (playerHealth != null)
            {
                audioSource.PlayOneShot(attackHitSound);
                //heroGameObject.GetComponent<PlayerHealth>().currenthealth -= 7;
                //playerHealth.TakeDamage(attackDamage);
                playerHealth.currentHealth -= attackDamage;
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
    public void EndDashAnimation()
    {
        animator.SetBool(dash, false);
    }
    public void EndComboAnimation()
    {
        animator.SetBool(combo, false);
    }

    private void Variables()
    {
        isAttackingMethod();
        //isOnCooldownMethod();
    }
    private void isAttackingMethod()
    {
        if (isFrontalActive || isSlashActive || isDashActive || isComboActive)
        {
            isAttacking = true;
            return;
        }
        isAttacking = false;
    }

    /*
    private void isOnCooldownMethod()
    {
        if (isFrontalOnCooldown && isSlashOnCooldown && isDashOnCooldown && isComboOnCooldown)
        {
            isOnCooldown = true;
            return;
        }
        isOnCooldown = false;
    }
    */
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}