using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Environment;
using Player;

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
    private Vector3 _swipeStartPosition;

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

            NodeBehaviour nodeBehaviour = nodeObj.AddComponent<NodeBehaviour>();
            nodeBehaviour.nodeData = newNode;
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

            nodeA.connectedEdges.Add(newEdge);
            nodeB.connectedEdges.Add(newEdge);

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
        LineRenderer lineRenderer = edge.edgeObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = edge.edgeObject.AddComponent<LineRenderer>();
        }

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

    private float swipeSpeedThreshold = 5.0f;  // Adjust this to set the minimum swipe speed for a valid slash
    private float swipeStartTime;

    void DetectSlash()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Store the starting point and start time of the swipe
                _swipeStartPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0f));
                _swipeStartPosition.z = 0;
                swipeStartTime = Time.time;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                // Store the end point of the swipe
                Vector3 swipeEndPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0f));
                swipeEndPosition.z = 0;
                float swipeEndTime = Time.time;

                // Calculate swipe distance and duration
                float swipeDistance = Vector3.Distance(_swipeStartPosition, swipeEndPosition);
                float swipeDuration = swipeEndTime - swipeStartTime;
                float swipeSpeed = swipeDistance / swipeDuration;

                // Check if swipe speed meets the threshold for a "slash"
                if (swipeSpeed >= swipeSpeedThreshold)
                {
                    foreach (Edge edge in new List<Edge>(edges))
                    {
                        if (EdgeIntersectedBySwipe(edge, _swipeStartPosition, swipeEndPosition))
                        {
                            Destroy(edge.edgeObject);
                            edges.Remove(edge);
                            onEdgeRemoved.Invoke();
                        }
                    }
                }
            }
        }
    }



    bool EdgeIntersectedBySwipe(Edge edge, Vector3 swipeStart, Vector3 swipeEnd)
    {
        Vector3 edgeStart = edge.startNode.position;
        Vector3 edgeEnd = edge.endNode.position;

        return LinesIntersect(swipeStart, swipeEnd, edgeStart, edgeEnd);
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
            
            selectedNodeA.connectedEdges.Add(newEdge);
            selectedNodeB.connectedEdges.Add(newEdge);
            
            DrawEdge(newEdge);
            onEdgeCreated.Invoke();
        }

        selectedNodeA = null;
        selectedNodeB = null;
    }
    
    bool LinesIntersect(Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2)
    {
        float det = (p2.x - p1.x) * (q2.y - q1.y) - (p2.y - p1.y) * (q2.x - q1.x);
        if (Mathf.Abs(det) < 0.0001f) return false;  // Lines are parallel

        float lambda = ((q2.y - q1.y) * (q2.x - p1.x) + (q1.x - q2.x) * (q2.y - p1.y)) / det;
        float gamma = ((p1.y - p2.y) * (q2.x - p1.x) + (p2.x - p1.x) * (q2.y - p1.y)) / det;

        return (0 < lambda && lambda < 1) && (0 < gamma && gamma < 1);
    }

}


[System.Serializable]
public class Node
{
    public GameObject nodeObject;
    public Vector3 position;
    public List<Edge> connectedEdges;

    public Node(GameObject obj, Vector3 pos)
    {
        nodeObject = obj;
        position = pos;
        connectedEdges = new List<Edge>();
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

