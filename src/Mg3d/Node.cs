using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Mg3d
{
    public class Node
    {
        public Matrix Xform { get; set; } = Matrix.Identity;
        public List<Node> Children { get; set; } = new List<Node>();
        public List<Mesh> Meshes { get; set; } = new List<Mesh>();
    }
}