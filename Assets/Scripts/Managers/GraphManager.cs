using UnityEngine;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour
{
    public GameObject nodePrefab;  // Prédéfini dans Unity pour représenter un nœud
    public GameObject edgePrefab;  // Prédéfini dans Unity pour représenter une arête (ligne ou autre)

    [SerializeField]
    public GameObject molecularItem; // Item qui déclenche la génération du graphe
    [SerializeField]
    private GameObject graphCenter; // Référence au GameObject vide pour le centre du graphe

    // Zone de génération des nœuds
    [SerializeField] private Vector2 spawnArea = new Vector2(10f, 10f); // Largeur et hauteur de la zone

    private SpriteRenderer _spriteRenderer; // Référence au SpriteRenderer

    private List<Node> nodes = new List<Node>();  // Liste des nœuds
    private List<Edge> edges = new List<Edge>();  // Liste des arêtes
    private bool isGraphGenerated = false;  // Indicateur si le graphe a été généré

    private void Awake()
    {
        _spriteRenderer = molecularItem.GetComponent<SpriteRenderer>();
        // Générer le graphe une seule fois au démarrage
        GenerateGraph(5, 10);
        ShowGraph(false);
    }

    void Update()
    {
        Debug.Log("isGraphGenerated :" + isGraphGenerated);

        if (_spriteRenderer.sprite != null)  // Vérifie si le SpriteRenderer a un sprite
        {
            if (!isGraphGenerated)  // Si le graphe n'est pas encore affiché
            {
                ShowGraph(true);  // Affiche le graphe
                isGraphGenerated = true;  // Met à jour l'indicateur
                Debug.Log("Graphe affiché.");
            }
        }
        else
        {
            if (isGraphGenerated)  // Si le graphe est affiché
            {
                ShowGraph(false);  // Masque le graphe
                isGraphGenerated = false;  // Met à jour l'indicateur
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

            // Vérifie si les deux nœuds sont déjà connectés
            if (nodeA != nodeB && !AreNodesConnected(nodeA, nodeB))
            {
                GameObject edgeObj = Instantiate(edgePrefab);
                Edge newEdge = new Edge(nodeA, nodeB, edgeObj);
                edges.Add(newEdge);
                
                // Visualise l'arête (ligne entre les nœuds)
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
        lineRenderer.startWidth = 0.1f; // Ajuste la largeur
        lineRenderer.endWidth = 0.1f; // Ajuste la largeur

        // Matériau pour le LineRenderer
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red; // Couleur de l'arête
        lineRenderer.endColor = Color.red; // Couleur de l'arête
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
    public GameObject nodeObject;  // GameObject du nœud
    public Vector3 position;       // Position du nœud

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
