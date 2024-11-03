using System.Collections.Generic;
using UnityEngine;
using Managers;


namespace Environment
{
    public class NodeBehaviour : MonoBehaviour
    {
        public Node nodeData; 
        private Selectable _selectable; 

        private void Awake()
        {
            _selectable = GetComponent<Selectable>();

            if (nodeData == null)
            {
                nodeData = new Node(gameObject, transform.position);
            }
        }

        private void Update()
        {
            if (_selectable.IsSelected)
            {
                HighlightConnectedEdges();
            }
        }

        private void HighlightConnectedEdges()
        {
            foreach (Edge edge in nodeData.connectedEdges)
            {
                EdgeOpacityManager edgeOpacityManager = edge.edgeObject.GetComponent<EdgeOpacityManager>();
                if (edgeOpacityManager != null)
                {
                    edgeOpacityManager.HighlightEdge();
                }
            }
        }
    }
}