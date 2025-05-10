using UnityEngine;
using UnityEngine.SceneManagement;

public class Pausa : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject panelPausa;

    void Start()
    {
        Time.timeScale = 1f;  // Asegura que el tiempo esté normal al inicio
        panelPausa.SetActive(false);  // El panel de pausa está oculto al principio
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1f)
            {
                PausarJuego();
            }
            else
            {
                ReanudarJuego();
            }
        }
    }

    public void PausarJuego()
    {
        panelPausa.SetActive(true);
        Time.timeScale = 0f;  // Pausa la física y las animaciones
    }

    public void ReanudarJuego()
    {
        panelPausa.SetActive(false);
        Time.timeScale = 1f;  // Reanuda el tiempo
    }

    public void Salir()
    {
        Time.timeScale = 1f;  // Asegura que el tiempo esté normal antes de salir
        SceneManager.LoadScene("Duelos");  // Cambia a la escena deseada
    }
}
