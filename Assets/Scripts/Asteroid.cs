using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private float _rotateSpeed = 18.0f;

    [SerializeField]
    private GameObject _explosionPrefab;

    [SerializeField]
    private WaveManager _waveManager;

    void Start()
    {
        _waveManager = GameObject.Find("Wave_Manager").GetComponent<WaveManager>();

        if (_waveManager == null)
        {
            Debug.LogError("WaveManager is NULL.");
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject);

            _waveManager.StartWave(1); // trigger the first wave after asteroid destruction
            
            Destroy(this.gameObject, 0.25f);
        }
    }
}
