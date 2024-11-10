using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    private bool _isActive = true;
    [SerializeField]
    private int _shieldStrength = 1; // Number of hits the shield can absorb

    public bool IsActive => _isActive;

    public void Activate()
    {
        _isActive = true;
        Debug.Log("Enemy shield Activate!");
       
        // activate shield visuals or any initialization
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        _isActive = false;
        Debug.Log("Deactivate shield!");

        // deactivate shield visuals
        Destroy(gameObject);
    }

    public bool AbsorbHit()
    {
        if (_isActive && _shieldStrength > 0)
        {
            // Shield is still active and absorbed the hit
            return true;
        }
        else
        { 
            Deactivate();
            Debug.Log("Enemy shield Deactivated!");
            return false; // reset
        }
    }
}
