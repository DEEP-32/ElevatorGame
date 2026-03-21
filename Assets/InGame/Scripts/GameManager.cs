using System;
using ElevatorGame.GameEntity;
using UnityEngine;

namespace ElevatorGame {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance = null;

        public LevelContext Context { get; private set; }

        public UIHandler UiHandler { get; private set; }

        public static Floor GetFloor(int index) {
            foreach (var floor in Instance.Context.Floors) {
                if (floor.FloorNumber == index) {
                    return floor;
                }
            }

            return null;
        }


        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            Context = FindFirstObjectByType<LevelContext>();
            UiHandler = FindFirstObjectByType<UIHandler>();
        }

        void Start() {
            foreach (var elevator in Context.Elevators) {
                elevator.SetCurrentFloor(0);
            }

            UiHandler.Initialize();
        }


        public void RequestElevatorForFloor(int floorIndex) {
            Elevator bestElevator = FindBestElevator(floorIndex);

            if (bestElevator == null) {
                Debug.LogError($"[GameManager] No elevator found for floor {floorIndex}");
                return;
            }

            bestElevator.AddFloorRequest(floorIndex);
        }

        private Elevator FindBestElevator(int floorIndex) {
            Elevator best = null;
            float bestScore = float.MaxValue;

            foreach (var elevator in Context.Elevators) {
                // Skip elevators already handling this floor
                if (elevator.IsFloorAlreadyQueued(floorIndex)) return elevator;

                float score = GetElevatorScore(elevator, floorIndex);

                if (score < bestScore) {
                    bestScore = score;
                    best = elevator;
                }
            }

            return best;
        }

        private float GetElevatorScore(Elevator elevator, int floorIndex) {
            float distance = Mathf.Abs(elevator.CurrentFloor - floorIndex);

            // Idle elevator — pure distance score
            if (!elevator.IsMoving)
                return distance;

            // Moving elevator — check if floor is on the way
            if (IsFloorOnTheWay(elevator, floorIndex))
                return distance * 0.5f; // reward — prefer this elevator

            // Moving but wrong direction — penalise
            return distance * 2f;
        }

        private bool IsFloorOnTheWay(Elevator elevator, int floorIndex) {
            bool movingUp = elevator.TargetFloor > elevator.CurrentFloor;
            bool isAbove = floorIndex > elevator.CurrentFloor;
            bool isBelow = floorIndex < elevator.CurrentFloor;

            return (movingUp && isAbove) || (!movingUp && isBelow);
        }
    }
}