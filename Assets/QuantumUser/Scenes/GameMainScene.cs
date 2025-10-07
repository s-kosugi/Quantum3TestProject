using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quantum;

public class GameMainScene : MonoBehaviour
{
    void OnEnable()
    {
        QuantumEvent.Subscribe(listener: this, handler: (EventGameEnded e) => EndGame(e.WinnerName,e.FinalScore), once: true);

    }

    private void EndGame(string WinnerName, int FinalScore)
    {
        Debug.Log($"ゲームエンドのイベント受け取り {WinnerName} {FinalScore} ");

        // Quantumの通信を停止(ここでやると一瞬空画面が見えるので本来は遷移先でやる)
        QuantumRunner.ShutdownAll();

        // リザルトシーン(※今はメニュー切り替え)へ
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
