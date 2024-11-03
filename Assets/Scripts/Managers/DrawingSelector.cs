using System.Collections.Generic;
using UnityEngine;
using Environment;
using UI;
using Player;

public class DrawingSelector : MonoBehaviour
{
    public GameObject linePrefab;
    public LayerMask collectibleLayer;
    public Transform drawableManager;
    public FakeMenu fakeMenu;

    private GameObject _currentLine;
    private LineRenderer _lineRenderer;
    private EdgeCollider2D _edgeCollider;
    private List<Vector2> _touchPositions = new List<Vector2>();
    private const float SensitivityThreshold = 0.3f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClickDown();
        }
        else if (Input.GetMouseButton(0))
        {
            HandleMoving();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            HandleClickUp();
        }
    }

    private void HandleClickDown()
    {
        if (fakeMenu.IsDragging || TouchInput.Instance.Selection.IsSelecting) return;

        _currentLine = Instantiate(linePrefab, new Vector3(0, 0, -1), Quaternion.identity, transform);

        _lineRenderer = _currentLine.GetComponent<LineRenderer>();
        _edgeCollider = _currentLine.GetComponent<EdgeCollider2D>();

        // Définir le Sorting Layer sur "Foreground"
        _lineRenderer.sortingLayerName = "Foreground";

        // Définir le Sorting Order pour qu'il soit au premier plan
        _lineRenderer.sortingOrder = 10;

        Vector2 startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _touchPositions.Add(startPos);
        _touchPositions.Add(startPos);

        _lineRenderer.SetPosition(0, _touchPositions[0]);
        _lineRenderer.SetPosition(1, _touchPositions[1]);
        _edgeCollider.points = _touchPositions.ToArray();
    }

    private void HandleMoving()
    {
        if (fakeMenu.IsDragging || TouchInput.Instance.Selection.IsSelecting) return;

        Vector2 tempTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (CanCreateNewLine(tempTouchPos))
        {
            DrawNewLine(tempTouchPos);
        }
    }

    private void HandleClickUp()
    {
        if (fakeMenu.IsDragging || TouchInput.Instance.Selection.IsSelecting) return;

        _touchPositions.Add(_touchPositions[0]);
        _lineRenderer.positionCount++;
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, _touchPositions[0]);
        _edgeCollider.points = _touchPositions.ToArray();

        SelectCollectiblesInside();

        Invoke(nameof(DestroyLine), 1f);
    }

private void SelectCollectiblesInside()
{
    PolygonCollider2D polygon = _currentLine.AddComponent<PolygonCollider2D>();
    polygon.isTrigger = true;

    Vector2[] colliderPoints = new Vector2[_touchPositions.Count];
    _touchPositions.CopyTo(colliderPoints);
    polygon.points = colliderPoints;

    // Initialiser une liste pour les résultats d'OverlapCollider
    List<Collider2D> results = new List<Collider2D>();
    ContactFilter2D filter = new ContactFilter2D();
    filter.SetLayerMask(collectibleLayer);

    // Utiliser OverlapCollider pour détecter les objets dans le polygone
    int overlapCount = polygon.OverlapCollider(filter, results);
    foreach (Collider2D collectible in results)
    {
        collectible.GetComponent<Collectible>().Collect();
    }

    Destroy(polygon);
}


    private bool CanCreateNewLine(Vector2 tempTouchPos)
    {
        return Vector2.Distance(tempTouchPos, _touchPositions[_touchPositions.Count - 1]) > SensitivityThreshold;
    }

    private void DrawNewLine(Vector2 newFingerPos)
    {
        _touchPositions.Add(newFingerPos);
        _lineRenderer.positionCount++;
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, newFingerPos);
        _edgeCollider.points = _touchPositions.ToArray();
    }

    public bool IsPointInSelection(Vector2 point)
    {
        if (_touchPositions.Count > 1) return false;

        //Debug.Log($"Checking touch positions: {_touchPositions.Count}");

        return IsPointInPolygon(point, _touchPositions.ToArray());
    }

    private static bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
    {
        bool isInside = false;
        for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
        {
            if (((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
            {
                isInside = !isInside;
            }
        }

        if (isInside) Debug.Log("1");

        return isInside;
    }

    private void DestroyLine()
    {
        _touchPositions.Clear();
        Destroy(_currentLine);
    }
}
