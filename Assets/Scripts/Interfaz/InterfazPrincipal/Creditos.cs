using UnityEngine;
using UnityEngine.SceneManagement;

//Script que controla la escena Crédios
public class Creditos : MonoBehaviour
{
    // Volver a la página principal
    public void GoBack(){
        SceneManager.LoadScene("Principal");
    }
}
