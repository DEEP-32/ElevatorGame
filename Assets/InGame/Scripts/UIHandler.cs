using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ElevatorGame {
    /// <summary>
    /// GLOBAL UI MANAGER
    /// 
    /// Role: Acts as the intermediary between Unity UI (Canvas/Buttons) and the GameManager.
    /// Responsibility: Sets up initial event listeners for floor buttons and 
    /// translates UI clicks into Dispatcher requests.
    /// </summary>
    public class UIHandler : MonoBehaviour {
        [SerializeField] List<ElevatorButton> buttons;

        public void Initialize() {
            foreach (var button in buttons) {
                var elevButton = button;
                elevButton.Button.onClick.AddListener(() => OnButtonClicked(elevButton.FloorNumber));
            }            
        }

        public void OnButtonClicked(int index) {
            Debug.Log("Button clicked: " + index);
            GameManager.Instance.RequestElevatorForFloor(index);
        }
        
    }
}