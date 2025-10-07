using UnityEngine.Scripting;
using Quantum;
using UnityEngine;
namespace Quantum.Asteroids
{

//#if QUANTUM_ENABLE_TEXTMESHPRO
    using Text = TMPro.TextMeshProUGUI;
//#else
//    using Text = UnityEngine.UI.Text;
//#endif
    [Preserve]
    public unsafe class AsteroidGameView : QuantumSceneViewComponent
    {
        public Text LevelText;
        public Text ScoreBoard;

        public Camera Camera2D;
        public Camera Camera3D;

        public override void OnInitialize()
        {
#if QUANTUM_XY
            Camera2D.gameObject.SetActive(true);
            Camera3D.gameObject.SetActive(false);
#else
            Camera2D.gameObject.SetActive(false);
            Camera3D.gameObject.SetActive(true);
#endif
        }
        public override void OnUpdateView()
        {
            if (LevelText != null)
            {
                LevelText.text = $"Level {VerifiedFrame.Global->AsteroidsWaveCount}";
            }
            if (ScoreBoard != null)
            {
                ScoreBoard.text = "<b>Score</b>\n";
                var shipsFilter = VerifiedFrame.Filter<PlayerLink, AsteroidShip>();
                while (shipsFilter.Next(out var entity, out var playerLink, out var shipFields))
                {
                    var playerName = VerifiedFrame.GetPlayerData(playerLink.PlayerRef).PlayerNickname;
                    ScoreBoard.text += $"{playerName}:{shipFields.Score}  \n";
                }
            }
        }
    }

}
