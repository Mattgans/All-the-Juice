using Oculus.Haptics;
using UnityEngine;

public class ParticleAudioTrigger : MonoBehaviour
{
    public AudioSource audioSource;
    public ParticleSystem particles;
    public HapticSource haptics;

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

    public void PlayHaptic()
    {
        if (haptics != null)
        {
            haptics.Play();
        }
    }
}
