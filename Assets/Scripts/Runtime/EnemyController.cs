using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Vector3 targetPosition;
    private Vector3 originalScale;
    private bool isDead = false;

    [Header("Movement")]
    [SerializeField]
    private Transform leftPoint;

    [SerializeField]
    private Transform rightPoint;

    [SerializeField]
    private float movementSpeed = 2.0f;

    [Header("Animation")]
    [SerializeField]
    private Animator animator;

    [Header("Squash")]
    [SerializeField]
    private Vector3 squashScale = new Vector3(1.3f, 0.35f, 1.3f);

    [SerializeField]
    private float squashTime = 0.15f;

    [SerializeField]
    private float disappearDelay = 0.25f;

    [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip squashSound;

    private void Start()
    {
        this.originalScale = this.transform.localScale;

        if (this.rightPoint != null)
        {
            this.targetPosition = this.rightPoint.position;
        }

        if (this.animator == null)
        {
            this.animator = this.GetComponentInChildren<Animator>();
        }

        if (this.audioSource == null)
        {
            this.audioSource = this.GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (this.isDead)
        {
            return;
        }

        MoveEnemy();
    }

    private void MoveEnemy()
    {
        if (this.leftPoint == null || this.rightPoint == null)
        {
            return;
        }

        this.transform.position = Vector3.MoveTowards(
            this.transform.position,
            this.targetPosition,
            this.movementSpeed * Time.deltaTime
        );

        Vector3 direction = this.targetPosition - this.transform.position;
        direction.y = 0.0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            this.transform.forward = direction.normalized;
        }

        if (Vector3.Distance(this.transform.position, this.targetPosition) < 0.05f)
        {
            if (this.targetPosition == this.rightPoint.position)
            {
                this.targetPosition = this.leftPoint.position;
            }
            else
            {
                this.targetPosition = this.rightPoint.position;
            }
        }

        if (this.animator != null)
        {
            this.animator.SetBool("IsRunning", true);
        }
    }

    public void Squash()
    {
        if (this.isDead)
        {
            return;
        }

        StartCoroutine(SquashCoroutine());
    }

    private IEnumerator SquashCoroutine()
    {
        this.isDead = true;

        if (this.animator != null)
        {
            this.animator.SetBool("IsRunning", false);
        }

        if (this.audioSource != null && this.squashSound != null)
        {
            this.audioSource.PlayOneShot(this.squashSound);
        }

        this.transform.localScale = this.squashScale;

        yield return new WaitForSeconds(this.squashTime);

        yield return new WaitForSeconds(this.disappearDelay);

        Destroy(this.gameObject);
    }
}