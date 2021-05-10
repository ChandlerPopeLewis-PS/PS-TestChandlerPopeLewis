using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    public Vector2 speed = new Vector2(7, 12);

    Rigidbody _rigidbody;

    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 newPosition = transform.position + (new Vector3(0, 0, -Random.Range(speed.x, speed.y)) * Time.fixedDeltaTime);
        
        if(Physics.Raycast(transform.position, Vector3.back, 1f, LayerMask.GetMask("HoleCollider"), QueryTriggerInteraction.Collide)) {
            ObjectPool.GetObjectPool("Obstacle").Push(gameObject);
        }

        _rigidbody.MovePosition(newPosition);
    }
}
