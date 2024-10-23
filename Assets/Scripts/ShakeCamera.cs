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
    public IEnumerator Shake(float amountTimeRemaining, float amountShake) 
    {
        
        Vector3 originalPosition = this.transform.position;
        float amountTimePassed = 0;
        
        while (amountTimePassed < amountTimeRemaining)
        {
            
            float x = Random.Range(-1f, 1f) * amountShake;
            float y = Random.Range(-1f, 1f) * amountShake;
            float z = Random.Range(-1f, 1f) * amountShake;

            
            this.transform.position = new Vector3(x, y, z) + originalPosition; 
            
            amountTimePassed += Time.deltaTime;

            yield return null;  
        }

        this.transform.position = originalPosition;
    }
 
}
