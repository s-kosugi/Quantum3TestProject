using UnityEngine;
using Photon.Deterministic;

namespace Quantum.Asteroids
{
    public class AsteroidInput : MonoBehaviour
    {
        private void OnEnable()
        {
            QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
        }
        public void PollInput(CallbackPollInput callback)
        {
            Quantum.Input i = new Quantum.Input();

            i.Left = UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetKey(KeyCode.LeftArrow) || UnityEngine.Input.GetAxis("Horizontal") < 0;
            i.Right = UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetKey(KeyCode.RightArrow);
            i.Up = UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetKey(KeyCode.UpArrow);
            i.Fire = UnityEngine.Input.GetKey(KeyCode.Space);

            callback.SetInput(i, DeterministicInputFlags.Repeatable);
        }
    }
}