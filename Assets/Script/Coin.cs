using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float spinRate = 0.1f;
    void Awake()
    {
        StartCoroutine(Spin());
    }

    IEnumerator Spin() {
        float time = 0.0f;
        while(gameObject.activeSelf) {
            yield return new WaitForSeconds(spinRate);
            
            time += spinRate;
            if(time > 1) time = 0;
            transform.eulerAngles = Vector3.LerpUnclamped(new Vector3(0, 0, -90), new Vector3(0, 360, -90), time);
        }
    }
}
