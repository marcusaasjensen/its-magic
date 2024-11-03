using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GraphManager : MonoBehaviour
{
    public GameObject nodePrefab;
    public GameObject edgePrefab;

    [SerializeField]
    public GameObject molecularItem;
    [SerializeField]
    private GameObject graphCenter;
    [SerializeField] private Color edgesColor;

    [SerializeField] private Vector2 spawnArea = new Vector2(10f, 10f);
    [SerializeField] private Transform parent;
    
    [SerializeField] private UnityEvent onEdgeCreated;
    [SerializeField] private UnityEvent onEdgeRemoved;

    private SpriteRenderer _spriteRenderer;

    private List<Node> nodes = new List<Node>();
    private List<Edge> edges = new List<Edge>();
    private bool isGraphGenerated = false;

    private Node selectedNodeA = null;
    private Node selectedNodeB = null;
    
    private bool _isVisible = false;

    private void Awake()
    {
        _spriteRenderer = molecularItem.GetComponent<SpriteRenderer>();
        GenerateGraph(5, 10);
    }

    private void Start()
    {
        ShowGraph(false);
    }

    void Update()
    {
        if (_spriteRenderer.sprite != null)
        {
            if (!isGraphGenerated)
            {
                ShowGraph(true);
                isGraphGenerated = true;
            }
            UpdateEdges();
        }
        else
        {
            if (isGraphGenerated)
            {
                ShowGraph(false);
                isGraphGenerated = false;
            }
        }
        
        if(!_isVisible)
        {
            return;
        }

        DetectSlash();
        HandleNodeSelection();
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
                Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
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
                GameObject edgeObj = Instantiate(edgePrefab, parent.transform.position, Quaternion.identity, parent);
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
        lineRenderer.startColor = edgesColor;
        lineRenderer.endColor = edgesColor;
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
        
        _isVisible = isVisible;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(graphCenter.transform.position, new Vector3(spawnArea.x, spawnArea.y, 0));
    }

    void UpdateEdges()
    {
        foreach (Edge edge in edges)
        {
            edge.startNode.position = edge.startNode.nodeObject.transform.position;
            edge.endNode.position = edge.endNode.nodeObject.transform.position;

            LineRenderer lineRenderer = edge.edgeObject.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, edge.startNode.position);
            lineRenderer.SetPosition(1, edge.endNode.position);
        }
    }

    // Detects slashes and removes edges if intersected by swipe
    void DetectSlash()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0f));
                touchPosition.z = 0;

                foreach (Edge edge in new List<Edge>(edges))
                {
                    if (EdgeIntersectedBySwipe(edge, touchPosition))
                    {
                        Destroy(edge.edgeObject);
                        edges.Remove(edge);
                        onEdgeRemoved.Invoke();
                    }
                }
            }
        }
    }

    // Checks if an edge is intersected by the swipe
    bool EdgeIntersectedBySwipe(Edge edge, Vector3 swipePosition)
    {
        float distanceToEdge = Vector2.Distance(edge.startNode.position, edge.endNode.position);
        float distanceToStart = Vector2.Distance(edge.startNode.position, swipePosition);
        float distanceToEnd = Vector2.Distance(edge.endNode.position, swipePosition);

        return distanceToStart + distanceToEnd <= distanceToEdge + 0.2f;  // Adjust threshold as needed
    }

    // Handles node selection to create new edges by linking nodes
    void HandleNodeSelection()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            touchPosition.z = 0;

            Node touchedNode = nodes.Find(n => Vector2.Distance(n.position, touchPosition) < 0.5f);

            if (touchedNode != null)
            {
                if (selectedNodeA == null)
                {
                    selectedNodeA = touchedNode;
                }
                else if (selectedNodeB == null && touchedNode != selectedNodeA)
                {
                    selectedNodeB = touchedNode;
                    CreateEdgeBetweenSelectedNodes();
                }
            }
        }
    }

    // Creates a new edge between selected nodes if not already connected
    void CreateEdgeBetweenSelectedNodes()
    {
        if (selectedNodeA != null && selectedNodeB != null && !AreNodesConnected(selectedNodeA, selectedNodeB))
        {
            GameObject edgeObj = Instantiate(edgePrefab, parent.transform.position, Quaternion.identity, parent);
            Edge newEdge = new Edge(selectedNodeA, selectedNodeB, edgeObj);
            edges.Add(newEdge);
            DrawEdge(newEdge);
            onEdgeCreated.Invoke();
        }

        // Reset node selections
        selectedNodeA = null;
        selectedNodeB = null;
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

