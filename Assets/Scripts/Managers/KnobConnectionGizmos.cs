using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEngine;

namespace Managers
{
    public class KnobConnectionGizmos : MonoBehaviour
    {
        [SerializeField] private KnobPool knobPool;
        [SerializeField] private Material fillMaterial;
        [SerializeField] private Color color;
        
        private Mesh _mesh;
        private GameObject _fillPolygon;

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
        }

        private void Update()
        {
            if (knobPool.Knobs == null)
            {
                return;
            }

            var knobs = knobPool.Knobs.FindAll(knob => knob.IsVisible);

            if (knobs.Count < 3)
            {
                ClearPolygon();
                return;
            }

            GeneratePolygon(knobs);
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

        private void ClearPolygon()
        {
            if (_fillPolygon != null)
            {
                _fillPolygon.GetComponent<MeshFilter>().mesh = null;
            }
        }
    }
}
