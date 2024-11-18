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
        
        private void Start()
        {
            Spawn();
        }
        
        private void Spawn()
        {
            var defaultPosition = transform.position;
            for (var i = 0; i < amount; i++)
            {
                var position = new Vector2(Random.Range(-spawnArea.x, spawnArea.x) + defaultPosition.x, Random.Range(-spawnArea.y, spawnArea.y) + defaultPosition.y);
                Instantiate(prefabs[Random.Range(0, prefabs.Count - 1)], position, Quaternion.Euler(0,0, Random.Range(0, 360)), transform).SetActive(true);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector3(spawnArea.x * 2, spawnArea.y * 2, 0));
        }
    }
}