using Assimp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mg3d
{
    public class Mesh
    {
        public string Name { get; set; }
        public VertexPositionNormalTexture[] Vertices { get; set; }
        public Material Material { get; set; }
    }
}