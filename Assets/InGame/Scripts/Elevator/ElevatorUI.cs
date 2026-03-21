using TMPro;
using UnityEngine;

namespace ElevatorGame {
    public class ElevatorUI : MonoBehaviour {
        [SerializeField] TMP_Text floorText;
        
        public void SetFloorText(int floor) {
            floorText.text = floor.ToString();
        }
    }
}