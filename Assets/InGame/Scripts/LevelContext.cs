using System;
using System.Collections.Generic;
using System.Linq;
using ElevatorGame.GameEntity;
using UnityEngine;

namespace ElevatorGame {
    
    //This class will hold all the data in our scene to reference it
    public class LevelContext : MonoBehaviour {
        List<Floor> floors;
        List<Elevator> elevators;
        
        public IReadOnlyList<Floor> Floors => floors;
        public IReadOnlyList<Elevator> Elevators => elevators;

        void Awake() {
            floors = GameObject.FindObjectsByType<Floor>(FindObjectsInactive.Exclude,FindObjectsSortMode.None).ToList();
            elevators = GameObject.FindObjectsByType<Elevator>(FindObjectsInactive.Exclude,FindObjectsSortMode.None).ToList();
        }
    }
}