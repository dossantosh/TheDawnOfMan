using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManagerWinLose : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private string estado;

    [Header("Victoria")]
    [SerializeField] private AudioClip primeraVictoria;
    [SerializeField] private AudioClip segundaVictoria;
    [SerializeField] private AudioClip terceraVictoria;

    [Header("Derrota")]
    [SerializeField] private AudioClip primeraDerrota;
    [SerializeField] private AudioClip segundaDerrota;

    private int sonido;
    private AudioSource audioSource;

    public void Start()
    {
        switch (estado)
        {
            case "Victoria":
                sonido = UnityEngine.Random.Range(0, 3);

                switch (sonido)
                {
                    case 0:
                        audioSource.PlayOneShot(primeraVictoria);
                        Debug.Log(1);
                        break;
                    case 1:
                        audioSource.PlayOneShot(segundaVictoria);
                        Debug.Log(2);
                        break;
                    case 2:
                        audioSource.PlayOneShot(terceraVictoria);
                        Debug.Log(3);
                        break;
                    default: break;
                }
                break;

            case "Derrota":
                sonido = UnityEngine.Random.Range(0, 2);
                switch (sonido)
                {
                    case 0:
                        audioSource.PlayOneShot(primeraDerrota);
                        break;
                    case 1:
                        audioSource.PlayOneShot(segundaDerrota);
                        break;
                    default:
                        break;
                }
                break;

            default:
                break;
        }
    }
}
