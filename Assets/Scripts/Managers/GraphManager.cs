using UnityEngine;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour
{
    public GameObject nodePrefab;  
    public GameObject edgePrefab;  

    [SerializeField]
    public GameObject molecularItem; 
    [SerializeField]
    private GameObject graphCenter; 

    // Zone de génération des nœuds
    [SerializeField] private Vector2 spawnArea = new Vector2(10f, 10f); 

    private SpriteRenderer _spriteRenderer; 

    private List<Node> nodes = new List<Node>(); 
    private List<Edge> edges = new List<Edge>(); 
    private bool isGraphGenerated = false;  

    private void Awake()
    {
        _spriteRenderer = molecularItem.GetComponent<SpriteRenderer>();
        GenerateGraph(5, 10);
        ShowGraph(false);
    }

    void Update()
    {
        Debug.Log("isGraphGenerated :" + isGraphGenerated);

        if (_spriteRenderer.sprite != null)  
        {
            if (!isGraphGenerated)  
            {
                ShowGraph(true); 
                isGraphGenerated = true;  
                Debug.Log("Graphe affiché.");
            }
        }
        else
        {
            if (isGraphGenerated) 
            {
                ShowGraph(false); 
                isGraphGenerated = false;  
                Debug.Log("Graphe masqué.");
            }
        }
    }

    public void GenerateGraph(int nodeCount, int edgeCount)
    {
        CreateNodes(nodeCount);
        CreateEdges(edgeCount);
    }

    void CreateNodes(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 position = graphCenter.transform.position + new Vector3(
                Random.Range(-spawnArea.x / 2, spawnArea.x / 2),  // Utilise les limites de spawnArea
                Random.Range(-spawnArea.y / 2, spawnArea.y / 2),  
                0f);
            
            GameObject nodeObj = Instantiate(nodePrefab, position, Quaternion.identity);
            Node newNode = new Node(nodeObj, position);
            nodes.Add(newNode);
        }
    }

    void CreateEdges(int count)
    {
        int createdEdges = 0;
        while (createdEdges < count)
        {
            Node nodeA = nodes[Random.Range(0, nodes.Count)];
            Node nodeB = nodes[Random.Range(0, nodes.Count)];

            if (nodeA != nodeB && !AreNodesConnected(nodeA, nodeB))
            {
                GameObject edgeObj = Instantiate(edgePrefab);
                Edge newEdge = new Edge(nodeA, nodeB, edgeObj);
                edges.Add(newEdge);
                
                DrawEdge(newEdge);
                
                createdEdges++;
            }
        }
    }

    bool AreNodesConnected(Node nodeA, Node nodeB)
    {
        foreach (Edge edge in edges)
        {
            if ((edge.startNode == nodeA && edge.endNode == nodeB) || 
                (edge.startNode == nodeB && edge.endNode == nodeA))
            {
                return true;
            }
        }
        return false;
    }

    void DrawEdge(Edge edge)
    {
        LineRenderer lineRenderer = edge.edgeObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, edge.startNode.position);
        lineRenderer.SetPosition(1, edge.endNode.position);
        lineRenderer.startWidth = 0.1f; 
        lineRenderer.endWidth = 0.1f; 

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        
        Color edgeColor = new Color(1f, 1f, 1f, 0.01f); 
        lineRenderer.startColor = edgeColor; 
        lineRenderer.endColor = edgeColor;
    }

    void ShowGraph(bool isVisible)
    {
        foreach (Node node in nodes)
        {
            node.nodeObject.SetActive(isVisible);
        }
        foreach (Edge edge in edges)
        {
            edge.edgeObject.SetActive(isVisible);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(graphCenter.transform.position, new Vector3(spawnArea.x, spawnArea.y, 0));
    }
}

[System.Serializable]
public class Node
{
    public GameObject nodeObject;  
    public Vector3 position;  

    public Node(GameObject obj, Vector3 pos)
    {
        nodeObject = obj;
        position = pos;
    }
}

[System.Serializable]
public class Edge
{
    public Node startNode;
    public Node endNode;
    public GameObject edgeObject;

    public Edge(Node start, Node end, GameObject obj)
    {
        startNode = start;
        endNode = end;
        edgeObject = obj;
    }
}

