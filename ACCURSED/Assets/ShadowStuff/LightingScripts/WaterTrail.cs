using System.Collections.Generic;
using UnityEngine;

public class WaterTrail : MonoBehaviour
{
    LineRenderer lineRenderer;
    [SerializeField] float minWidth;
    [SerializeField] float widthIncreaseRate;
    [SerializeField] float trailDuration;

    float trailLength = 0;
    Vector2 prevPosition;
    Queue<Vector3> positions = new Queue<Vector3>();
    Queue<float> timeLeft = new Queue<float>();
    float timePassed = 0;

    float fadeBufferTime;
    Vector3[] emptyPositionsArray = new Vector3[0];

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        prevPosition = transform.position;
        StartTrail();
    }

    public void StartTrail()
    {
        fadeBufferTime = trailDuration;
        trailLength = 0;
        lineRenderer.SetPositions(emptyPositionsArray);
        timePassed = 0;
        positions = new Queue<Vector3>();
        timeLeft = new Queue<float>();
        lineRenderer.startWidth = minWidth;
        lineRenderer.endWidth = minWidth;
    }

    void Update()
    {
        float distTravelled = ((Vector2)transform.position - prevPosition).sqrMagnitude;
        trailLength += distTravelled;

        lineRenderer.endWidth = widthIncreaseRate * trailLength;
    }
}
