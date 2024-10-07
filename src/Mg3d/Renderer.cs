using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Mg3d
{
    public static class Renderer
    {
        public static void DrawNode(GraphicsDevice grDev, Effect effect, Node node)
        {
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                foreach (var mesh in node.Meshes)
                {
                    grDev.DrawUserPrimitives(PrimitiveType.TriangleList, mesh.Vertices, 0, mesh.Vertices.Length / 3);
                }
            }
        }
        public static void DrawNodeRecur(GraphicsDevice grDev, Effect implicitEffect, Dictionary<string, Effect> explicitEffects, Node node)
        {
            foreach (var mesh in node.Meshes)
            {
                if (mesh.Name == "Window")
                {
                    var i = 0;
                    ++i;
                }
                Effect effect = implicitEffect;
                bool explicitEffect = false;
                if (explicitEffects != null)
                {
                    if (explicitEffects.ContainsKey(mesh.Name))
                    {
                        effect = explicitEffects[mesh.Name];
                        explicitEffect = true;
                    }
                    else
                    {
                        effect.Parameters["FlatColor"].SetValue(mesh.Material.Vector3Props["FlatColor"]);
                    }
                }

                var textureSamplerName = "DiffuseTexture";
                Texture2D oldTex = null;
                if (mesh.Material.TextureEnabled && effect.Parameters[textureSamplerName] != null)
                {
                    oldTex = effect.Parameters[textureSamplerName].GetValueTexture2D();
                    if (mesh.Material.TextureSamplerProps != null)
                    {
                        if (mesh.Material.TextureSamplerProps.Exists(textureSamplerName))
                        {
                            effect.Parameters[textureSamplerName].SetValue(mesh.Material.TextureSamplerProps["DiffuseTexture"]);
                            effect.Parameters["FlatColor"].SetValue(new Vector3(0, 0, 0));
                        }
                    }
                }
                else
                {
                    if (!explicitEffect)
                    {
                        effect.Parameters[textureSamplerName].SetValue(oldTex);
                    }
                }
                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    grDev.DrawUserPrimitives(PrimitiveType.TriangleList, mesh.Vertices, 0, mesh.Vertices.Length / 3/*, VertexPositionNormalTexture.VertexDeclaration*/);
                }
                if (oldTex != null)
                {
                    
                }
            }
            foreach (var childNode in node.Children)
            {
                DrawNodeRecur(grDev, implicitEffect, explicitEffects, childNode);
            }
        }
    }
}