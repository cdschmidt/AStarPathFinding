using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    // Start is called before the first frame update
    private UGrid grid;
    public Transform seeker, target;
    private int slowUpdate = 0;

    private void Awake()
    {
        grid = GetComponent<UGrid>();
    }

    void Start()
    {
        StartCoroutine(LateStart(.5f));
    }
    
    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        runAStar(seeker.position, target.position);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void runAStar(Vector3 startPos, Vector3 goalPos)
    {
        Node startNode = new Node(startPos);
        Node goalNode = new Node(goalPos);
        float shortestFromStartDist = float.MaxValue;
        float shortestFromGoalDist = float.MaxValue;
        Node deleteStartNode = new Node(Vector3.zero);
        Node deleteGoalNode = new Node(Vector3.zero);;
        bool deleteTheStartNode = false;
        bool deleteTheGoalNode = false;
        foreach (var node in grid.nodes)
        {
            if (Vector3.Distance(node.pos, startPos) < shortestFromStartDist)
            {
                deleteStartNode = node;
                shortestFromStartDist = Vector3.Distance(node.pos, startPos);
                deleteTheStartNode = true;
            }
            
            if (Vector3.Distance(node.pos, goalPos) < shortestFromGoalDist)
            {
                deleteGoalNode = node;
                shortestFromGoalDist = Vector3.Distance(node.pos, goalPos);
                deleteTheGoalNode = true;
            }
        }
        grid.nodes.Add(startNode);
        grid.nodes.Add(goalNode);
        if(deleteTheStartNode)
            grid.nodes.Remove(deleteStartNode);
        if(deleteTheGoalNode)
            grid.nodes.Remove(deleteGoalNode);
        grid.RemoveBadNodes();
        grid.AddNeighbors();
        
        List<Node> path = new List<Node>();

        List<Node> fringe = new List<Node>();
        HashSet<Node> explored = new HashSet<Node>();
        fringe.Add(startNode);
        print(grid.nodes.Count);
        bool firstTime = true;
        while (fringe.Count > 0)
        {
            print("caling astar");
            float minFCost = float.MaxValue;
            Node node = fringe[0];
            if (!firstTime)
            {
                for(int i = 0; i < fringe.Count; i++)
                {
                    float hCost = Vector3.Distance(fringe[i].pos, goalNode.pos);
                    float fCost = fringe[i].gCost + hCost;
                    if(fCost < minFCost){
                        node = fringe[i];
                        minFCost = fCost;
                    }
                }
            }
            firstTime = false;

            fringe.Remove(node);
            explored.Add(node);
            
            if (node == goalNode)
            {
                RetracePath(startNode, goalNode);
                print("goal found");
                break;
            }

            foreach (var neighbor in node.neighbors)
            {
                if (explored.Contains(neighbor))
                {
                    continue;
                }

                float costToNeighbor = node.gCost + Vector3.Distance(node.pos, neighbor.pos);
                if (costToNeighbor < neighbor.gCost || !fringe.Contains(neighbor))
                {
                    neighbor.gCost = costToNeighbor;
                    neighbor.parent = node;
                }
                
                if (!fringe.Contains(neighbor))
                {
                    fringe.Add(neighbor);
                }
            }

        }
    }
    
    void RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(startNode);
        path.Reverse();

        grid.path = path;
    }
}
