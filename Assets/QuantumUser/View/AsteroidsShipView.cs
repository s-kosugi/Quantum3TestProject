using Quantum;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

namespace Quantum.Asteroids
{
    public unsafe class AsteroidsShipView : QuantumEntityViewComponent
    {
        public Canvas PlayerNameCanvas;
        public GameObject Model;
        public Text PlayerNameText;

        public ParticleSystem PropulsionFX;

        private Quaternion _initialRotation;

        public override void OnActivate(Frame frame)
        {
#if QUANTUM_XY
            _initialRotation = Quaternion.identity;
            Model.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
#else
             _initialRotation = Quaternion.Euler(90f, 0f, 0f);
             Model.transform.localRotation = Quaternion.identity;
#endif
            if (PlayerNameText != null)
            {
                PlayerLink playerLink = PredictedFrame.Get<PlayerLink>(_entityView.EntityRef);
                RuntimePlayer playerData = PredictedFrame.GetPlayerData(playerLink.PlayerRef);
                PlayerNameText.text = playerData.PlayerNickname;
            }
        }
        public override void OnUpdateView()
        {
            // プレイヤーの回転にあわせず初期回転位置を記憶しておき、UIは常に正常な角度で見えるようにする
            PlayerNameCanvas.transform.rotation = _initialRotation;
            // リスポーン時は自機のビュー(UIなど)を見えなくする
            ChangeShipView(PredictedFrame.Has<AsteroidsShipRespawn>(EntityRef)== false);

            PlayerLink playerLink = PredictedFrame.Get<PlayerLink>(_entityView.EntityRef);
            Quantum.Input* input = PredictedFrame.GetPlayerInput(playerLink.PlayerRef);

            if (input->Up)
            {
                if (PropulsionFX.isPlaying == false)
                {
                    PropulsionFX.Play();
                }
            }
            else
            {
                PropulsionFX.Stop();
            }
        }

        private void ChangeShipView(bool isAlive)
        {
            Model.SetActive(isAlive);
            PlayerNameCanvas.gameObject.SetActive(isAlive);
        }
    }
}