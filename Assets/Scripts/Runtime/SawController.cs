using System;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class SawController : MonoBehaviour
{
    [Header("Spinning")]
    [SerializeField] private float spinSpeed = 300f;
    [SerializeField] private Vector3 spinAxis = Vector3.forward;

    [Header("Audio")]
    [SerializeField] private AudioClip idleSound;
    [SerializeField] private AudioClip cuttingSound;

    [Header("Particles")]
    [SerializeField] private ParticleSystem sparklingParticles;

    private AudioSource audioSource;

    private bool isCutting;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        sparklingParticles.Stop();
    }

    private void Start()
    {
        if (idleSound != null)
        {
            audioSource.clip = idleSound;
            audioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetState(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetState(false);
        }
    }

    private void SetState(bool newState)
    {
        if (isCutting == newState)
            return;

        // CUTTING
        if (newState)
        {
            isCutting = true;
            audioSource.clip = cuttingSound;
            audioSource.Play();

            sparklingParticles.Play();
        }
        // IDLE
        else
        {
            isCutting = false;
            audioSource.clip = idleSound;
            audioSource.Play();

            sparklingParticles.Stop();
        }
    }

    private void Update()
    {
        transform.Rotate(spinAxis, spinSpeed * Time.deltaTime);
    }
}
