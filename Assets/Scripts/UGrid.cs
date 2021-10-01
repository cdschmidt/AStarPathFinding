using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UGrid : MonoBehaviour
{
    // Start is called before the first frame update
    private AStar astar;
    public Material obsticalMat;
    public bool drawPRM = true;
    public int numNodes;
    public int numObsticals;
    public float width;
    public float height;
    public List<Node> nodes;
    private GameObject[] obsticals;
    public Transform player;
    public float playerSpeed = 10;
    private Animator animator;
    void Awake()
    {
        obsticals = new GameObject[numObsticals];
        for (int i = 0; i < numObsticals; i++) {
            obsticals[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var pos = new Vector3(Random.Range(-width/2.2f, width/2.2f), 1, Random.Range(-height/2.2f, height/2.2f));
            var scale = new Vector3(Random.Range(1, 10), 2, Random.Range(1, 10));
            obsticals[i].transform.position = pos;
            obsticals[i].transform.localScale = scale;
            var boxCollider = obsticals[i].GetComponent<BoxCollider>();
            boxCollider.size = new Vector3(1 + (1 / scale.x), 1, 1 + (1 / scale.z));
            var rend = obsticals[i].GetComponent<Renderer>();
            rend.material = obsticalMat;
        }
        nodes = new List<Node>();
        bool addNode = true;
        for (int i = 0; i < numNodes; i++)
        {
            addNode = true;
            var pos = new Vector3(Random.Range(-width/2, width/2), 0, Random.Range(-height/2, height/2));
            foreach (var obstical in obsticals)
            {
                var col = obstical.GetComponent<Collider>();
                if (col.bounds.Contains(pos))
                {
                    addNode = false;
                    break;
                }
            }
            if (addNode)
            {
                nodes.Add(new Node(pos));
            }
            
        }

        astar = GetComponent<AStar>();
        player.position = astar.seeker.position;
        Vector3 dir = astar.target.position - player.position;
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
        player.rotation = rotation;
        animator = player.GetComponent<Animator>();
    }

    public void AddNeighbors()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = 0; j < nodes.Count; j++)
            {
                Vector3 dir = nodes[j].pos - nodes[i].pos;
                float dist = Vector3.Distance(nodes[i].pos, nodes[j].pos);
                if(dist <= 0.001) continue;
                bool hit = Physics.Raycast(nodes[i].pos, dir, dist);
                if (!hit)
                {
                    nodes[i].neighbors.Add(nodes[j]);
                }
            }
        }
    }

    public List<Node> path;
    private void OnDrawGizmos()
    {
        if (drawPRM && nodes != null)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < nodes.Count-1; i++)
            {
                for (int j = i+1; j < nodes.Count; j++)
                {
                    Vector3 dir = nodes[j].pos - nodes[i].pos;
                    float dist = Vector3.Distance(nodes[i].pos, nodes[j].pos);
                    bool hit = Physics.Raycast(nodes[i].pos, dir, dist);
                    if (!hit)
                    {
                        Gizmos.DrawLine(nodes[i].pos, nodes[j].pos);
                    }
                
                }
            }
        }
        

        if (path != null)
        {
            print(path.Count);
            Gizmos.color = Color.red;
            for (int i = 0; i < path.Count-1; i++)
            {
                Gizmos.DrawLine(path[i].pos, path[i+1].pos);
            }
        }
        
    }

    void MovePlayer()
    {
        if (path.Count == 0)
        {
            animator.SetBool("isIdle", true);
            return;
        }
        for (int j = 0; j < path.Count; j++)
        {
            Vector3 pathDir = path[j].pos - player.position;
            float dist = Vector3.Distance(path[j].pos, player.position);
            if(dist <= 0.001) continue;
            bool hit = Physics.Raycast(player.position, pathDir, dist);
            if (!hit)
            {
                path.RemoveRange(0,j);
            }
        }
        Vector3 dir = path[0].pos - player.position;
        dir.Normalize();
        player.position += playerSpeed * Time.deltaTime * dir;
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
        player.rotation = Quaternion.Lerp(player.rotation, rotation, Time.time * .001f);
        if (Vector3.Distance(path[0].pos, player.position) < 0.1)
        {
            path.RemoveAt(0);
        }

        if (path.Count == 0)
        {
            animator.SetBool("isIdle", true);
        }
    }
    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }
}
