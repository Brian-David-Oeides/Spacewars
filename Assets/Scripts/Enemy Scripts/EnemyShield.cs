using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    private bool _isActive = true;

    public bool IsActive => _isActive;

    public void Activate()
    {
        _isActive = true;
        Debug.Log("Enemy shield Activate!");
        // add visual feedback for the shield activation
    }

    public void Deactivate()
    {
        _isActive = false;
        Debug.Log("Deactivate shield!");
        // add visual feedback for the shield deactivation
    }

    public bool AbsorbHit()
    {
        if (_isActive)
        {
            Deactivate();
            Debug.Log("Enemy shield Deactivated!");
            return true; // hit absorbed
        }
        return false; // reset
    }
}
