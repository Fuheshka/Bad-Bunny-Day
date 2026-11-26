using UnityEngine;

namespace GameJam.Player
{
    /// <summary>
    /// Player model holding reference to physics components and velocity state
    /// </summary>
    public class PlayerModel
    {
        public Rigidbody Rigidbody { get; set; }
        public Transform Transform { get; set; }
        public Vector3 CurrentVelocity { get; set; } = Vector3.zero;

        public PlayerModel(Rigidbody rigidbody, Transform transform)
        {
            Rigidbody = rigidbody;
            Transform = transform;
        }
    }
}
