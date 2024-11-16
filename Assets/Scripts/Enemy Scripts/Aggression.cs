using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // need to use keyword Action 

public class Aggression : MonoBehaviour 
{
    // delcare an event using Action delegate

    // adjust the ramming range
    // track/enable/disable ramming behavior
    // reference players transform component
    // store the direction towards player

    // Start is called before the first frame update
    void Start()
    {
        // set the reference to find the players stransform
        // null check
        // debug statement
    }

    // Update is called once per frame
    void Update()
    {
        // if ramming is true, reset
        // if ramming is not true call the ram method
    }

    // check the distance to player method
        // if player does not exist, reset
        // calculate and store the distance to player
        // if distance to player is less than equal to ramming range then 
        // call ramming range

    // enable ram behavior
        // set to true/enable ramming
        // calculate the direction/vector
        // invoke subscribers to start ramming

    // fixed update
        // if ramming is true then 
        // move towards the player in real time 

    // dynamically set the ram range value during runtime
        // update ram range value
}
