using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Node : MonoBehaviour
{
    public Vector3 Position => transform.position;
    public List<Node> Neighbors = new();

    void Start()
    {
        CheckNeighbors();
    }

    void CheckNeighbors()
    {
        Vector3[] directions = { Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        foreach (var dir in directions)
        {
            // ignoro la layer "Leader"
            int mask = ~( (1 << LayerMask.NameToLayer("Leader")) | (1 << LayerMask.NameToLayer("NPC")) );

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, 4.5f, mask))
            {
                Node neighbor = hit.collider.GetComponent<Node>();
                if (neighbor != null && !Physics.Raycast(transform.position, dir, 4.5f, LayerMask.GetMask("Wall")))
                {
                    Neighbors.Add(neighbor);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Vector3 origin = transform.position;
        Vector3[] directions = {
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back
        };

        foreach (Vector3 dir in directions)
        {
            Vector3 target = origin + dir * 4.5f;
            
            // ignoro la layer "Leader"
            int mask = ~( (1 << LayerMask.NameToLayer("Leader")) | (1 << LayerMask.NameToLayer("NPC")) );

            Ray ray = new Ray(origin, dir);
            if (Physics.Raycast(ray, out RaycastHit hit, 4.5f, mask, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.GetComponent<Node>() != null)
                {
                    Gizmos.color = Color.green;
                }
                else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.magenta;
                }
            }
            else
            {
                Gizmos.color = Color.magenta;
            }

            Gizmos.DrawLine(origin, target);
        }
    }

}