using System;
using UnityEngine;

namespace WorldGeneration
{
    public class VoxelMap : MonoBehaviour
    {
        public float size = 2.0f;

        public int voxelResolution = 8;
        public int chunkResolution = 2;

        public VoxelGrid voxelGridPrefab;

        private VoxelGrid[] chunks;
        private float chunkSize, voxelSize, halfSize;
        
        private void Awake()
        {
            halfSize = size * 0.5f;
            chunkSize = size / chunkResolution;
            voxelSize = chunkSize / voxelResolution;

            chunks = new VoxelGrid[chunkResolution * chunkResolution];

            for (int i = 0, y = 0; y < chunkResolution; y++)
            {
                for (int x = 0; x < chunkResolution; x++, i++)
                {
                    CreateChunk(i, x, y);
                }
            }
            
            var box = gameObject.AddComponent<BoxCollider>();
            box.size = new Vector3(size, 0.0f, size);
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        EditVoxels(transform.InverseTransformPoint(hit.point));
                    }
                }
            }
        }

        private static string[] fillTypeNames = {"Filled", "Empty"};
        private static string[] radiusNames = {"0", "1", "2", "3", "4", "5"};
        private int fillTypeIndex, radiusIndex;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(4.0f, 4.0f, 150f, 500f));
            GUILayout.Label("Fill Type");
            fillTypeIndex = GUILayout.SelectionGrid(fillTypeIndex, fillTypeNames, 2);
            GUILayout.Label("Radius");
            radiusIndex = GUILayout.SelectionGrid(radiusIndex, radiusNames, 6);
            GUILayout.EndArea();
        }

        private void EditVoxels (Vector3 point) {
            int centerX = (int)((point.x + halfSize) / voxelSize);
            int centerY = (int)((point.z + halfSize) / voxelSize);

            int xStart = (centerX - radiusIndex - 1) / voxelResolution;
            if (xStart < 0) {
                xStart = 0;
            }
            int xEnd = (centerX + radiusIndex) / voxelResolution;
            if (xEnd >= chunkResolution) {
                xEnd = chunkResolution - 1;
            }
            int yStart = (centerY - radiusIndex - 1) / voxelResolution;
            if (yStart < 0) {
                yStart = 0;
            }
            int yEnd = (centerY + radiusIndex) / voxelResolution;
            if (yEnd >= chunkResolution) {
                yEnd = chunkResolution - 1;
            }

            VoxelStencil activeStencil = new VoxelStencil();
            activeStencil.Initialize(fillTypeIndex == 0, radiusIndex);

            int voxelYOffset = yEnd * voxelResolution;
            for (int y = yEnd; y >= yStart; y--) {
                int i = y * chunkResolution + xEnd;
                int voxelXOffset = xEnd * voxelResolution;
                for (int x = xEnd; x >= xStart; x--, i--) {
                    activeStencil.SetCenter(centerX - voxelXOffset, centerY - voxelYOffset);
                    chunks[i].Apply(activeStencil);
                    voxelXOffset -= voxelResolution;
                }
                voxelYOffset -= voxelResolution;
            }
        }

        private void CreateChunk(int i, int x, int y)
        {
            var chunk = Instantiate(voxelGridPrefab, transform);
            chunk.Initialize(voxelResolution, chunkSize);
            chunk.transform.localPosition = new Vector3(x * chunkSize - halfSize, 0.0f, y * chunkSize - halfSize);
            chunks[i] = chunk;
            
            if (x > 0) {
                chunks[i - 1].xNeighbor = chunk;
            }
            if (y > 0) {
                chunks[i - chunkResolution].yNeighbor = chunk;
                if (x > 0) {
                    chunks[i - chunkResolution - 1].xyNeighbor = chunk;
                }
            }
        }
    }
}