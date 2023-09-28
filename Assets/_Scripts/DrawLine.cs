using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    //Just draws a line. Nothing else here
    private LineRenderer lineRenderer;
    private float counter;
    private float dist;

    public Transform origin;
    public Transform destination;

    public float lineDrawSpeed = 6f;


    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, origin.position);
        lineRenderer.startWidth = .45f;
        lineRenderer.endWidth = .45f;

        dist = Vector3.Distance(origin.position, destination.position);
    }

    
    void Update()
    {

     if (counter < dist)
     {
        counter += .1f / lineDrawSpeed;

        float x = Mathf.Lerp(0, dist, counter);

        Vector3 pointA = new Vector3(origin.position.x, origin.position.y, 2);
        Vector3 pointB = new Vector3(destination.position.x, destination.position.y, 2);

        Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;
        
        lineRenderer.SetPosition(1, pointAlongLine);

        if (pointB == null){pointB = origin.position;}
     }   
    }
}
