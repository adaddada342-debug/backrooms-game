using Backrooms.Loading;
using UnityEngine;

namespace Backrooms.Transitions
{
    [RequireComponent(typeof(Collider))]
    public class LevelTransitionTrigger : MonoBehaviour
    {
        [SerializeField]
        private string targetLevelId;

        [SerializeField]
        private string targetPackageId;

        [SerializeField]
        private string transitionType;

        [SerializeField]
        private bool requirePlayerTag = true;

        [SerializeField]
        private string playerTag = "Player";

        [SerializeField]
        private LevelLoader levelLoader = null;

        private bool triggered;

        public void Configure(
            string newTargetLevelId,
            string newTargetPackageId,
            string newTransitionType,
            LevelLoader newLevelLoader)
        {
            targetLevelId = newTargetLevelId;
            targetPackageId = newTargetPackageId;
            transitionType = newTransitionType;
            levelLoader = newLevelLoader;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggered)
            {
                return;
            }

            if (requirePlayerTag && !other.CompareTag(playerTag))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(targetLevelId) &&
                string.IsNullOrWhiteSpace(targetPackageId))
            {
                Debug.LogError("Level transition failed: target level id and target package id are both missing.");
                return;
            }

            if (levelLoader == null)
            {
                levelLoader = Object.FindAnyObjectByType<LevelLoader>();
            }

            if (levelLoader == null)
            {
                Debug.LogError("Level transition failed: no LevelLoader was assigned or found in the scene.");
                return;
            }

            triggered = true;

            LevelLoadRequest request = new LevelLoadRequest
            {
                currentPackageId = string.Empty,
                targetLevelId = targetLevelId,
                targetPackageId = targetPackageId,
                transitionType = transitionType,
                seed = 0,
                hasExplicitSeed = false
            };

            Debug.Log(
                $"Level transition triggered. targetPackageId='{targetPackageId}', targetLevelId='{targetLevelId}', transitionType='{transitionType}'.");

            levelLoader.Load(request);
        }
    }
}
