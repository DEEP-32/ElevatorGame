using TMPro;
using UnityEngine;

namespace ElevatorGame {
    /// <summary>
    /// UI DISPLAY
    /// 
    /// Role: Small display unit attached to an elevator.
    /// Responsibility: Shows the current floor number.
    /// </summary>
    public class ElevatorUI : MonoBehaviour {
        [SerializeField] TMP_Text floorText;
        
        public void SetFloorText(int floor) {
            floorText.text = floor.ToString();
        }
    }
}