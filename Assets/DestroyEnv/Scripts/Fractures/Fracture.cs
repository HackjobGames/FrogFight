using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Utils;
using UnityEngine;

namespace Project.Scripts.Fractures
{
    public static class Fracture
    {
        public static void FractureGameObject(GameObject gameObject, int seed, int totalChunks, Material insideMaterial, Material outsideMaterial, float density, GameObject effect)
        {
            // Translate all meshes to one world mesh
            var mesh = GetWorldMesh(gameObject);
            
            NvBlastExtUnity.setSeed(seed);

            var nvMesh = new NvMesh(
                mesh.vertices,
                mesh.normals,
                mesh.uv,
                mesh.vertexCount,
                mesh.GetIndices(0),
                (int) mesh.GetIndexCount(0)
            );

            var meshes = FractureMeshesInNvblast(totalChunks, nvMesh);

            // Build chunks gameobjects
            var chunkMass = mesh.Volume() * density / totalChunks;
            var chunks = BuildChunks(insideMaterial, outsideMaterial, meshes, chunkMass);
            foreach(GameObject chunk in chunks) {
              chunk.transform.SetParent(gameObject.transform);
              chunk.layer = 6;
              chunk.tag = "Terrain";
              chunk.AddComponent<DestructableObject>().effect = effect;
              chunk.GetComponent<DestructableObject>().center = chunk.GetComponent<MeshFilter>().sharedMesh.bounds.center;
            }
        }


        private static List<GameObject> BuildChunks(Material insideMaterial, Material outsideMaterial, List<Mesh> meshes, float chunkMass)
        {
            return meshes.Select((chunkMesh, i) =>
            {
                var chunk = BuildChunk(insideMaterial, outsideMaterial, chunkMesh, chunkMass);
                chunk.name += $" [{i}]";
                return chunk;
            }).ToList();
        }

        private static List<Mesh> FractureMeshesInNvblast(int totalChunks, NvMesh nvMesh)
        {
            var fractureTool = new NvFractureTool();
            fractureTool.setRemoveIslands(false);
            fractureTool.setSourceMesh(nvMesh);
            var sites = new NvVoronoiSitesGenerator(nvMesh);
            sites.uniformlyGenerateSitesInMesh(totalChunks);
            fractureTool.voronoiFracturing(0, sites);
            fractureTool.finalizeFracturing();

            // Extract meshes
            var meshCount = fractureTool.getChunkCount();
            var meshes = new List<Mesh>(fractureTool.getChunkCount());
            for (var i = 1; i < meshCount; i++)
            {
                meshes.Add(ExtractChunkMesh(fractureTool, i));
            }

            return meshes;
        }

        private static Mesh ExtractChunkMesh(NvFractureTool fractureTool, int index)
        {
            var outside = fractureTool.getChunkMesh(index, false);
            var inside = fractureTool.getChunkMesh(index, true);
            var chunkMesh = outside.toUnityMesh();
            chunkMesh.subMeshCount = 2;
            chunkMesh.SetIndices(inside.getIndexes(), MeshTopology.Triangles, 1);
            return chunkMesh;
        }

        private static Mesh GetWorldMesh(GameObject gameObject)
        {
            var combineInstances = gameObject
                .GetComponentsInChildren<MeshFilter>()
                .Where(mf => ValidateMesh(mf.mesh))
                .Select(mf => new CombineInstance()
                {
                    mesh = mf.mesh,
                    transform = mf.transform.localToWorldMatrix
                }).ToArray();
            
            var totalMesh = new Mesh();
            totalMesh.CombineMeshes(combineInstances, true);
            return totalMesh;
        }
        
        private static bool ValidateMesh(Mesh mesh)
        {
            if (mesh.isReadable == false)
            {
                Debug.LogError($"Mesh [{mesh}] has to be readable.");
                return false;
            }
            
            if (mesh.vertices == null || mesh.vertices.Length == 0)
            {
                Debug.LogError($"Mesh [{mesh}] does not have any vertices.");
                return false;
            }
            
            if (mesh.uv == null || mesh.uv.Length == 0)
            {
                Debug.LogError($"Mesh [{mesh}] does not have any uvs.");
                return false;
            }

            return true;
        }

        private static GameObject BuildChunk(Material insideMaterial, Material outsideMaterial, Mesh mesh, float mass)
        {
            var chunk = new GameObject($"Chunk");
            
            var renderer = chunk.AddComponent<MeshRenderer>();
            renderer.sharedMaterials = new[]
            {
                outsideMaterial,
                insideMaterial
            };

            var meshFilter = chunk.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;

            // var rigibody = chunk.AddComponent<Rigidbody>();
            // rigibody.mass = mass;
            // rigibody.isKinematic = true;

            var mc = chunk.AddComponent<MeshCollider>();
            mc.inflateMesh = true;
            mc.convex = true;

            return chunk;
        }
    }
}