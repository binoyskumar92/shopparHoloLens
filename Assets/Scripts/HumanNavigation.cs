using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanNavigation : MonoBehaviour {

    public GameObject destination;

    public static NavMeshAgent agent;

    LineRenderer lineRenderer;


    public static Vector3 destinationVector;


    void Start()
    {

        lineRenderer = GetComponent<LineRenderer>();

        agent = GetComponent<NavMeshAgent>();

        agent.updatePosition = false;

        destinationVector = destination.transform.position;

    }

    public void setDestinationHelper(Vector3 val)
    {
        agent.SetDestination(val);
    }
    public void resetLineRenderer()
    {
        lineRenderer.SetVertexCount(0);
        Debug.Log("Reset linerenderer done");
    }

    void Update()
    {
        agent.SetDestination(destinationVector);
        agent.nextPosition = Camera.main.transform.position;

        if (agent.hasPath || agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            var positions = agent.path.corners;

            lineRenderer.positionCount = positions.Length;

            for (int i = 0; i < positions.Length; i++)

            {
                lineRenderer.SetPosition(i, positions[i]);
            }
        }
    }
}
