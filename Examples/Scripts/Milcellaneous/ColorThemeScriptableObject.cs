using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Theme", menuName = "Tesla API/Color Theme", order = 1)]
public class ColorThemeScriptableObject : ScriptableObject {

    public string Name;

    public Color Background;

    public List<Color> Colors;
}
