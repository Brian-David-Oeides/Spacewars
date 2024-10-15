using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartMissile : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;
    private GameObject _target;
    [SerializeField]
    private float _range = 11.0f;
    private float _selfDestructTime = 6.0f;

    void Start()
    {
        // Find an enemy target within range
        FindTarget();

        // Start the countdown for self-destruction if no target is found
        Invoke("SelfDestruct", _selfDestructTime);
    }

    void Update()
    {
        if (_target != null)
        {
            // move towards target
            Vector3 direction = (_target.transform.position - transform.position).normalized;
            transform.Translate(direction * _speed * Time.deltaTime);

            // check distance and destroy both missile and target on collision
            if (Vector3.Distance(transform.position, _target.transform.position) < 0.2f)
            {
                Destroy(_target, 2.8f); // Destroy enemy
                Destroy(this.gameObject); // Destroy missile
            }
        }
    }

    void FindTarget()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, _range);

        foreach (var enemy in enemiesInRange)
        {
            if (enemy.CompareTag("Enemy"))
            {
                _target = enemy.gameObject;
                break;
            }

            Debug.Log("Target Locked!");
        }
    }

    void SelfDestruct()
    {
        if (_target == null)
        {
            Destroy(this.gameObject); // destroy if no target found in 6 seconds
        }
    }

    // visualize OverlapCircle 
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;  // set color 
        Gizmos.DrawWireSphere(transform.position, _range);  // draw a circle at missile's position with range radius
    }
}
