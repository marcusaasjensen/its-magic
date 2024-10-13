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

            var triangles = new List<int>();

            for (var i = 1; i < knobs.Count - 1; i++)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);
            }

            _mesh.vertices = knobs.Select(t => t.transform.position).ToArray();
            _mesh.triangles = triangles.ToArray();
            _mesh.RecalculateNormals();

            DrawPolygon();
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
            _lineRenderer.positionCount = knobs.Count;

            for (var i = 0; i < knobs.Count; i++)
            {
                _lineRenderer.SetPosition(i, knobs[i].transform.position);
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
            if (knobs.Count < 3) return false;

            var polygonVertices = knobs.Select(k => (Vector2)k.transform.position).ToArray();
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
