using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public List<Node> neighbors;
    public Node parent;
    public Vector3 pos;
    public float gCost;

    public Node(Vector3 _pos)
    {
        pos = _pos;
        neighbors = new List<Node>();
        gCost = 0;
        parent = null;
    }

}
