using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Player;
using UnityEngine;

namespace Managers
{
    public class KnobSelection : MonoBehaviour
    {
        [SerializeField] private KnobPool knobPool;
        [SerializeField] private Material fillMaterial;
        [SerializeField] private Material lineMaterial;
        [SerializeField] private Color color;
        [SerializeField] private bool enableFill = true;
        [SerializeField] private bool enableLines = true;
        [SerializeField] private float maxDistance = 5f;
        
        private Mesh _mesh;
        private GameObject _fillPolygon;
        private LineRenderer _lineRenderer;

        private void Start()
        {
            _fillPolygon = new GameObject("FilledPolygon");
            var meshFilter = _fillPolygon.AddComponent<MeshFilter>();
            var meshRenderer = _fillPolygon.AddComponent<MeshRenderer>();
            meshRenderer.material = fillMaterial;

            var material = meshRenderer.material;
            material.shader = Shader.Find($"Unlit/Color");
            material.color = color;

            _fillPolygon.transform.SetParent(transform);
            _fillPolygon.transform.localPosition = new Vector3(0, 0, -0.1f);

            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.material = lineMaterial;
            _lineRenderer.startColor = color;
            _lineRenderer.endColor = color;
            _lineRenderer.positionCount = 0;
            _lineRenderer.loop = true;
            _lineRenderer.widthMultiplier = 0.05f;
        }

        private void Update()
        {
            if (knobPool.Knobs == null) return;

            var knobs = knobPool.Knobs.FindAll(knob => knob.IsVisible);

            if (knobs.Count < 2)
            {
                ClearPolygon();
                ClearLines();
                return;
            }

            if (enableFill && knobs.Count >= 3)
            {
                GeneratePolygon(knobs);
            }
            else
            {
                ClearPolygon();
            }

            if (enableLines)
            {
                DrawLines(knobs);
            }
            else
            {
                ClearLines();
            }
        }

        private void GeneratePolygon(List<Knob> knobs)
        {
            if (_mesh == null)
            {
                _mesh = new Mesh();
            }
            else
            {
                _mesh.Clear();
            }

            var vertices = new List<Vector3>();
            var triangles = new List<int>();

            vertices.Add(knobs[0].transform.position);

            for (var i = 1; i < knobs.Count; i++)
            {
                if (Vector3.Distance(knobs[i - 1].transform.position, knobs[i].transform.position) <= maxDistance)
                {
                    vertices.Add(knobs[i].transform.position);

                    if (vertices.Count > 2)
                    {
                        triangles.Add(0);
                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 1);
                    }
                }
            }

            if (vertices.Count >= 3)
            {
                _mesh.vertices = vertices.ToArray();
                _mesh.triangles = triangles.ToArray();
                _mesh.RecalculateNormals();

                DrawPolygon();
            }
        }

        private void DrawPolygon()
        {
            if (_fillPolygon != null)
            {
                _fillPolygon.GetComponent<MeshFilter>().mesh = _mesh;
            }
        }

        private void DrawLines(List<Knob> knobs)
        {
            var linePositions = new List<Vector3>();

            for (var i = 0; i < knobs.Count; i++)
            {
                if (i > 0 && Vector3.Distance(knobs[i - 1].transform.position, knobs[i].transform.position) > maxDistance)
                {
                    continue;
                }

                linePositions.Add(knobs[i].transform.position);
            }

            _lineRenderer.positionCount = linePositions.Count;

            for (var i = 0; i < linePositions.Count; i++)
            {
                _lineRenderer.SetPosition(i, linePositions[i]);
            }
        }

        private void ClearPolygon()
        {
            if (_fillPolygon != null)
            {
                _fillPolygon.GetComponent<MeshFilter>().mesh = null;
            }
        }

        private void ClearLines()
        {
            if (_lineRenderer != null)
            {
                _lineRenderer.positionCount = 0;
            }
        }
        
        public bool IsPointInSelection(Vector2 point)
        {
            var knobs = knobPool.Knobs.FindAll(knob => knob.IsVisible);
            if(knobs.Count == 0) return false;
            var validKnobs = new List<Knob> { knobs[0] };

            for (var i = 1; i < knobs.Count; i++)
            {
                if (Vector3.Distance(knobs[i - 1].transform.position, knobs[i].transform.position) <= maxDistance)
                {
                    validKnobs.Add(knobs[i]);
                }
            }

            if (validKnobs.Count < 3) return false;

            var polygonVertices = validKnobs.Select(k => (Vector2)k.transform.position).ToArray();
            return IsPointInPolygon(point, polygonVertices);
        }

        private static bool IsPointInPolygon(Vector2 point, [NotNull] Vector2[] polygon)
        {
            if (polygon == null) throw new ArgumentNullException(nameof(polygon));
            var isInside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                    (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }
    }
}
