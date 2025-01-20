using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private int amount;
        [SerializeField] private Vector2 spawnArea;
        [SerializeField] private List<GameObject> prefabs;
        [SerializeField] private bool spawnOnStart = true;

        private void Start()
        {
            if (spawnOnStart)
            {
                Spawn();
            }
        }

        private void Spawn()
        {
            var defaultPosition = transform.position;
            var rotation = transform.rotation;

            for (var i = 0; i < amount; i++)
            {
                // Generate a random position within the spawn area
                var localPosition = new Vector2(
                    Random.Range(-spawnArea.x, spawnArea.x),
                    Random.Range(-spawnArea.y, spawnArea.y)
                );

                // Rotate the local position by the transform's rotation
                var rotatedPosition = rotation * new Vector3(localPosition.x, localPosition.y, 0);
                var worldPosition = defaultPosition + rotatedPosition;

                // Instantiate the prefab
                Instantiate(
                    prefabs[Random.Range(0, prefabs.Count)],
                    worldPosition,
                    Quaternion.Euler(0, 0, Random.Range(0, 360)),
                    transform
                ).SetActive(true);
            }
        }

        public void SpawnAtIndex(int index)
        {
            if (index < 0 || index >= prefabs.Count)
            {
                Debug.LogError($"Invalid index: {index}. Must be between 0 and {prefabs.Count - 1}.");
                return;
            }

            var defaultPosition = transform.position;
            var rotation = transform.rotation;

            // Generate a random position within the spawn area
            var localPosition = new Vector2(
                Random.Range(-spawnArea.x, spawnArea.x),
                Random.Range(-spawnArea.y, spawnArea.y)
            );

            // Rotate the local position by the transform's rotation
            var rotatedPosition = rotation * new Vector3(localPosition.x, localPosition.y, 0);
            var worldPosition = defaultPosition + rotatedPosition;

            // Instantiate the prefab
            Instantiate(
                prefabs[index],
                worldPosition,
                Quaternion.Euler(0, 0, Random.Range(0, 360)),
                transform
            ).SetActive(true);
        }
        
        public void SpawnAtPosition()
        {
            Instantiate(
                prefabs[0],
                transform.position,
                Quaternion.identity,
                transform
            ).SetActive(true);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            // Draw the rotated spawn area rectangle
            var corners = new Vector3[4]
            {
                new Vector3(-spawnArea.x, -spawnArea.y, 0),
                new Vector3(spawnArea.x, -spawnArea.y, 0),
                new Vector3(spawnArea.x, spawnArea.y, 0),
                new Vector3(-spawnArea.x, spawnArea.y, 0)
            };

            var rotation = transform.rotation;

            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = rotation * corners[i] + transform.position;
            }

            Gizmos.DrawLine(corners[0], corners[1]);
            Gizmos.DrawLine(corners[1], corners[2]);
            Gizmos.DrawLine(corners[2], corners[3]);
            Gizmos.DrawLine(corners[3], corners[0]);
        }
    }
}
