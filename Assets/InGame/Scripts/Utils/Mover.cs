using System;
using System.Collections;
using UnityEngine;

namespace ElevatorGame.GameEntity {
    /// <summary>
    /// Moves a transform smoothly to a target Y position using
    /// AnimationCurve for easing and LerpUnclamped for overshoot/bounce support.
    /// </summary>
    public class Mover : MonoBehaviour {
        [Header("Movement Settings")]
        [SerializeField]
        float moveDuration = 1.5f;

        [Header("Easing — tip: values above 1 in curve = bounce/overshoot")]
        [SerializeField]
        AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Distance Scaling")]
        [SerializeField]
        float maxFloorDistance = 9f;

        private Coroutine _moveCoroutine;

        public bool IsMoving { get; private set; }

        // ─────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────

        /// <summary>
        /// Moves the elevator to the given world Y position.
        /// Interrupts any current movement and starts fresh.
        /// onComplete fires when the elevator reaches the target.
        /// completeInstant if true, the elevator will move instantly to the target.
        /// </summary>
        public void MoveTo(float targetY, Action onComplete = null, bool completeInstant = false) {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);

            if (!completeInstant) {
                _moveCoroutine = StartCoroutine(MoveRoutine(targetY, onComplete));
            }
            else {
                IsMoving = true;
                transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
                IsMoving = false;
                onComplete?.Invoke();
            }
        }

        /// <summary>
        /// Immediately stops any ongoing movement.
        /// </summary>
        public void Stop() {
            if (_moveCoroutine != null) {
                StopCoroutine(_moveCoroutine);
                _moveCoroutine = null;
            }

            IsMoving = false;
        }

        // ─────────────────────────────────────────
        // Core Movement
        // ─────────────────────────────────────────

        private IEnumerator MoveRoutine(float targetY, Action onComplete) {
            IsMoving = true;

            Vector3 startPos = transform.position;
            Vector3 endPos = new Vector3(transform.position.x, targetY, transform.position.z);

            // Already at target — no movement needed
            if (Mathf.Approximately(startPos.y, targetY)) {
                IsMoving = false;
                onComplete?.Invoke();
                yield break;
            }

            // Scale duration proportionally to distance
            // so a 1-floor trip is faster than a 3-floor trip
            float distance = Mathf.Abs(targetY - startPos.y);
            float scaledDuration = moveDuration * (distance / maxFloorDistance);

            // Clamp so very short trips don't feel instant
            scaledDuration = Mathf.Max(scaledDuration, 0.3f);

            float elapsed = 0f;

            while (elapsed < scaledDuration) {
                elapsed += Time.deltaTime;

                // t is always 0→1 (linear progress through the trip)
                float t = Mathf.Clamp01(elapsed / scaledDuration);

                // curvedT can go above 1 or below 0 if curve is designed that way
                // This is what enables bounce and overshoot effects
                float curvedT = moveCurve.Evaluate(t);

                // LerpUnclamped respects curvedT values outside 0-1
                // unlike Lerp which would clamp and kill the bounce
                transform.position = Vector3.LerpUnclamped(startPos, endPos, curvedT);

                yield return null;
            }

            // Always snap exactly to target
            // Handles float imprecision and curve not ending exactly at 1
            transform.position = endPos;

            IsMoving = false;
            _moveCoroutine = null;
            onComplete?.Invoke();
        }

        // ─────────────────────────────────────────
        // Debug
        // ─────────────────────────────────────────

#if UNITY_EDITOR
        [ContextMenu("Test Move Up 3 Units")]
        void TestMoveUp() {
            MoveTo(transform.position.y + 3f, () => Debug.Log("[ElevatorMover] Reached target."));
        }

        [ContextMenu("Test Move Down 3 Units")]
        void TestMoveDown() {
            MoveTo(transform.position.y - 3f, () => Debug.Log("[ElevatorMover] Reached target."));
        }
#endif
    }
}