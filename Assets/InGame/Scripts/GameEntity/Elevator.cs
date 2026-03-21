using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElevatorGame.GameEntity {
    public class Elevator : MonoBehaviour {
        [SerializeField] private Mover mover;
        [SerializeField] private ElevatorUI elevatorUI;
        [SerializeField] private int delayBetweenFloors = 1;

        private Queue<int> floorsToGo = new Queue<int>();

        public bool IsMoving => mover.IsMoving;

        public int TargetFloor { get; private set; } = -1;

        public int CurrentFloor { get; private set; } = 0;

        // ─────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────

        public void SetCurrentFloor(int floorNumber) {
            CurrentFloor = floorNumber;
            elevatorUI.SetFloorText(floorNumber);
        }

        public void AddFloorRequest(int floorNumber) {
            if (IsFloorAlreadyQueued(floorNumber)) {
                Debug.Log($"[Elevator] Floor {floorNumber} already queued, skipping.");
                return;
            }

            if (IsMoving) {
                // Elevator already moving — insert in smart sorted position
                InsertFloorSorted(floorNumber);
            }
            else {
                // Elevator idle — just enqueue and kick off
                floorsToGo.Enqueue(floorNumber);
                ProcessNextFloor();
            }
        }

        public bool IsFloorAlreadyQueued(int floorIndex) {
            foreach (var floor in floorsToGo)
                if (floor == floorIndex)
                    return true;

            return TargetFloor == floorIndex;
        }

        // ─────────────────────────────────────────
        // Internal Queue Processing
        // ─────────────────────────────────────────

        // Single private method drives everything — no public chaining
        private void ProcessNextFloor() {
            if (floorsToGo.Count == 0) {
                TargetFloor = -1;
                Debug.Log($"[Elevator] {gameObject.name} queue empty, now idle.");
                return;
            }

            int nextFloor = floorsToGo.Dequeue(); // remove from queue HERE
            TargetFloor = nextFloor; // mark as active target

            Debug.Log($"[Elevator] {gameObject.name} moving to floor {nextFloor}. Remaining: [{string.Join(", ", floorsToGo)}]");

            var floor = GameManager.GetFloor(nextFloor);

            mover.MoveTo(floor.ElevatorPos.position.y, onComplete: () => { OnFloorReached(nextFloor); });
        }

        private void OnFloorReached(int floorNumber) {
            CurrentFloor = floorNumber;
            TargetFloor = -1;

            elevatorUI.SetFloorText(CurrentFloor);

            Debug.Log($"[Elevator] {gameObject.name} reached floor {floorNumber}.");

            // Wait then process next — using coroutine instead of Invoke
            // so it's cancellable if needed
            if (floorsToGo.Count > 0)
                StartCoroutine(DelayThenNext());
        }

        private IEnumerator DelayThenNext() {
            yield return new WaitForSeconds(delayBetweenFloors);
            ProcessNextFloor();
        }

        // ─────────────────────────────────────────
        // Smart Queue Insertion
        // ─────────────────────────────────────────

        private void InsertFloorSorted(int floorNumber)
        {
            bool movingUp = TargetFloor > CurrentFloor;

            // Include TargetFloor in the list so we can sort against it
            List<int> sorted = new List<int>(floorsToGo);
            sorted.Add(TargetFloor);    // ← add active target back in
            sorted.Add(floorNumber);    // ← add new request

            if (movingUp)
            {
                sorted.Sort((a, b) =>
                {
                    bool aAbove = a >= CurrentFloor;
                    bool bAbove = b >= CurrentFloor;

                    if (aAbove && bAbove)   return a.CompareTo(b);
                    if (!aAbove && !bAbove) return b.CompareTo(a);
                    return aAbove ? -1 : 1;
                });
            }
            else
            {
                sorted.Sort((a, b) =>
                {
                    bool aBelow = a <= CurrentFloor;
                    bool bBelow = b <= CurrentFloor;

                    if (aBelow && bBelow)   return b.CompareTo(a);
                    if (!aBelow && !bBelow) return a.CompareTo(b);
                    return aBelow ? -1 : 1;
                });
            }

            // First item in sorted becomes the NEW immediate target
            // Stop current movement and redirect
            int newTarget = sorted[0];
            sorted.RemoveAt(0);

            floorsToGo = new Queue<int>(sorted);

            // Only redirect if new target is different from current target
            if (newTarget != TargetFloor)
            {
                Debug.Log($"[Elevator] Redirecting from Floor {TargetFloor} → Floor {newTarget}");
                RedirectTo(newTarget);
            }

            Debug.Log($"[Elevator] Queue after insert: [{string.Join(", ", floorsToGo)}]");
        }

        private void RedirectTo(int floorNumber)
        {
            // Stop current movement and go to new target instead
            mover.Stop();
            TargetFloor = floorNumber;

            var floor = GameManager.GetFloor(floorNumber);

            mover.MoveTo(floor.ElevatorPos.position.y, onComplete: () =>
            {
                OnFloorReached(floorNumber);
            });
        }

    }
}