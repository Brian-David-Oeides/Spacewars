using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float speed = 4f;
    public int maxHealth = 50;
    private int currentHealth;

    //public Slider healthSlider;

    private IBossState currentState;

    public Transform playerTransform; // Reference to the player's Transform
    public float dodgeRange = 5f; // Range to detect player's laser
    public float dodgeDistance = 8f;

    private void Start()
    {
        currentHealth = maxHealth;

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

        // Find Player Transform dynamically if not assigned in Inspector
        if (playerTransform == null)
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
        currentHealth -= damage;

        /*if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }*/

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Play explosion animation (if any)
        Debug.Log("Boss defeated!");
        // Add animation trigger or VFX here
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering the range is a laser
        if (other.CompareTag("Laser"))
        {
            Debug.Log("Laser detected. Triggering evade...");
            // Ensure the boss is in the EvadeState
            if (currentState is BossEvadeState evadeState)
            {
                evadeState.TriggerEvade(this, other.transform.position);
            }
        }
    }
}
