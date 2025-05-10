using UnityEngine;
using UnityEngine.SceneManagement;

public class Pausa : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject panelPausa;

    void Start()
    {
        Time.timeScale = 1f;  // Asegura que el tiempo est� normal al inicio
        panelPausa.SetActive(false);  // El panel de pausa est� oculto al principio
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
        Time.timeScale = 0f;  // Pausa la f�sica y las animaciones
    }

    public void ReanudarJuego()
    {
        panelPausa.SetActive(false);
        Time.timeScale = 1f;  // Reanuda el tiempo
    }

    public void Salir()
    {
        Time.timeScale = 1f;  // Asegura que el tiempo est� normal antes de salir
        SceneManager.LoadScene("Duelos");  // Cambia a la escena deseada
    }
}
