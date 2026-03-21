using UnityEngine;

namespace ElevatorGame.GameEntity {
    public class Floor : MonoBehaviour {
        [SerializeField] int floorNumber;
        [SerializeField] Transform elevatorPos;
        
        public int FloorNumber => floorNumber;
        public Transform ElevatorPos => elevatorPos;
    }
}