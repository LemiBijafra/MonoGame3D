using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Assimp;
using System.Diagnostics;
using System.IO;

namespace Mg3d
{
    public static class FromAssimp
    {
        public static Vector4 ConvertColor4D(Color4D assimpColor)
        {
            return new Vector4(assimpColor.R, assimpColor.G, assimpColor.B, assimpColor.A);
        }
        public static Mesh ConvertMesh(GraphicsDevice grDev, Assimp.Scene assimpScene, Assimp.Mesh assimpMesh)
        {
            var mesh = new Mesh
            {
                Name = assimpMesh.Name,
                Vertices = new VertexPositionNormalTexture[assimpMesh.VertexCount],
                Material = new Material()
            };

            mesh.Material.Vector4Props ??= new Material.PropMap<Vector4>()
            {
                ["DiffuseColor"] = ConvertColor4D(assimpScene.Materials[assimpMesh.MaterialIndex].ColorDiffuse)
            };

            var foo = 0;
            if (mesh.Name.Contains("Cube"))
            {
                ++foo;
            }

            var assimpMat = assimpScene.Materials[assimpMesh.MaterialIndex];
            if (assimpMat.Name == "Material.002")
            {
                ++foo;
            }
            if (assimpMat.HasTextureDiffuse)
            {
                mesh.Material.TextureSamplerProps ??= new();
                var texPath = $"../../../Content/{assimpMat.TextureDiffuse.FilePath}";
                Texture2D texture = null;
                if (File.Exists(texPath))
                {
                    texture = Texture2D.FromFile(grDev, texPath);
                }
                if (texture != null)
                {
                    mesh.Material.TextureSamplerProps["DiffuseTexture"] = texture;
                    mesh.Material.TextureEnabled = true;
                }
                else
                {
                    mesh.Material.TextureEnabled = false;
                }
            }

            var i = 0;
            foreach (var vert in assimpMesh.Vertices)
            {
                var uV = new Vector2(1, 1);
                if (assimpMesh.TextureCoordinateChannelCount > 0)
                {
                    uV = new Vector2(assimpMesh.TextureCoordinateChannels[0][i].X, assimpMesh.TextureCoordinateChannels[0][i].Y);
                    //!!!Debug.WriteLine($"{uV.X} {uV.Y}"); // hangs the VS
                }
                Vector3 normal;
                if (assimpMesh.HasNormals)
                {
                    normal = new Vector3(assimpMesh.Normals[i].X, assimpMesh.Normals[i].Y, assimpMesh.Normals[i].Z);
                }
                else
                {
                    normal = new Vector3();
                }
                mesh.Vertices[i] = new VertexPositionNormalTexture(new Vector3(vert.X, vert.Y, vert.Z), normal, uV);
                ++i;
            }
            return mesh;
        }

        public static Matrix ConvertMatrix(Matrix4x4 aM /* Assimp matrix */)
        {
            return new Matrix(aM.A1, aM.A2, aM.A3, aM.A4, aM.B1, aM.B2, aM.B3, aM.B4, aM.C1, aM.C2, aM.C3, aM.C4, aM.D1, aM.D2, aM.D3, aM.D4);
        }

        public static Node ConvertNodeTree(GraphicsDevice grDev, Assimp.Scene assimpScene, Assimp.Node assimpNode, Node parent)
        {
            var node = new Node();
            parent?.Children.Add(node);
            node.Xform = ConvertMatrix(assimpNode.Transform);
            foreach (var meshIdx in assimpNode.MeshIndices)
            {
                node.Meshes.Add(ConvertMesh(grDev, assimpScene, assimpScene.Meshes[meshIdx]));
            }
            foreach (var childNode in assimpNode.Children)
            {
                ConvertNodeTree(grDev, assimpScene, childNode, node);
            }
            return node;
        }
    }
}