using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace UI
{
    public class SpotlightBoundaryHighlight : MonoBehaviour
    {
        public Light2D spotlight; // Reference to the Light2D
        public LineRenderer lineRenderer; // LineRenderer to show the highlights
        public ParticleSystem startParticleSystem; // Particle system for the start point
        public ParticleSystem endParticleSystem; // Particle system for the end point

        public Transform boundaryTransform;
        public Vector2 boundarySize = new(10, 10);
        
        private Vector2 _boundaryCenter = Vector2.zero;

        void Update()
        {
            _boundaryCenter = boundaryTransform.position;
            HighlightIntersection();
        }

        void OnDrawGizmos()
        {
            // Draw the boundary as a gizmo
            Gizmos.color = Color.green;
            Vector2 halfSize = boundarySize / 2f;
            Vector2 bottomLeft = _boundaryCenter - halfSize;
            Vector2 topLeft = _boundaryCenter + new Vector2(-halfSize.x, halfSize.y);
            Vector2 topRight = _boundaryCenter + halfSize;
            Vector2 bottomRight = _boundaryCenter + new Vector2(halfSize.x, -halfSize.y);

            Gizmos.DrawLine(bottomLeft, topLeft);
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
        }

        void HighlightIntersection()
        {
            if (!lineRenderer) return;

            Vector2 spotlightPos = spotlight.transform.position;
            float spotlightAngle = spotlight.pointLightOuterAngle / 2f;
            float spotlightRadius = spotlight.pointLightOuterRadius;

            // Get the square boundary corners
            Vector2[] corners = GetSquareCorners();

            // Store intersections with their associated edges
            List<Vector2> intersectionPoints = new List<Vector2>();

            // Detect intersections for each edge
            for (int i = 0; i < corners.Length; i++)
            {
                Vector2 start = corners[i];
                Vector2 end = corners[(i + 1) % corners.Length];

                if (TryGetIntersection(spotlightPos, spotlightRadius, spotlightAngle, start, end, out Vector2 segmentStart, out Vector2 segmentEnd))
                {
                    intersectionPoints.Add(segmentStart);
                    intersectionPoints.Add(segmentEnd);
                }
            }

            if (intersectionPoints.Count == 0)
            {
                lineRenderer.positionCount = 0;
                return;
            }

            // Start building the line from the first intersection
            List<Vector3> orderedPoints = new List<Vector3>();
            Vector2 currentPoint = intersectionPoints[0];
            orderedPoints.Add(currentPoint);

            // Remove the used point
            intersectionPoints.RemoveAt(0);

            // Connect the next closest points
            while (intersectionPoints.Count > 0)
            {
                int closestIndex = -1;
                float closestDistance = float.MaxValue;

                for (int i = 0; i < intersectionPoints.Count; i++)
                {
                    float distance = Vector2.Distance(currentPoint, intersectionPoints[i]);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestIndex = i;
                    }
                }

                if (closestIndex >= 0)
                {
                    currentPoint = intersectionPoints[closestIndex];
                    orderedPoints.Add(currentPoint);
                    intersectionPoints.RemoveAt(closestIndex);
                }
            }

            // Set the start and end points of the line renderer
            Vector3[] positions = orderedPoints.ToArray();
            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);

            // Set the particles at the start and end of the line renderer
            if (orderedPoints.Count > 0)
            {
                // Set the start particle system at the first point
                SetParticleSystemPosition(startParticleSystem, orderedPoints[0]);

                // Set the end particle system at the last point
                SetParticleSystemPosition(endParticleSystem, orderedPoints[orderedPoints.Count - 1]);
            }
        }

        void SetParticleSystemPosition(ParticleSystem particleSystem, Vector2 position)
        {
            if (particleSystem != null)
            {
                var main = particleSystem.main;
                particleSystem.transform.position = position;
                if (!particleSystem.isPlaying) particleSystem.Play();
            }
        }

        Vector2[] GetSquareCorners()
        {
            // Calculate the corners of the square boundary
            Vector2 halfSize = boundarySize / 2f;

            return new Vector2[]
            {
                _boundaryCenter + new Vector2(-halfSize.x, -halfSize.y), // Bottom-left
                _boundaryCenter + new Vector2(-halfSize.x, halfSize.y),  // Top-left
                _boundaryCenter + new Vector2(halfSize.x, halfSize.y),   // Top-right
                _boundaryCenter + new Vector2(halfSize.x, -halfSize.y)   // Bottom-right
            };
        }

        bool TryGetIntersection(Vector2 spotlightPos, float radius, float angle, Vector2 edgeStart, Vector2 edgeEnd, out Vector2 segmentStart, out Vector2 segmentEnd)
        {
            segmentStart = Vector2.zero;
            segmentEnd = Vector2.zero;

            // Generate rays at spotlight edges
            Vector2 spotlightDirection = spotlight.transform.right;
            Vector2 leftRay = RotateVector(spotlightDirection, -angle);
            Vector2 rightRay = RotateVector(spotlightDirection, angle);

            // Check intersections
            bool startInSpotlight = IsPointWithinSpotlight(spotlightPos, edgeStart, radius, angle);
            bool endInSpotlight = IsPointWithinSpotlight(spotlightPos, edgeEnd, radius, angle);

            List<Vector2> intersectionPoints = new List<Vector2>();

            // If either point is inside the spotlight
            if (startInSpotlight) intersectionPoints.Add(edgeStart);
            if (endInSpotlight) intersectionPoints.Add(edgeEnd);

            // Check intersection of edge with left and right spotlight rays
            if (RaySegmentIntersection(spotlightPos, leftRay, edgeStart, edgeEnd, out Vector2 leftIntersection))
                intersectionPoints.Add(leftIntersection);

            if (RaySegmentIntersection(spotlightPos, rightRay, edgeStart, edgeEnd, out Vector2 rightIntersection))
                intersectionPoints.Add(rightIntersection);

            // Sort intersections by distance from the spotlight
            intersectionPoints.Sort((a, b) => Vector2.Distance(spotlightPos, a).CompareTo(Vector2.Distance(spotlightPos, b)));

            // Define visible segment
            if (intersectionPoints.Count >= 2)
            {
                segmentStart = intersectionPoints[0];
                segmentEnd = intersectionPoints[1];
                return true;
            }

            return false;
        }

        bool IsPointWithinSpotlight(Vector2 spotlightPos, Vector2 point, float radius, float angle)
        {
            Vector2 direction = point - spotlightPos;
            float distance = direction.magnitude;

            if (distance > radius) return false;

            float angleToPoint = Vector2.Angle(spotlight.transform.right, direction);
            return angleToPoint <= angle;
        }

        bool RaySegmentIntersection(Vector2 rayOrigin, Vector2 rayDirection, Vector2 segmentStart, Vector2 segmentEnd, out Vector2 intersection)
        {
            intersection = Vector2.zero;

            // Parametric line intersection formula
            Vector2 v1 = rayOrigin - segmentStart;
            Vector2 v2 = segmentEnd - segmentStart;
            Vector2 v3 = new Vector2(-rayDirection.y, rayDirection.x);

            float dot = Vector2.Dot(v2, v3);
            if (Mathf.Abs(dot) < 0.0001f) return false; // Parallel lines

            float t1 = (v2.x * v1.y - v2.y * v1.x) / dot; // Cross product equivalent
            float t2 = Vector2.Dot(v1, v3) / dot;

            if (t1 >= 0 && t2 >= 0 && t2 <= 1)
            {
                intersection = rayOrigin + t1 * rayDirection;
                return true;
            }

            return false;
        }

        Vector2 RotateVector(Vector2 v, float angleDegrees)
        {
            float angleRadians = angleDegrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angleRadians);
            float sin = Mathf.Sin(angleRadians);

            return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
        }
    }
}
