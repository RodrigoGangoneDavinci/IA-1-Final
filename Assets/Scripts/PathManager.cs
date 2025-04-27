using UnityEngine;

public class PathManager : MonoBehaviour
{
    void Awake()
    {
        NodeGrid.AllNodes = FindObjectsOfType<Node>();
    }
}