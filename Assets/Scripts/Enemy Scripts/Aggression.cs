using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aggression : MonoBehaviour
{
    private Player _player;
    private float _ramSpeed = 5f;
    [SerializeField] private float _ramRange = 1.5f;

    private bool _isAggressive = false;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.LogError("Player not found.");
            return;
        }

       StartCoroutine(CheckAggressionRoutine()); // Check every 0.5 seconds
    }

    private IEnumerator CheckAggressionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f); // Run every 0.5 seconds

            if (_isAggressive && _player != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
                Debug.Log("Distance to player: " + distanceToPlayer);

                if (distanceToPlayer <= _ramRange)
                {
                    StartCoroutine(RamPlayer());
                }
            }
        }
    }

    private IEnumerator RamPlayer()
    {
        Debug.Log("Ramming player...");


        while (_isAggressive && _player != null)
        {
            Vector3 directionToPlayer = (_player.transform.position - transform.position).normalized;
            transform.position += directionToPlayer * _ramSpeed * Time.deltaTime;

            yield return null; // Continue moving in each frame
        }
        Debug.Log("Stopped ramming player.");
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isAggressive = true; // Activate aggression when player is close
            Debug.Log("Player entered aggression range.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isAggressive = false; // Stop aggression when player moves away
            Debug.Log("Player exited aggression range.");
        }
    }
}
