using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ElevatorGame {
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