using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    
    private bool _isShieldActive = false;
    private SpriteRenderer _shieldRenderer;

    void Start()
    {
        _shieldRenderer = GetComponent<SpriteRenderer>();
        if (_shieldRenderer == null)
        {
            Debug.LogError("Shield SpriteRenderer is missing.");
        }

        DeactivateShield(); // Start with the shield deactivated
    }

    public void ActivateShield()
    {
        _isShieldActive = true;
        _shieldRenderer.enabled = true; // Show the shield
    }

    public void DeactivateShield()
    {
        _isShieldActive = false;
        _shieldRenderer.enabled = false; // Hide the shield
    }

    public bool IsShieldActive()
    {
        return _isShieldActive;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser" && _isShieldActive)
        {
            DeactivateShield(); // Disable shield after being hit
            Destroy(other.gameObject); // Destroy the laser
        }
    }
}
