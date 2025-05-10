using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth;
    [SerializeField] private string hit;
    [SerializeField] private string death;
    [SerializeField] private string personaje;
    [SerializeField] private string personajeEnemigo;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Image imagen;

    [Header("Health Bar")]
    [SerializeField] private Transform hpLine = null;
    [SerializeField] private Vector3 originalScale;

    //Utils
    private Rigidbody2D rb;
    private Animator animator;

    //Variables Locales
    public float health;
    public float currentHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        health = maxHealth;
        currentHealth = health;
        if (personaje.Equals("player"))
        {
            return;
        }
        originalScale = hpLine.localScale;
    }

    void Update()
    {
        Vida(hit);
        Muerte(death);

        if (personaje.Equals("player"))
        {
            return;
        }
        UpdateHealthBar();
    }

    private void Vida(string hit)
    {

        if (currentHealth == health)
        {
            return;
        }

        if (health <= 0)
        {
            return;
        }
        health = currentHealth;

        if (personaje.Equals("player"))
        {
            CambiarColorSegunVida();
            if (playerController.GetComponent<PlayerController>().isDashing)
            {
                return;
            }
        }

        if (hit.Length > 0)
        {
            animator.SetTrigger(hit);
        }
        Debug.Log(health);
    }
    private void Muerte(string death)
    {
        if (health > 0)
        {
            return;
        }
        animator.SetBool(death, true);

        if (personaje.Equals("player"))
        {
            SceneManager.LoadScene("PantallaLose");
            return;
        }

        if (personaje.Equals("rey"))
        {
            SceneManager.LoadScene("Final");
            return;
        }
        SceneManager.LoadScene("PantallaWin");
    }

    private void UpdateHealthBar()
    {
        float healthPercentage = (float)currentHealth / maxHealth;
        if (health > 0)
        {
            hpLine.localScale = new Vector3(originalScale.x * healthPercentage, originalScale.y, originalScale.z);
        } else
        {
            hpLine.localScale = new Vector3(originalScale.x * 0, originalScale.y, originalScale.z);
        }
    }
    private void CambiarColorSegunVida()
    {
        float healthPercentage = (float)currentHealth / maxHealth;
        if (healthPercentage > 0.8f) return;

        personajeEnemigo = personajeEnemigo.ToLower();
        Color colorFondo;
        Color color; // Mantener color actual por defecto

        // Asignar color de fondo y sprite según el tipo de enemigo
        switch (personajeEnemigo)
        {
            case "gigante":
                colorFondo = new Color(0.6f, 0f, 0f,
                    healthPercentage > 0.6f ? 0.4f :
                    healthPercentage > 0.4f ? 0.5f :
                    healthPercentage > 0.2f ? 0.6f : 0.7f);
                break;

            case "mago":
                colorFondo = new Color(0.6f, 0f, 0f,
                    healthPercentage > 0.6f ? 0.05f :
                    healthPercentage > 0.4f ? 0.1f :
                    healthPercentage > 0.2f ? 0.15f : 0.2f);
                break;

            case "rey":
                colorFondo = new Color(0.6f, 0f, 0f,
                    healthPercentage > 0.6f ? 0.4f :
                    healthPercentage > 0.4f ? 0.5f :
                    healthPercentage > 0.2f ? 0.6f : 0.7f);

                color =
                    healthPercentage > 0.6f ? new Color(0.24f, 0.24f, 0.24f) :
                    healthPercentage > 0.4f ? new Color(0.5f, 0.5f, 0.5f) :
                    healthPercentage > 0.2f ? new Color(0.74f, 0.74f, 0.74f) :
                    Color.white;

                spriteRenderer.color = color;
                break;

            default:
                colorFondo = Color.clear;
                break;
        }

        // Asignar color de fondo
        imagen.color = colorFondo;
    }

}
