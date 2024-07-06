using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
        //Get the position of the game object each update
        transform.Translate(Vector3.left * _speed * Time.deltaTime);
    }
}
