using System.Collections.Generic;
using ElevatorGame.GameEntity;
using UnityEngine;

namespace ElevatorGame {
    
    //This class will hold all the data in our scene to reference it
    public class LevelContext : MonoBehaviour {
        [SerializeField] List<Floor> floors;
        [SerializeField] List<Elevator> elevators;
        
        public IReadOnlyList<Floor> Floors => floors;
        public IReadOnlyList<Elevator> Elevators => elevators;
    }
}