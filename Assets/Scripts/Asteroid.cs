using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    // rotation speed
    private float _rotateSpeed = 18.0f;

    [SerializeField]
    private GameObject _explosionPrefab;
    private SpawnManager _spawnManager; 

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null )
        {
            Debug.LogError("Spawn Manager is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // rotate this game object on z-axis
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }

    // Check for laser collision (Trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            _spawnManager.StartSpawning();
            Destroy(this.gameObject, 0.25f);
        }
    }
    // Instantiate explosion at the position of the asteroid
    // Destroy the explosion after 3 seconds.
}
