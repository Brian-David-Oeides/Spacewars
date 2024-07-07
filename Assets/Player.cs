using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;

    // Start is called before the first frame update
    void Start()
    {
        //Set the current position of this game object to = new position (0,0,0)
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        // new Vector3 (-5,0,0) * 5 * real-time
        transform.Translate(Vector3.right * horizontalInput * _speed * Time.deltaTime); 
    }
}
