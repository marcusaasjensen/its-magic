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
    private Selectable _selectedObjectToDrag; // Track selected object
    private const float SensitivityThreshold = 0.3f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClickDown();
        }
        else if (Input.GetMouseButton(0))
        {
            if (_selectedObjectToDrag != null)
            {
                // Handle dragging if an object is selected
                HandleDragging();
            }
            else
            {
                HandleMoving();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (_selectedObjectToDrag != null)
            {
                // Stop dragging
                _selectedObjectToDrag = null; 
            }
            else
            {
                HandleClickUp();
            }
        }
    }

    private void HandleClickDown()
    {
        if (fakeMenu.IsDragging || TouchInput.Instance.Selection.IsSelecting) return;

        // Check if a Draggable object is under the mouse
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            var selectable = hit.collider.GetComponent<Selectable>();
            if (selectable != null)
            {
                // Start dragging if a Draggable object is found and exit to avoid starting a drawing
                _selectedObjectToDrag = selectable;
                return;
            }
        }

        // Start drawing only if no Draggable object was found
        StartDrawing(mousePosition);
    }

    private void StartDrawing(Vector2 startPos)
    {
        if (_touchPositions.Count > 0)
        {
            _touchPositions.Clear();
        }

        _currentLine = Instantiate(linePrefab, new Vector3(0, 0, -1), Quaternion.identity, transform);
        _lineRenderer = _currentLine.GetComponent<LineRenderer>();
        _edgeCollider = _currentLine.GetComponent<EdgeCollider2D>();

        _lineRenderer.sortingLayerName = "Foreground";
        _lineRenderer.sortingOrder = 10;

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

        Destroy(_currentLine, 1f);
    }

    private void SelectCollectiblesInside()
    {
        Collider2D[] collectibles = Physics2D.OverlapAreaAll(_touchPositions[0], _touchPositions[0], collectibleLayer);

        foreach (var collectible in collectibles)
        {
            Vector2 point = collectible.transform.position;
            if (IsPointInPolygon(point, _touchPositions.ToArray()))
            {
                var selectable = collectible.GetComponent<Selectable>();
                if (selectable != null)
                {
                    selectable.IsSelected = true; // Mark as selected
                }
            }
        }
    }

    private void HandleDragging()
    {
        // Move the selected object to follow the mouse
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _selectedObjectToDrag.transform.position = mousePosition;
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
        return IsPointInPolygon(point, _touchPositions.ToArray());
    }

    private static bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
    {
        int polygonLength = polygon.Length, i = 0;
        bool inside = false;

        for (int j = polygonLength - 1; i < polygonLength; j = i++)
        {
            if (((polygon[i].y <= point.y && point.y < polygon[j].y) || (polygon[j].y <= point.y && point.y < polygon[i].y)) &&
                (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
            {
                inside = !inside;
            }
        }
        return inside;
    }
}
