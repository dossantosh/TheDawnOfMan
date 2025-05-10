using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource source;
    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        source.Play();
    }
}
