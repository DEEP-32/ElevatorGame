using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ElevatorGame.Player {
    
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour {
        
        [SerializeField] float moveSpeed = 10f;
        float xDir = 0;
        
        Rigidbody2D rb;

        void Awake() {
            rb = GetComponent<Rigidbody2D>();
        }

        public void OnMove(InputValue value) {
            xDir = value.Get<Vector2>().x;
        }

        void FixedUpdate() {
            rb.linearVelocityX = xDir * moveSpeed * Time.fixedDeltaTime;
        }
    }
}