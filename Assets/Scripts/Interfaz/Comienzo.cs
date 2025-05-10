using UnityEngine;
using UnityEngine.SceneManagement;

//Script que controla la escena de duelos
public class Comienzo : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject panelPopup;
    [SerializeField] private string Nivel;

    void Start()
    {
        Time.timeScale = 1f;
        if (Nivel.Equals("Gigante"))
        {
            panelPopup.SetActive(true);
        }
    }

    // Cargar la escena del juego
    public void Play()
    {
        switch (Nivel)
        {
            case "Gigante":
                SceneManager.LoadScene("Primer Duelo");
                break;
            case "Mago":
                SceneManager.LoadScene("Segundo Duelo");
                break;
            case "Rey":
                SceneManager.LoadScene("Tercer Duelo");
                break;
            case "Final":
                SceneManager.LoadScene("Creditos");
                break;
            default:
                break;
        }
    }
    public void CerrarPanel()
    {
        panelPopup.SetActive(false);
    }

}

