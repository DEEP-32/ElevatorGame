using System.Collections.Generic;
using ElevatorGame.GameEntity;
using UnityEngine;

namespace ElevatorGame {
    
    /// <summary>
    /// SCENE DATA WRAPPER
    /// 
    /// Role: Exposes specific scene objects to the GameManager.
    /// Purpose: Decentralizes scene hierarchy from logic; if we move floors/elevators, 
    /// we only update this container.
    /// </summary>
    public class LevelContext : MonoBehaviour {
        [SerializeField] List<Floor> floors;
        [SerializeField] List<Elevator> elevators;
        
        public IReadOnlyList<Floor> Floors => floors;
        public IReadOnlyList<Elevator> Elevators => elevators;
    }
}