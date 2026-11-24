using System.Collections.Generic;
using UnityEngine;
using CyberEscape.Obstacles;
using CyberEscape.Pickups;

namespace CyberEscape.Environment
{
    public class GroundTile : MonoBehaviour
    {
        [Header("Tile Dimensions")]
        public float tileLength = 30f;
        public float laneDistance = 3f;

        private readonly float[] lanePositions = new float[] { -3f, 0f, 3f };
        private List<GameObject> spawnedObjects = new List<GameObject>();

        /// <summary>
        /// Populates obstacles and pickups procedurally along this tile.
        /// </summary>
        public void SpawnContent(Material neonPinkMat, Material goldMat, Material neonBlueMat, Material neonGreenMat)
        {
            // Spawn 1 or 2 obstacle rows per tile (at z = +5 and z = +20 relative to tile center)
            SpawnObstacleRow(5f, neonPinkMat);
            SpawnObstacleRow(20f, neonPinkMat);

            // Spawn coins or power-ups in the open lanes
            SpawnPickups(goldMat, neonBlueMat, neonGreenMat);
        }

        private void SpawnObstacleRow(float localZ, Material mat)
        {
            // 70% chance to spawn an obstacle row on this z
            if (Random.value > 0.75f) return;

            // Pick 1 or 2 lanes to place obstacles (Leave at least 1 lane open!)
            int obstacleCount = Random.Range(1, 3); // 1 or 2
            List<int> availableLanes = new List<int> { 0, 1, 2 };

            for (int i = 0; i < obstacleCount; i++)
            {
                int laneIndex = availableLanes[Random.Range(0, availableLanes.Count)];
                availableLanes.Remove(laneIndex);

                float xPos = lanePositions[laneIndex];
                Vector3 spawnPos = transform.position + new Vector3(xPos, 0f, localZ);

                // Choose obstacle type
                int typeRoll = Random.Range(0, 4);
                GameObject obstacleObj = null;

                switch (typeRoll)
                {
                    case 0: // Low Barrier - Must jump
                        obstacleObj = CreateLowBarrier(spawnPos, mat);
                        break;
                    case 1: // High Barrier - Must slide
                        obstacleObj = CreateHighBarrier(spawnPos, mat);
                        break;
                    case 2: // Full Block - Must change lane
                        obstacleObj = CreateFullBlock(spawnPos, mat);
                        break;
                    case 3: // Moving Block
                        obstacleObj = CreateMovingBlock(spawnPos, mat);
                        break;
                }

                if (obstacleObj != null)
                {
                    obstacleObj.transform.SetParent(transform);
                    spawnedObjects.Add(obstacleObj);
                }
            }
        }

        private GameObject CreateLowBarrier(Vector3 position, Material mat)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = "Obstacle_LowBarrier";
            obj.tag = "Obstacle";
            obj.transform.position = position + new Vector3(0, 0.4f, 0);
            obj.transform.localScale = new Vector3(2.6f, 0.8f, 0.6f);
            if (mat != null) obj.GetComponent<MeshRenderer>().material = mat;

            Obstacle obs = obj.AddComponent<Obstacle>();
            obs.Initialize(ObstacleType.LowBarrier);
            return obj;
        }

        private GameObject CreateHighBarrier(Vector3 position, Material mat)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = "Obstacle_HighBarrier";
            obj.tag = "Obstacle";
            obj.transform.position = position + new Vector3(0, 2.3f, 0);
            obj.transform.localScale = new Vector3(2.6f, 1.2f, 0.6f);
            if (mat != null) obj.GetComponent<MeshRenderer>().material = mat;

            // Add side pillar legs for aesthetics
            GameObject leftLeg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            leftLeg.transform.SetParent(obj.transform);
            leftLeg.transform.localPosition = new Vector3(-0.45f, -0.9f, 0);
            leftLeg.transform.localScale = new Vector3(0.1f, 0.9f, 0.1f);
            Destroy(leftLeg.GetComponent<Collider>()); // visual only

            GameObject rightLeg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rightLeg.transform.SetParent(obj.transform);
            rightLeg.transform.localPosition = new Vector3(0.45f, -0.9f, 0);
            rightLeg.transform.localScale = new Vector3(0.1f, 0.9f, 0.1f);
            Destroy(rightLeg.GetComponent<Collider>()); // visual only

            Obstacle obs = obj.AddComponent<Obstacle>();
            obs.Initialize(ObstacleType.HighBarrier);
            return obj;
        }

        private GameObject CreateFullBlock(Vector3 position, Material mat)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = "Obstacle_FullBlock";
            obj.tag = "Obstacle";
            obj.transform.position = position + new Vector3(0, 1.25f, 0);
            obj.transform.localScale = new Vector3(2.6f, 2.5f, 1.5f);
            if (mat != null) obj.GetComponent<MeshRenderer>().material = mat;

            Obstacle obs = obj.AddComponent<Obstacle>();
            obs.Initialize(ObstacleType.FullBlock);
            return obj;
        }

        private GameObject CreateMovingBlock(Vector3 position, Material mat)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = "Obstacle_MovingBlock";
            obj.tag = "Obstacle";
            obj.transform.position = position + new Vector3(0, 1.25f, 0);
            obj.transform.localScale = new Vector3(2.2f, 2.5f, 1.2f);
            if (mat != null) obj.GetComponent<MeshRenderer>().material = mat;

            MovingObstacle moving = obj.AddComponent<MovingObstacle>();
            moving.Initialize(ObstacleType.MovingBlock);
            return obj;
        }

        private void SpawnPickups(Material goldMat, Material neonBlueMat, Material neonGreenMat)
        {
            // Pick a lane for a line of coins (or a power-up)
            int laneIndex = Random.Range(0, 3);
            float xPos = lanePositions[laneIndex];

            // 20% chance to spawn a Power-up, otherwise 3 coins in a line
            if (Random.value < 0.2f)
            {
                PickupType pType = (Random.value < 0.5f) ? PickupType.Shield :
                                   ((Random.value < 0.5f) ? PickupType.Magnet : PickupType.SlowMotion);

                Material pMat = (pType == PickupType.Shield) ? neonBlueMat : neonGreenMat;
                Vector3 pPos = transform.position + new Vector3(xPos, 1.2f, 12f);
                GameObject powerUp = CreatePickupObject(pPos, pType, pMat, PrimitiveType.Sphere);
                powerUp.transform.SetParent(transform);
                spawnedObjects.Add(powerUp);
            }
            else
            {
                // Spawn 3 coins in a row
                for (int c = 0; c < 3; c++)
                {
                    Vector3 coinPos = transform.position + new Vector3(xPos, 1f, 10f + c * 3f);
                    GameObject coin = CreatePickupObject(coinPos, PickupType.Coin, goldMat, PrimitiveType.Cylinder);
                    coin.transform.localScale = new Vector3(0.8f, 0.1f, 0.8f);
                    coin.transform.rotation = Quaternion.Euler(90f, 0, 0);
                    coin.transform.SetParent(transform);
                    spawnedObjects.Add(coin);
                }
            }
        }

        private GameObject CreatePickupObject(Vector3 position, PickupType type, Material mat, PrimitiveType primitive)
        {
            GameObject obj = GameObject.CreatePrimitive(primitive);
            obj.name = $"Pickup_{type}";
            obj.tag = "Pickup";
            obj.transform.position = position;
            obj.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            if (mat != null) obj.GetComponent<MeshRenderer>().material = mat;

            // Trigger collider
            Collider col = obj.GetComponent<Collider>();
            col.isTrigger = true;

            PickupItem pickup = obj.AddComponent<PickupItem>();
            pickup.Initialize(type);
            return obj;
        }
    }
}
