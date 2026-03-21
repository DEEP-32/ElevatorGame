using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ElevatorGame {
    public class UIHandler : MonoBehaviour {
        [SerializeField] List<ElevatorButton> buttons;

        public void Initialize() {
            foreach (var button in buttons) {
                var elevButton = button;
                elevButton.Button.onClick.AddListener(() => OnButtonClicked(elevButton.FloorNumber));
            }            
        }

        public void OnButtonClicked(int index) {
            GameManager.Instance.RequestElevatorForFloor(index);
        }
        
    }
}