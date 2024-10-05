using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mg3d
{
    public class Material
    {
        public class PropMap<T>
        {
            private Dictionary<string, T> dict = new Dictionary<string, T>();
            public T this[string key]
            {
                get => dict[key];
                set => dict[key] = value;
            }
            public bool Exists(string key) => dict.ContainsKey(key);
        }
        public PropMap<Vector2> Vector2Props;
        public PropMap<Vector3> Vector3Props;
        public PropMap<Vector4> Vector4Props;
        public PropMap<Matrix> Matrix4x4Props;
        public PropMap<Texture2D> TextureSamplerProps;
        public bool TextureEnabled { get; set; } = false;
    }
}