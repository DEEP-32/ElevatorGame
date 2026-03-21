using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElevatorGame.GameEntity {
    public class Elevator : MonoBehaviour {
        [SerializeField] Mover mover;
        [SerializeField] ElevatorUI elevatorUI;
        Queue<int> floorsToGo = new Queue<int>();

        public bool IsMoving => mover.IsMoving;

        public int TargetFloor { get; private set; } = -1;

        public int CurrentFloor { get; private set; } = 0;


        public void SetCurrentFloor(int floorNumber) {
            CurrentFloor = floorNumber;
            elevatorUI.SetFloorText(floorNumber);
        }

        public void AddFloorRequest(int floorNumber) {
            if (IsFloorAlreadyQueued(floorNumber)) {
                return;
            }

            if (!IsMoving) {
                floorsToGo.Enqueue(floorNumber);
                GoToNextFloor();
                return;
            }

            InsertFloorSorted(floorNumber);
        }

        void InsertFloorSorted(int floorNumber) {
            bool movingUp = TargetFloor > CurrentFloor;

            List<int> sorted = new List<int>(floorsToGo);
            sorted.Add(floorNumber);

            if (movingUp) {
                // Sort: floors above current first (ascending), then floors below (descending)
                sorted.Sort((a, b) => {
                    bool aAbove = a >= CurrentFloor;
                    bool bAbove = b >= CurrentFloor;

                    if (aAbove && bAbove) return a.CompareTo(b); // both above — nearest first
                    if (!aAbove && !bAbove) return b.CompareTo(a); // both below — highest first
                    return aAbove ? -1 : 1; // above floors come first
                });
            }
            else {
                // Sort: floors below current first (descending), then floors above (ascending)
                sorted.Sort((a, b) => {
                    bool aBelow = a <= CurrentFloor;
                    bool bBelow = b <= CurrentFloor;

                    if (aBelow && bBelow) return b.CompareTo(a); // both below — nearest first
                    if (!aBelow && !bBelow) return a.CompareTo(b); // both above — lowest first
                    return aBelow ? -1 : 1; // below floors come first
                });
            }
            
            floorsToGo = new Queue<int>(sorted);
        }

        public int GetNextFloor() {
            return floorsToGo.Peek();
        }


        public void GoToNextFloor(Action onFloorReached = null) {
            if (onFloorReached == null) onFloorReached = FloorReached;
            else onFloorReached += FloorReached;

            int nextFloor = floorsToGo.Dequeue();
            TargetFloor = nextFloor;
            var floor = GameManager.GetFloor(nextFloor);
            mover.MoveTo(floor.ElevatorPos.position.y, onFloorReached);
        }

        void FloorReached() {
            CurrentFloor = TargetFloor;
            TargetFloor = -1;

            elevatorUI.SetFloorText(CurrentFloor);
            if (floorsToGo.Count > 0) {
                GoToNextFloor();
            }
        }

        public bool IsFloorAlreadyQueued(int floorIndex) {
            foreach (var floor in floorsToGo)
                if (floor == floorIndex) return true;

            return TargetFloor == floorIndex;
        }
    }
}