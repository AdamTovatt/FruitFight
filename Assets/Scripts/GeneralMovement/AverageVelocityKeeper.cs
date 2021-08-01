using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AverageVelocityKeeper : MonoBehaviour
{
    public int TimeSamples = 10;
    public float TimeResolution = 0.02f;

    public AverageVelocityKeeper Parent { get; set; }

    public float Velocity { get { return float.IsNaN(_velocity) ? 0 : (Parent == null ? _velocity : _velocity - Parent.Velocity); } }
    private float _velocity = 0;

    private List<PointAndTime> points = new List<PointAndTime>();

    private PointAndTime lastSample = null;

    private NavMeshAgent navMeshAgent = null;

    void Start()
    {
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        lastSample = new PointAndTime(transform.position, Time.timeAsDouble, null);
    }

    void Update()
    {
        if (navMeshAgent == null)
        {
            if (lastSample == null)
                lastSample = new PointAndTime(transform.position, Time.timeAsDouble, null);

            PointAndTime currentSample = null;
            double currentTime = Time.timeAsDouble;
            if (currentTime - lastSample.Time > TimeResolution)
            {
                currentSample = new PointAndTime(transform.position, currentTime, lastSample);
                points.Add(currentSample);
                if (points.Count > TimeSamples)
                {
                    points.RemoveAt(0);
                }
                lastSample = currentSample;
            }

            double totalPassedTime = 0;
            float totalDistance = 0f;
            foreach (PointAndTime point in points)
            {
                totalPassedTime += point.TimeToPrevious;
                totalDistance += point.DistanceToPrevious;
            }

            _velocity = (float)(totalDistance / totalPassedTime);
        }
        else
        {
            _velocity = navMeshAgent.velocity.magnitude;
        }
    }
}

public class PointAndTime
{
    public Vector3 Point { get; set; }
    public double Time { get; set; }
    public float DistanceToPrevious { get; set; }
    public double TimeToPrevious { get; set; }

    public PointAndTime(Vector3 point, double time, PointAndTime lastSample)
    {
        Point = point;
        Time = time;

        if (lastSample != null)
        {
            DistanceToPrevious = Vector3.Distance(lastSample.Point, point);
            TimeToPrevious = time - lastSample.Time;
        }
        else
        {
            TimeToPrevious = 0.001;
        }
    }
}
