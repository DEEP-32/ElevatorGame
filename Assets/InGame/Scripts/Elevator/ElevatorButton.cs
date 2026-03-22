using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ElevatorGame {
    /// <summary>
    /// UI COMPONENT
    /// 
    /// Role: Represents an individual button on the elevator control panel.
    /// Responsibility: Holds the floor index and a reference to its UI Button.
    /// </summary>
    public class ElevatorButton : MonoBehaviour {
        
        public int FloorNumber => floorNumber;
        public Button Button => button;

        
        [SerializeField] TMP_Text buttonText;
        [SerializeField] Button button;
        [SerializeField] int floorNumber;


        #if UNITY_EDITOR
        void OnValidate() {
            if (buttonText != null) {
                buttonText.text = floorNumber.ToString();
            }
        }
        #endif
    }
}