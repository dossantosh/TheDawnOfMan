using UnityEngine;
using UnityEngine.SceneManagement;

//Script que controla la escena de duelos
public class Duelos : MonoBehaviour
{
    //Conecto la source del audio
    [SerializeField] private AudioSource source;

    // Cargar la escena del juego
    void Start()
    {
        Time.timeScale = 1f;
    }
    public void PlayGigante()
    {
        SceneManager.LoadScene("PrimerActo");
    }
    public void PlayMago()
    {
        SceneManager.LoadScene("SegundoActo");
    }
    public void PlayRey()
    {
        SceneManager.LoadScene("TercerActo");
    }

    // Ir al menú principal
    public void IrAlMenu()
    {
        SceneManager.LoadScene("Principal");
    }

}

