using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioClip[] clips;

    public void Play(int index, float volume, float pitch)
    {
        AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
        audioSource.clip = clips[index];
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        audioSource.Play();
        Destroy(audioSource, clips[index].length);
    }

    public void PlayPitch(int index)
    {
        AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
        audioSource.clip = clips[index];
        audioSource.volume = 0.5f;
        audioSource.pitch = Random.Range(0.9f, 1.1f);

        audioSource.Play();
        Destroy(audioSource, clips[index].length);
    }

}
