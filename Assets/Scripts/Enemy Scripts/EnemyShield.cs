using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    private bool _isActive = true;
    
    [SerializeField]
    private int _shieldStrength = 1; // number of hits the shield can absorb

    public void Activate()
    {
        _isActive = true;
        Debug.Log("Enemy shield Activated!");
       
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
            // shield is still active and absorbed collision
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
