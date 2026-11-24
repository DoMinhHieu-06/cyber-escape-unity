using System.Collections.Generic;
using UnityEngine;

namespace CyberEscape.Environment
{
    public class GroundLooper : MonoBehaviour
    {
        [Header("Tile Settings")]
        [SerializeField] private float tileLength = 30f;
        [SerializeField] private int initialTileCount = 6;
        [SerializeField] private float roadWidth = 10f;

        [Header("Materials")]
        [SerializeField] private Material roadMaterial;
        [SerializeField] private Material borderMaterial;
        [SerializeField] private Material obstacleMaterial;
        [SerializeField] private Material coinMaterial;
        [SerializeField] private Material shieldMaterial;
        [SerializeField] private Material powerUpMaterial;

        private Transform playerTransform;
        private List<GameObject> activeTiles = new List<GameObject>();
        private float nextSpawnZ = 0f;

        public void InitializeMaterials(Material road, Material border, Material obs, Material coin, Material shield, Material powerUp)
        {
            roadMaterial = road;
            borderMaterial = border;
            obstacleMaterial = obs;
            coinMaterial = coin;
            shieldMaterial = shield;
            powerUpMaterial = powerUp;
        }

        private void Start()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }

            // Spawn initial tiles
            for (int i = 0; i < initialTileCount; i++)
            {
                // First 2 tiles have no obstacles to let the player get ready
                SpawnTile(i >= 2);
            }
        }

        private void Update()
        {
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null) playerTransform = player.transform;
                return;
            }

            // If player is within distance of the end of the first tile, recycle and spawn next
            if (activeTiles.Count > 0 && playerTransform.position.z - 35f > activeTiles[0].transform.position.z)
            {
                Destroy(activeTiles[0]);
                activeTiles.RemoveAt(0);
                SpawnTile(true);
            }
        }

        public void SpawnTile(bool withObstacles)
        {
            GameObject tileObj = new GameObject($"GroundTile_{nextSpawnZ:F0}");
            tileObj.transform.position = new Vector3(0, 0, nextSpawnZ);
            tileObj.transform.SetParent(transform);

            // Ground plane / mesh
            GameObject roadMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roadMesh.name = "RoadMesh";
            roadMesh.tag = "Ground";
            roadMesh.transform.SetParent(tileObj.transform);
            roadMesh.transform.localPosition = new Vector3(0, -0.25f, tileLength / 2f);
            roadMesh.transform.localScale = new Vector3(roadWidth, 0.5f, tileLength);
            if (roadMaterial != null) roadMesh.GetComponent<MeshRenderer>().material = roadMaterial;

            // Border rails (neon borders for futuristic look)
            CreateBorderRail(tileObj.transform, -roadWidth / 2f, tileLength);
            CreateBorderRail(tileObj.transform, roadWidth / 2f, tileLength);

            // Add GroundTile component
            GroundTile groundTile = tileObj.AddComponent<GroundTile>();
            groundTile.tileLength = tileLength;

            if (withObstacles)
            {
                groundTile.SpawnContent(obstacleMaterial, coinMaterial, shieldMaterial, powerUpMaterial);
            }

            activeTiles.Add(tileObj);
            nextSpawnZ += tileLength;
        }

        private void CreateBorderRail(Transform parent, float xPos, float length)
        {
            GameObject rail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rail.name = "BorderRail";
            rail.transform.SetParent(parent);
            rail.transform.localPosition = new Vector3(xPos, 0.3f, length / 2f);
            rail.transform.localScale = new Vector3(0.5f, 0.6f, length);
            if (borderMaterial != null) rail.GetComponent<MeshRenderer>().material = borderMaterial;
        }
    }
}
