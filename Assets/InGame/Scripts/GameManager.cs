using ElevatorGame.GameEntity;
using UnityEngine;

namespace ElevatorGame {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance = null;

        public LevelContext Context { get; private set; }

        public UIHandler UiHandler { get; private set; }

        // ─────────────────────────────────────────
        // Unity Lifecycle
        // ─────────────────────────────────────────

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
            foreach (var elevator in Context.Elevators)
                elevator.SetCurrentFloor(0);

            UiHandler.Initialize();
        }

        // ─────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────

        public static Floor GetFloor(int index) {
            foreach (var floor in Instance.Context.Floors)
                if (floor.FloorNumber == index)
                    return floor;

            return null;
        }

        public void RequestElevatorForFloor(int floorIndex) {
            Elevator best = FindBestElevator(floorIndex);

            if (best == null) {
                Debug.LogError($"[GameManager] No elevator found for floor {floorIndex}");
                return;
            }

            best.AddFloorRequest(floorIndex);
        }

        // ─────────────────────────────────────────
        // Dispatching Logic
        // ─────────────────────────────────────────

        private Elevator FindBestElevator(int floorIndex) {
            Elevator best = null;
            float bestScore = float.MaxValue;

            foreach (var elevator in Context.Elevators) {
                if (elevator.IsFloorAlreadyQueued(floorIndex)) {
                    Debug.Log($"[GameManager] {elevator.name} already handling floor {floorIndex}");
                    return elevator;
                }

                float score = GetElevatorScore(elevator, floorIndex);
                Debug.Log($"[GameManager] {elevator.name} score: {score}");

                if (score < bestScore) {
                    bestScore = score;
                    best = elevator;
                }
            }

            Debug.Log($"[GameManager] Best: {best?.name} with score {bestScore}");
            return best;
        }

        private float GetElevatorScore(Elevator elevator, int floorIndex) {
            Floor requestedFloor = GetFloor(floorIndex);
            if (requestedFloor == null) return float.MaxValue;

            // Real world Y distance — not floor index arithmetic
            float distance = Mathf.Abs(
                elevator.transform.position.y - requestedFloor.ElevatorPos.position.y
            );

            if (!elevator.IsMoving)
                return distance;

            if (IsFloorOnTheWay(elevator, floorIndex))
                return distance * 0.5f;

            return distance * 2f;
        }

        private bool IsFloorOnTheWay(Elevator elevator, int floorIndex) {
            Floor requestedFloor = GetFloor(floorIndex);
            Floor targetFloor = GetFloor(elevator.TargetFloor);

            if (requestedFloor == null || targetFloor == null) return false;

            float elevatorY = elevator.transform.position.y;
            float requestedY = requestedFloor.ElevatorPos.position.y;
            float targetY = targetFloor.ElevatorPos.position.y;

            bool movingUp = targetY > elevatorY;

            if (movingUp)
                // On the way up — requested floor must be between here and target
                return requestedY > elevatorY && requestedY <= targetY;
            else
                // On the way down — requested floor must be between here and target
                return requestedY < elevatorY && requestedY >= targetY;
        }
    }
}