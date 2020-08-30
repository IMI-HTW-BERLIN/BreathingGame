using System.Collections.Generic;
using LevelObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Small UI that shows the player what to collect and where to go.
    /// </summary>
    public class LevelGoalScreen : MonoBehaviour
    {
        [SerializeField] private Transform hlgCrystalsToCollect;
        [SerializeField] private GameObject crystalPrefab;

        /// <summary>
        /// Removes all crystal images. Not needed anymore.
        /// </summary>
        private void OnDisable()
        {
            foreach (Transform child in hlgCrystalsToCollect)
                Destroy(child.gameObject);
        }

        /// <summary>
        /// Shows the player all crystals that he needs to collect.
        /// </summary>
        public void ShowCrystalsToCollect(IEnumerable<Crystal> listOfCrystals)
        {
            foreach (Crystal crystal in listOfCrystals)
                Instantiate(crystalPrefab, hlgCrystalsToCollect).GetComponent<Image>().sprite = crystal.CrystalSprite;
        }
    }
}