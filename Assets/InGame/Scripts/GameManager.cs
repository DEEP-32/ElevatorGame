using ElevatorGame.GameEntity;
using UnityEngine;

namespace ElevatorGame {
    /// <summary>
    /// CENTRAL DISPATCHER / COORDINATOR
    /// 
    /// Role: Acts as the high-level brain for the elevator system.
    /// Responsibility: Receives requests from floors/UI and assigns them to the "best" elevator.
    /// Pattern: Singleton + Level-Specific Context.
    /// </summary>
    public class GameManager : MonoBehaviour {
        public static GameManager Instance = null;

        /// <summary>
        /// Scene references container (Floors, Elevators). 
        /// Separating this from GameManager keeps the manager independent of scene hierarchy changes.
        /// </summary>
        public LevelContext Context { get; private set; }

        public UIHandler UiHandler { get; private set; }

        // ─────────────────────────────────────────
        // Unity Lifecycle
        // ─────────────────────────────────────────

        void Awake() {
            // Singleton enforcement: Ensures only one dispatcher exists across scene loads.
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            // Auto-discovery of key scene components.
            Context = FindFirstObjectByType<LevelContext>();
            UiHandler = FindFirstObjectByType<UIHandler>();
        }

        void Start() {
            // Initializing system state.
            foreach (var elevator in Context.Elevators)
                elevator.SetCurrentFloor(0);

            UiHandler.Initialize();
        }

        // ─────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────

        /// <summary>
        /// Helper to retrieve floor data by its logical index.
        /// Architecture Note: Using global floor lookup allows elevators to be agnostic of 
        /// exact scene positions until they need to move.
        /// </summary>
        public static Floor GetFloor(int index) {
            foreach (var floor in Instance.Context.Floors)
                if (floor.FloorNumber == index)
                    return floor;

            return null;
        }

        /// <summary>
        /// The primary entry point for external systems (Buttons, AI) to request service.
        /// </summary>
        public void RequestElevatorForFloor(int floorIndex) {
            // Step 1: Run Dispatcher logic to pick the optimal unit.
            Elevator best = FindBestElevator(floorIndex);

            if (best == null) {
                Debug.LogError($"[GameManager] No elevator found for floor {floorIndex}");
                return;
            }

            // Step 2: Delegate the request to the specific Elevator instance.
            best.AddFloorRequest(floorIndex);
        }

        // ─────────────────────────────────────────
        // Dispatching Logic (Heuristic-based)
        // ─────────────────────────────────────────

        /// <summary>
        /// SCORING SYSTEM (The "Elevator Algorithm")
        /// 
        /// Instead of a simple "closest elevator", we use a scoring heuristic.
        /// Lower score = More efficient assignment.
        /// </summary>
        private Elevator FindBestElevator(int floorIndex) {
            Elevator best = null;
            float bestScore = float.MaxValue;

            foreach (var elevator in Context.Elevators) {
                // Efficiency check: if an elevator is already going there, just let it handle it.
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

        /// <summary>
        /// HEURISTIC CALCULATION
        /// 
        /// Weighted factors:
        /// 1. Distance: Baseline cost based on physical Y distance.
        /// 2. Directional Efficiency: 
        ///    - If elevator is IDLE: Score = Distance.
        ///    - If elevator is "ON THE WAY": Score = Distance * 0.5 (Highly efficient).
        ///    - If elevator is moving AWAY: Score = Distance * 2.0 (Expensive/Low priority).
        /// </summary>
        private float GetElevatorScore(Elevator elevator, int floorIndex) {
            Floor requestedFloor = GetFloor(floorIndex);
            if (requestedFloor == null) return float.MaxValue;

            // Real world Y distance — not floor index arithmetic (more accurate for uneven floors).
            float distance = Mathf.Abs(
                elevator.transform.position.y - requestedFloor.ElevatorPos.position.y
            );

            if (!elevator.IsMoving)
                return distance;

            if (IsFloorOnTheWay(elevator, floorIndex))
                return distance * 0.5f;

            return distance * 2f;
        }

        /// <summary>
        /// Check if the requested floor lies between the elevator's current Y and its current Target Y.
        /// </summary>
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