using System;
using UnityEngine;

namespace ColorfulHierarchy.Utilities
{
    [Serializable]
    public class CHIconData
    {
        [SerializeField] private string name;
        [SerializeField] private Texture2D texture;

        public Texture2D Texture => texture;
        public string Name => name;

        public CHIconData(Texture2D _texture, string _name)
        {
            texture = _texture;
            name = _name;
        }
    }
}
