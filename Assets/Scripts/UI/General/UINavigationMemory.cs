    using UnityEngine;
    using UnityEngine.EventSystems;
    using System.Collections;

    public class UINavigationMemory : MonoBehaviour
    {
        [Header("UI Navigation Settings")]
        [SerializeField] private GameObject defaultFirstSelected;

        [Tooltip("If true, this UI remembers the last selected element when reopened.")]
        [SerializeField] private bool rememberLastPosition = true;

        private GameObject lastSelectedLocal;

        /// <summary>
        /// Called when this UI is activated.
        /// </summary>
        public void ActivateUI()
        {
            StartCoroutine(SelectAfterFrame());
        }

        private IEnumerator SelectAfterFrame()
        {
            yield return new WaitForSeconds(0.2f); ; // Wait 1 frame

            // Set toSelect to lastSelectedlocal if not null or else set to defaultFirstSelect
            GameObject toSelect = lastSelectedLocal ?? defaultFirstSelected;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(toSelect);
            Debug.Log("Set selected default");
        }

        /// <summary>
        /// Called when this UI is deactivated.
        /// </summary>
        public void DeactivateUI()
        {
            if (rememberLastPosition)
            {
                lastSelectedLocal = EventSystem.current.currentSelectedGameObject;
            }
            else
            {
                lastSelectedLocal = null; // Reset memory when closed
            }
        }

        public void ResetMemory()
        {
            lastSelectedLocal = null;
        }
    }
