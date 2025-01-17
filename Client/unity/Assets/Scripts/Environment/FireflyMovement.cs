using UnityEngine;

public class FireflyMovement : MonoBehaviour
{
    public float speed = 2f; // Speed of the firefly
    public Vector2 areaMin = new Vector2(-5f, -5f); // Minimum bounds of the area
    public Vector2 areaMax = new Vector2(5f, 5f); // Maximum bounds of the area

    private Vector2 direction; // Current movement direction

    private void Start()
    {
        // Initialize with a random direction
        SetRandomDirection();
    }

    private void Update()
    {
        // Move the firefly in the current direction
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        // Keep the firefly within the bounds
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, areaMin.x, areaMax.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, areaMin.y, areaMax.y);

        // If the firefly reaches the boundary, pick a new random direction
        if (clampedPosition != transform.position)
        {
            transform.position = clampedPosition;
            SetRandomDirection();
        }
    }

    private void SetRandomDirection()
    {
        // Choose a random direction vector
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }
}