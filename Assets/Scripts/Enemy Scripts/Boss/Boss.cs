using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float speed = 4f;
    public int maxHealth = 10;
    private int _currentHealth;

    //public Slider healthSlider;

    private IBossState currentState;

    public Transform playerTransform; // Reference to the player's Transform

    public GameObject bossExplosionPrefab;
    private AudioSource _audioSource;
    private ShakeCamera _cameraShake;

    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer for color changes

    public float dodgeRange = 5f; // Range to detect player's laser
    public float dodgeDistance = 8f;
    
    private void Start()
    {

        _cameraShake = Camera.main.GetComponent<ShakeCamera>();
        _audioSource = GetComponent<AudioSource>();

        _currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the Boss GameObject!");
        }

        // Setup health slider
        /*if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        else
        {
            Debug.LogWarning("Health slider not assigned in the Inspector!");
        }*/

        // find Player Transform dynamically if not assigned in Inspector
        if (playerTransform != null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("Player GameObject not found. Ensure the Player has the 'Player' tag.");
            }
        }

        // Start in Idle state
        SetState(new BossIdleState());
    }

    private void Update()
    {
        // Execute the current state's behavior
        currentState?.Execute(this);
    }

    public void SetState(IBossState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
    }


    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        /*if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }*/

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Stop all movement and state execution
        speed = 0; // Stop movement based on speed
        currentState = null; // Disable state execution

        // Instantiate boss-specific explosion
        if (bossExplosionPrefab != null)
        {
            GameObject explosion = Instantiate(bossExplosionPrefab, transform.position, Quaternion.identity);

            // Trigger the explosion animation
            Animator explosionAnimator = explosion.GetComponent<Animator>();
            
            if (explosionAnimator != null)
            {
                explosionAnimator.SetTrigger("PlayExplosion");
            }
        }
        else
        {
            Debug.LogError("Boss explosion prefab is not assigned!");
        }

        // Play explosion sound
        if (_audioSource != null)
        {
            _audioSource.Play();
        }

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }

        // Stop power-up spawning
        SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
        if (spawnManager != null)
        {
            spawnManager.StopPowerUpSpawning();
        }

        Debug.Log("Boss defeated!");
        
        Destroy(this.gameObject, 2.8f);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering the range is a laser
        if (other.tag == "Laser")
        {
            Debug.Log("Laser detected. Triggering evade...");
            // Ensure the boss is in the EvadeState
            if (currentState is BossEvadeState evadeState)
            {
                evadeState.TriggerEvade(this, other.transform.position);
            }

            // Take damage from the laser
            TakeDamage(1); // Example: Boss takes 5 damage per laser hit
            Debug.Log("Direct Hit!");
            
            // Start the flash coroutine
            StartCoroutine(FlashRed());

            // Destroy the laser after it hits the boss
            Destroy(other.gameObject);
        }
        // Check if the object entering the range is the player
        else if (other.tag == "Player")
        {
            Debug.Log("Boss collided with Player!");

            // Access the Player script to apply damage or effects
            Player player = other.transform.GetComponent<Player>();
            
            if (player != null)
            {
                player.Damage(1); // Example: Inflict 10 damage to the player
                if (_cameraShake != null)
                {
                    _cameraShake.TriggerShake(0.3f, 0.5f);
                }
            }
        }
    }
    private IEnumerator FlashRed()
    {
        if (spriteRenderer == null) yield break;

        // Store the original color
        Color originalColor = spriteRenderer.color;

        // Change to red
        spriteRenderer.color = Color.red;

        // Wait for a short duration
        yield return new WaitForSeconds(0.1f);

        // Return to the original color
        spriteRenderer.color = originalColor;
    }
}
