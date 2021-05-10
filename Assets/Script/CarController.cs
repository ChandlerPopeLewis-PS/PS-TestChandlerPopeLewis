using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    public float speed;

    Rigidbody _rigidbody;

    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 newPosition = transform.position + (new Vector3(0, 0, -speed) * Time.fixedDeltaTime);
        
        _rigidbody.MovePosition(newPosition);
    }
}
