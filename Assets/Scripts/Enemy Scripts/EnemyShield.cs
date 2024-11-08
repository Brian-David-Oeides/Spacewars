using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    private bool _isActive = false;

    public bool IsActive => _isActive;

    public void Activate()
    {
        _isActive = true;
        // add visual feedback for the shield activation
    }

    public void Deactivate()
    {
        _isActive = false;
        // add visual feedback for the shield deactivation
    }

    public bool AbsorbHit()
    {
        if (_isActive)
        {
            Deactivate();
            return true; // hit absorbed
        }
        return false; // reset
    }
}
