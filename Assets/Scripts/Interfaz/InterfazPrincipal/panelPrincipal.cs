using UnityEngine;
using UnityEngine.SceneManagement;

//Script que controla la escena principal
public class panelPrincipal : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject panelPopup;

    void Start()
    {
        panelPopup.SetActive(false);
    }

    // Cargar la escena del juego
    public void PlayGame()
    {
        SceneManager.LoadScene("Duelos");
    }

    // Salir del juego
    public void QuitGame()
    {
        Application.Quit();
    }

    // Ir a créditos
    public void Credits(){
        SceneManager.LoadScene("Creditos");
    }

    // Se llama cuando se pulsa el botón
    public void MostrarPanel()
    {
        panelPopup.SetActive(true);
    }

    public void CerrarPanel()
    {
        panelPopup.SetActive(false);
    }

}
