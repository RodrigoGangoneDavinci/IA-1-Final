using UnityEngine;
 using System.Collections.Generic;
 
 public static class ThetaStarPathfinding
 {
     public static List<Vector3> GetPath(Vector3 start, Vector3 end)
     {
         Node startNode = NodeGrid.GetClosestNode(start);
         Node endNode = NodeGrid.GetClosestNode(end);
 
         if (startNode == null || endNode == null)
             return new List<Vector3>();
 
         List<Node> openList = new() { startNode };
         HashSet<Node> closedList = new();
 
         Dictionary<Node, Node> parent = new();
         Dictionary<Node, float> gCost = new();
         Dictionary<Node, float> fCost = new();
 
         gCost[startNode] = 0;
         fCost[startNode] = Vector3.Distance(startNode.Position, endNode.Position);
 
         while (openList.Count > 0)
         {
             openList.Sort((a, b) => fCost[a].CompareTo(fCost[b]));
             Node current = openList[0];
             openList.RemoveAt(0);
 
             if (current == endNode)
                 return ReconstructPath(parent, current);
 
             closedList.Add(current);
 
             foreach (var neighbor in current.Neighbors)
             {
                 if (closedList.Contains(neighbor)) continue;
 
                 float tentativeG = gCost[current] + Vector3.Distance(current.Position, neighbor.Position);
 
                 if (!openList.Contains(neighbor) || tentativeG < gCost.GetValueOrDefault(neighbor, float.MaxValue))
                 {
                     if (HasLineOfSight(parent.ContainsKey(current) ? parent[current] : current, neighbor))
                     {
                         parent[neighbor] = parent.ContainsKey(current) ? parent[current] : current;
                         gCost[neighbor] = tentativeG;
                     }
                     else
                     {
                         parent[neighbor] = current;
                         gCost[neighbor] = tentativeG;
                     }
 
                     fCost[neighbor] = gCost[neighbor] + Vector3.Distance(neighbor.Position, endNode.Position);
 
                     if (!openList.Contains(neighbor)) openList.Add(neighbor);
                 }
             }
         }
 
         return new List<Vector3>();
     }
 
     static bool HasLineOfSight(Node from, Node to)
     {
         Vector3 origin = from.Position + Vector3.up * 0.5f;
         Vector3 dir = (to.Position - origin).normalized;
         float dist = Vector3.Distance(origin, to.Position);
         return !Physics.Raycast(origin, dir, dist, LayerMask.GetMask("Wall"));
     }
 
     static List<Vector3> ReconstructPath(Dictionary<Node, Node> parent, Node current)
     {
         List<Vector3> path = new() { current.Position };
 
         while (parent.ContainsKey(current))
         {
             current = parent[current];
             path.Insert(0, current.Position);
         }
 
         return path;
     }
 }