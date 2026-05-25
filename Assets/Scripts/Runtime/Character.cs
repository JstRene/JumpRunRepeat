using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{

    private bool isJumping = false;

    private float jumpCooldownTimer;

    private CharacterController controller;

    private InputAction moveAction;
    private InputAction jumpAction;

    private Animator animator;

    [SerializeField]
    private float jumpCooldown;

    [SerializeField]
    private float characterSpeed;

    [SerializeField]
    private float dampening;

    [SerializeField]
    private float gravity;

    [SerializeField]
    private float jumpSpeed;

    [SerializeField]
    private float platformRayDistance;

    [SerializeField]
    private Transform cameraTransform;

    //Audio
    [SerializeField]
    private AudioSource footstepsAudioSource;

    [SerializeField]
    private AudioSource jumpAudioSource;

    [SerializeField]
    private AudioClip footstepsClip;

    [SerializeField]
    private AudioClip jumpClip;
    //Audio Ende nur zum merken für mich
    //Partikel
    [SerializeField]
    private ParticleSystem runningDustParticles;
    //Partikel Ende
    //Gegner
    [SerializeField]
    private float enemyBounceSpeed = 8.0f;

    [SerializeField]
    private float enemyStompHeightOffset = 0.3f;
    //Gegner Ende

    [SerializeField] private float maxHealth = 100.0f;

    private float currentHealth;
    public float GetCurrentHealth() => this.currentHealth;
    public float GetMaxHealth() => this.maxHealth;

    private Vector3 characterMovement;
    private Vector3 jumpVelocity;
    private Vector3 platformVelocity;
    private Vector3 characterGravity;

    private void Start()
    {
        this.currentHealth = this.maxHealth;
        this.controller = this.GetComponent<CharacterController>();
        this.moveAction = InputSystem.actions.FindAction("Move");
        this.jumpAction = InputSystem.actions.FindAction("Jump");
        this.jumpCooldownTimer = 0.0f;
        this.animator = this.GetComponent<Animator>();

        //Audio
        if (this.footstepsAudioSource != null)
        {
            this.footstepsAudioSource.clip = this.footstepsClip;
            this.footstepsAudioSource.loop = true;
            this.footstepsAudioSource.playOnAwake = false;
        }

        if (this.jumpAudioSource != null)
        {
            this.jumpAudioSource.loop = false;
            this.jumpAudioSource.playOnAwake = false;
        }
        //Partikel
        if (this.runningDustParticles != null)
        {
            this.runningDustParticles.Stop();
        }
    }

    void HandleJumping()
    {
        if (this.controller.isGrounded && this.isJumping && this.jumpCooldownTimer <= 0.0f)
        {
            this.jumpVelocity = Vector3.zero;
            this.isJumping = false;
        }

        if (this.controller.isGrounded && !this.isJumping && this.jumpAction.WasPressedThisFrame())
        {
            this.characterGravity = Vector3.zero;
            this.jumpVelocity = Vector3.zero;
            this.jumpVelocity.y = this.jumpSpeed;
            this.jumpCooldownTimer = this.jumpCooldown;
            this.isJumping = true;
        }

        if (this.jumpVelocity.y > 0.0f)
        {
            this.jumpVelocity.y -= Time.fixedDeltaTime;
        }
        else
        {
            this.jumpVelocity = Vector3.zero;
        }

        this.jumpCooldownTimer -= Time.fixedDeltaTime;

    }

    private void HandlePlatforms()
    {
        this.platformVelocity = Vector3.zero;
        if(this.controller.isGrounded 
            && Physics.Raycast(this.transform.position, Vector3.down, out var hit, this.platformRayDistance, LayerMask.GetMask("Platforms"))) {
            var platformObject = hit.collider.gameObject;
            var movingPlatform = platformObject.GetComponent<MovingPlatform>();
            if(movingPlatform != null)
            {
                this.platformVelocity = movingPlatform.GetVelocity();
            }
        }
    }

    private void FixedUpdate()
    {
        this.HandleJumping();
        this.HandlePlatforms();
        var inputMovement = this.moveAction.ReadValue<Vector2>();
        this.HandleRunningDust(inputMovement); //Partikel
        //this.SetAnimationState(); Alte Rene klein Version
        this.SetAnimationState(inputMovement);
        this.HandleFootsteps(inputMovement);

        var inputRightDirection = this.cameraTransform.right;
        var inputForwardDirection = this.cameraTransform.forward;

        inputRightDirection.y = 0f;
        inputForwardDirection.y = 0f;
        inputRightDirection.Normalize();
        inputForwardDirection.Normalize();

        if (this.controller.isGrounded) {
            this.characterGravity.y = 0.0f;
        }

        this.characterGravity.y += this.gravity * Time.fixedDeltaTime;
        this.characterMovement += this.characterGravity * Time.fixedDeltaTime;
        this.characterMovement += this.jumpVelocity * Time.fixedDeltaTime;
        this.characterMovement += inputRightDirection * inputMovement.x * this.characterSpeed * Time.fixedDeltaTime;
        this.characterMovement += inputForwardDirection * inputMovement.y * this.characterSpeed * Time.fixedDeltaTime;

        this.characterMovement *= (1.0f - this.dampening);

        Vector3 characterForward = this.characterMovement;
        characterForward.y = 0.0f;

        if(characterForward.sqrMagnitude > 0.0f && characterForward != Vector3.zero) {
            this.transform.forward = characterForward.normalized;
        }

        this.controller.Move(this.characterMovement + this.platformVelocity * Time.fixedDeltaTime);
    }

    
    /*void SetAnimationState()
    {
        this.animator.SetBool("IsJumping", this.isJumping); rene
    }*/

    void SetAnimationState(Vector2 inputMovement)
    {
        this.animator.SetBool("IsJumping", this.isJumping);
        this.animator.SetBool("IsRunning", inputMovement != Vector2.zero);
        this.animator.SetFloat("MovementForward", inputMovement.magnitude);
    }

    //Audio
    private void HandleFootsteps(Vector2 inputMovement)
    {
        if (this.footstepsAudioSource == null || this.footstepsClip == null)
        {
            return;
        }

        bool isMoving = inputMovement != Vector2.zero;
        bool shouldPlayFootsteps = isMoving && this.controller.isGrounded && !this.isJumping;

        if (shouldPlayFootsteps)
        {
            if (!this.footstepsAudioSource.isPlaying)
            {
                this.footstepsAudioSource.Play();
            }
        }
        else
        {
            if (this.footstepsAudioSource.isPlaying)
            {
                this.footstepsAudioSource.Stop();
            }
        }
    }

    private void PlayJumpSound()
    {
        Debug.Log("Du solltest kommen, fuck you");

        if (this.jumpAudioSource != null && this.jumpClip != null)
        {
            this.jumpAudioSource.PlayOneShot(this.jumpClip);
        }   
    }

    private void HandleRunningDust(Vector2 inputMovement)
    {
        if (this.runningDustParticles == null)
        {
            return;
        }

        bool isMoving = inputMovement != Vector2.zero;
        bool shouldPlayDust = isMoving && this.controller.isGrounded && !this.isJumping;

         if (shouldPlayDust)
        {
            if (!this.runningDustParticles.isPlaying)
            {
                this.runningDustParticles.Play();
            }
        }
        else
        {
            if (this.runningDustParticles.isPlaying)
            {
                this.runningDustParticles.Stop();
            }
        }
    }

    //Gegner springen
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        EnemyController enemy = hit.gameObject.GetComponentInParent<EnemyController>();

        if (enemy == null)
        {
            return;
        }

        bool playerIsAboveEnemy = this.transform.position.y > enemy.transform.position.y + this.enemyStompHeightOffset;
        bool playerIsFalling = this.characterGravity.y < 0.0f || this.characterMovement.y < 0.0f;

        if (playerIsAboveEnemy && playerIsFalling)
        {
            enemy.Squash();

            this.characterGravity = Vector3.zero;
            this.jumpVelocity = Vector3.zero;
            this.jumpVelocity.y = this.enemyBounceSpeed;
            this.isJumping = true;
        }
        else
        {
            Debug.Log("Was glotzt du von der Seite");
        }
    }

    public void InflictDamage(float amount)
    {
        this.currentHealth -= amount;
        this.currentHealth = Mathf.Clamp(this.currentHealth, 0.0f, this.maxHealth); 
    }
}

