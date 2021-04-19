using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public float radius = 2.0f;
    public float radiusSpeed = 0.5f;
    public float rotationSpeed = 400.0f;

    Transform center;
    Vector3 axisUp = Vector3.up;
    Vector3 axisRight = Vector3.right;

    Vector3 desiredPosition;

    void Start()
    {
        center = transform;
        transform.position = (transform.position - center.position).normalized * radius + center.position;
        radius = 2.0f;

        transform.Rotate(new Vector3(Random.Range(-100,100), Random.Range(-100, 100), Random.Range(-100, 100)));
    }

    void Update()
    {
        transform.RotateAround(center.position, axisUp, rotationSpeed * Time.deltaTime);
        transform.RotateAround(center.position, axisRight, rotationSpeed * Time.deltaTime);
        desiredPosition = (transform.position - center.position).normalized * radius + center.position;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed);
    }
}
