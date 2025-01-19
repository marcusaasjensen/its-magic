using System;
using System.Linq;
using Data;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class RecipeManager : MonoBehaviour
    {
        [SerializeField] private RecipeData recipeData;
        [SerializeField] private UnityEvent onIngredientAdded;
        [SerializeField] private UnityEvent onRecipeComplete;

        public bool IsRecipeComplete { get; private set; }

        private void Start()
        {
            if (recipeData.currentIngredientsById == null || recipeData.currentIngredientsById.Length !=
                recipeData.requiredIngredientsById.Length)
            {
                recipeData.ResetIngredients();
            }
        }

        public void AddIngredient(string id)
        {
            var ingredientId = int.Parse(id) - 1;
            if (ingredientId < 0 || ingredientId >= recipeData.currentIngredientsById.Length)
            {
                return;
            }

            recipeData.currentIngredientsById[ingredientId]++;
            onIngredientAdded.Invoke();
            CheckRecipe();
        }

        public void CheckRecipe()
        {
            if (IsRecipeComplete)
            {
                return;
            }

            for (int i = 0; i < recipeData.currentIngredientsById.Length; i++)
            {
                if (recipeData.currentIngredientsById[i] < recipeData.requiredIngredientsById[i])
                {
                    return;
                }
            }

            Debug.Log("Recipe complete!");
            IsRecipeComplete = true;
            onRecipeComplete.Invoke();
        }
    }
}