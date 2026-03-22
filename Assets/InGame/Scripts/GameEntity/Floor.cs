using UnityEngine;

namespace ElevatorGame.GameEntity {
    /// <summary>
    /// WORLD OBJECT
    /// 
    /// Role: Represents a physical floor in the game world.
    /// Responsibility: Acts as a reference for elevator positioning.
    /// </summary>
    public class Floor : MonoBehaviour {
        [SerializeField] int floorNumber;
        [SerializeField] Transform elevatorPos;
        
        public int FloorNumber => floorNumber;
        public Transform ElevatorPos => elevatorPos;
    }
}