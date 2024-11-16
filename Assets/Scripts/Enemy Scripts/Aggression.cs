using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // need to use keyword Action 

public class Aggression : MonoBehaviour 
{
    // delcare an event using Action delegate
    public event Action Ramming;

    // adjust the ramming range
    [SerializeField]
    private float _rammingRange = 2f;
    // track/enable/disable ramming behavior
    private bool _isRamming = false;
    // reference players transform component
    private Vector3 _rammingDirection;
    // store the direction towards player
    private Transform _playerTransform;

    void Start()
    {
        // set the reference to find the players stransform
        _playerTransform = GameObject.Find("Player").transform;
        // null check
        if (_playerTransform == null)
        {
            Debug.LogError("Player transform is not assigned."); // debug statement
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if ramming is true, reset
        if (_isRamming || !canRam()) return; // check if can ram is not true
        // if ramming is not true call the ram method
        CheckDistanceToRam(); 
    }

    // check the distance to player method
    private void CheckDistanceToRam()
    {
        // if player does not exist, reset
        if (_playerTransform == null) return;
        // calculate and store the distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
        // if distance to player is less than equal to ramming range then 
        if (distanceToPlayer <= _rammingRange)
        {
            // call ramming range
            StartRamming();
        }
    }

    // can ram method
    private bool canRam()
    {
        // if player transform doesn't exist reset
        if (_playerTransform == null) return false;
        // if this transform is less than the players position then
        if (transform.position.y < _playerTransform.position.y)
        {
            // verify passed players position 
            Debug.Log($"{gameObject.name} cannot begin ramming as it is below the player's position.");
            // reset
            return false; 
        }
        return true;
    }

    // enable ram behavior
    private void StartRamming()
    {
        // set to true/enable ramming
        _isRamming = true;
        // calculate the direction/vector
        _rammingDirection = (_playerTransform.position - transform.position).normalized;
        // invoke subscribers to start ramming
        Ramming?.Invoke();
    }

    // fixed update
    public void FixedUpdate()
    {
        // if ramming is true then 
        if (_isRamming)
        {
            // move towards the player in real time 
            transform.position += _rammingDirection * Time.deltaTime * 2f;
        }
    }

    // dynamically set the ram range value during runtime
    public void SetRammingRange(float range)
    {
        // update ram range value
        _rammingRange = range;
    }

    // Draw the Gizmo to visualize the ramming range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; // set Gizmo color to red
        Gizmos.DrawWireSphere(transform.position, _rammingRange); // draw a wireframe sphere
    }
}
