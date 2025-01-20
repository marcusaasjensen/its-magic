using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "IngredientData", menuName = "ScriptableObjects/IngredientData", order = 1)]
    public class RecipeData : ScriptableObject
    {
        public int[] requiredIngredientsById;
        public int[] currentIngredientsById;

        public void ResetIngredients()
        {
            currentIngredientsById = new int[requiredIngredientsById.Length];
        }
    }
}