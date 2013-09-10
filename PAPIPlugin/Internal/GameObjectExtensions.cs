#region Usings

using UnityEngine;

#endregion

namespace PAPIPlugin.Internal
{
    public static class GameObjectExtensions
    {
        public static void SetLayerRecursively(this GameObject sGameObject, int newLayerNumber)
        {
            // Only set to layer 'newLayerNumber' if the collider is not a trigger.
            if ((sGameObject.collider != null && sGameObject.collider.enabled && !sGameObject.collider.isTrigger) || sGameObject.collider == null)
            {
                sGameObject.layer = newLayerNumber;
            }

            foreach (Transform child in sGameObject.transform)
            {
                SetLayerRecursively(child.gameObject, newLayerNumber);
            }
        }
    }
}
