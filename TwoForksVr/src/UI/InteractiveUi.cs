using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TwoForksVr.UI
{
    public class InteractiveUi : MonoBehaviour
    {
        private static Transform targetTransform;
        private BoxCollider collider;
        private vgUIInputModule[] inputModules = { };

        private void Start()
        {
            SetUpCollider();
            SetUpInputModules();
        }

        private void Update()
        {
            UpdateTransform();

            var active = IsAnyInputModuleActive();
            if (active && !collider.enabled)
                collider.enabled = true;
            else if (!active && collider.enabled) collider.enabled = false;
        }

        public static void SetTargetTransform(Transform transform)
        {
            targetTransform = transform;
        }

        private bool IsAnyInputModuleActive()
        {
            if (inputModules.Length == 0) return false;
            foreach (var inputModule in inputModules)
                if (inputModule && inputModule.gameObject.activeInHierarchy && inputModule.enabled)
                    return true;

            return false;
        }

        private void SetUpInputModules()
        {
            var rootObjects = gameObject.scene.GetRootGameObjects();
            foreach (var rootObject in rootObjects)
            {
                var modules = rootObject.GetComponentsInChildren<vgUIInputModule>(true);
                inputModules = inputModules.AddRangeToArray(modules);
            }
        }

        private void SetUpCollider()
        {
            collider = gameObject.GetComponent<BoxCollider>();
            if (collider != null) return;

            var rectTransform = gameObject.GetComponent<RectTransform>();
            collider = gameObject.gameObject.AddComponent<BoxCollider>();
            var rectSize = rectTransform.sizeDelta;
            collider.size = new Vector3(rectSize.x, rectSize.y, 0.1f);
            gameObject.layer = LayerMask.NameToLayer("UI");
        }

        private void UpdateTransform()
        {
            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
        }
    }
}