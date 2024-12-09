using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster
{
    private float _baseSpeed;
    private float _thrusterSpeed;
    private float _speedMultiplier;
    private float _thrustDuration;
    private float _thrustCooldown;

    private bool _isOnCooldown = false;

    public event Action<float> OnThrusterActivated; // Notify speed changes
    public event Action OnThrusterDeactivated;

    public Thruster(float baseSpeed, float speedMultiplier, float thrustDuration, float thrustCooldown)
    {
        _baseSpeed = baseSpeed;
        _speedMultiplier = speedMultiplier;
        _thrustDuration = thrustDuration;
        _thrustCooldown = thrustCooldown;
        _thrusterSpeed = baseSpeed;
    }

    public float CurrentSpeed => _thrusterSpeed; // Public getter for current speed

    public void HandleInput(bool isThrusterKeyPressed)
    {
        if (isThrusterKeyPressed && !_isOnCooldown)
        {
            ActivateThrusters();
        }
    }

    private void ActivateThrusters()
    {
        _isOnCooldown = true;
        _thrusterSpeed = _baseSpeed * _speedMultiplier;

        OnThrusterActivated?.Invoke(_thrusterSpeed); // Trigger event

        // Start cooldown and reset logic
        TimerUtility.InvokeAfterTime(_thrustDuration, DeactivateThrusters);
    }

    private void DeactivateThrusters()
    {
        _thrusterSpeed = _baseSpeed;
        OnThrusterDeactivated?.Invoke();

        // Reset cooldown
        TimerUtility.InvokeAfterTime(_thrustCooldown, () => _isOnCooldown = false);
    }

    public void UpdateBaseSpeed(float newBaseSpeed)
    {
        _baseSpeed = newBaseSpeed;
        if (!_isOnCooldown) _thrusterSpeed = newBaseSpeed; // Update current speed if not in cooldown
    }
}
