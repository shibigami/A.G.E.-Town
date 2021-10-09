using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Recipe", menuName = "LPGK/Recipe")]
public class Recipe : ScriptableObject
{
    public List<RecipeItem> Items;

    public RecipeItem Result;
}
