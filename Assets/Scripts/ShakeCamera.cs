using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{

    // Public method to trigger the camera shake
    public void TriggerShake(float amountTimeRemaining, float amountShake)
    {
        StartCoroutine(Shake(amountTimeRemaining, amountShake));
    }
    public IEnumerator Shake(float amountTimeRemaining, float amountShake) // choreograph shake movement
    {
        // get this gameobjects position
        Vector3 originalPosition = this.transform.position;
        // track the time 
        float amountTimePassed = 0;
        // if the amount of elapsed time is less than set time of duration
        while (amountTimePassed < amountTimeRemaining)
        {
            // then shake camera between a random range on the x, y, and z
            float x = Random.Range(-1f, 1f) * amountShake;
            float y = Random.Range(-1f, 1f) * amountShake;
            float z = Random.Range(-1f, 1f) * amountShake;

            // set new position for x, y and z coordinates during shake
            this.transform.position = new Vector3(x, y, z) + originalPosition; 
            // increament time of shake based on real time
            amountTimePassed += Time.deltaTime;

            yield return null; // pause after each loop 
        }

        // set this gameobjects position back to original position
        this.transform.position = originalPosition;
    }
 
}
