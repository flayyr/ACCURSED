using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WaterTrail : MonoBehaviour
{
    LineRenderer lineRenderer;
    [SerializeField] float minWidth = 1f;
    [SerializeField] float widthIncreaseRate=3f;
    [SerializeField] float trailDuration = 1f;


    float trailSquaredLength = 0;
    Vector3 prevPosition;

    Queue<Vector3> positions = new Queue<Vector3>();
    Queue<float> timesLeft = new Queue<float>();
    Queue<float> squaredDists = new Queue<float>();
    

    float fadeBufferTime;
    float timePassed = 0;

    bool inWater;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        prevPosition = transform.position;
    }

    public void StartTrail()
    {
        fadeBufferTime = trailDuration;
        timePassed = 0;
        trailSquaredLength = 0;
        lineRenderer.positionCount = 0;

        prevPosition = transform.position;
        positions = new Queue<Vector3>();
        positions.Enqueue(prevPosition);

        timesLeft = new Queue<float>();
        timesLeft.Enqueue(fadeBufferTime);

        squaredDists = new Queue<float>();
        squaredDists.Enqueue(0);

        lineRenderer.startWidth = minWidth;
        lineRenderer.endWidth = minWidth;

        inWater = true;
    }

    void Update()
    {
        if (inWater)
        {
            //add new position
            timesLeft.Enqueue(Time.deltaTime);
            positions.Enqueue(transform.position);

            float currDist = (transform.position - prevPosition).sqrMagnitude;
            trailSquaredLength += currDist;
            squaredDists.Enqueue(currDist);
            prevPosition = transform.position;


        }


        //remove positions
        if (timesLeft.Count > 0)
        {
            timePassed += Time.deltaTime;
            while (timesLeft.Count > 0 && timePassed > timesLeft.Peek())
            {
                timePassed -= timesLeft.Dequeue();
                Vector3 positionToRemove = positions.Dequeue();
                trailSquaredLength -= squaredDists.Dequeue();
            }
        }

        //set positions and width
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
        lineRenderer.startWidth = minWidth + widthIncreaseRate * Mathf.Sqrt( trailSquaredLength);
    }

    public void EndTrail()
    {
        inWater = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Water")
        {
            StartTrail();
            //Debug.Log("start");
        }
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "Water")
    //    {
    //        Debug.Log("stay");
    //    }
    //}

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Water")
        {
            EndTrail();
            //Debug.Log("end");
        }
    }
}
