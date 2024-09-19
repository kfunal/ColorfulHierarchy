using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ColorfulHierarchy.Utilities
{
    [CreateAssetMenu(fileName = "Colorful Hierarchy Palette", menuName = "Colorful Hierarchy/Palette")]
    public class CHPalette : ScriptableObject
    {
        [SerializeField] private List<CHIconData> icons;
        [SerializeField] private List<CHIconData> defaultIcons;
        [SerializeField] private List<ColorArrayWrapper> gradients;

        public List<CHIconData> Icons => icons;
        public List<ColorArrayWrapper> Gradients => gradients;

        public void AddGradient(List<Color> _colors)
        {
            if (gradients.Any(x => x.SameGradient(_colors)))
                return;

            gradients.Add(new ColorArrayWrapper(_colors));
        }

        public void AddIcons(List<Texture2D> _icons)
        {
            if (_icons == null || _icons.Count == 0) return;

            for (int i = 0; i < _icons.Count; i++)
            {
                if (_icons[i] == null) continue;
                icons.Add(new CHIconData(_icons[i], _icons[i].name));
            }
        }

        public Texture2D GetIcon(string _iconName)
        {
            if (icons == null) return null;
            if (icons.Count == 0) return null;

            for (int i = 0; i < icons.Count; i++)
            {
                if (icons[i].Name == _iconName)
                    return icons[i].Texture;
            }

            return null;
        }

        public void RemoveColor(int _index)
        {
            if (gradients == null) return;
            gradients.RemoveAt(_index);
        }
    }
}

[System.Serializable]
public class ColorArrayWrapper
{
    [SerializeField] private List<Color> colors;

    public List<Color> Colors
    {
        get { return colors; }
        set { colors = value; }
    }

    public ColorArrayWrapper(List<Color> _colors)
    {
        colors = _colors;
    }

    public bool SameGradient(List<Color> _colors)
    {
        if (colors == null || colors.Count == 0)
            return false;

        if (_colors == null || _colors.Count == 0)
            return false;

        return colors.SequenceEqual(_colors);
    }
}
