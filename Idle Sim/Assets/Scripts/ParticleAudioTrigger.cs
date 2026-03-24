using UnityEngine;

public class ParticleAudioTrigger : MonoBehaviour
{
    public AudioSource audioSource;
    public ParticleSystem particles;

    public void PlayAudio()
    {
        if (audioSource != null)
            audioSource.Play();
    }

    public void PlayParticles()
    {
        if (particles != null)
        {
            particles.Play();
        }
    }
}
