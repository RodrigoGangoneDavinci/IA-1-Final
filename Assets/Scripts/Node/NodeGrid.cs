using UnityEngine;
using System.Linq;

public static class NodeGrid
{
    public static Node[] AllNodes;

    public static Node GetClosestNode(Vector3 position)
    {
        if (AllNodes == null || AllNodes.Length == 0) return null;
        return AllNodes.OrderBy(n => Vector3.Distance(position, n.transform.position)).FirstOrDefault();
    }
}