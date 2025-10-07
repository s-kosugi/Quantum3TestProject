

namespace Quantum.Asteroids
{
  using UnityEngine.Scripting;
  using Photon.Deterministic;
  using Quantum;
  using UnityEngine;

  [Preserve]
  public unsafe class AsteroidsRuleSystem : SystemMainThread, ISignalStartGameRuleInitialize
  {
    public override void OnInit(Frame frame)
    {
      // テストではあまり意味ないが、今後のことを考えて練習
      frame.Signals.StartGameRuleInitialize();
    }
    public override void Update(Frame frame)
    {
      if (frame.TryGetSingleton<AsteroidsGameRule>(out var gameRule))
      {
        // ゲームエンドしてたらやらない
        if (gameRule.GameEnded) return;

        gameRule.ElapsedTime += frame.DeltaTime;
        if (gameRule.ElapsedTime >= gameRule.MaxTime)
        {
          Debug.Log("イベント発火");
          gameRule.GameEnded = true;
          // ※いったんここで発火
          frame.Signals.EndGame();
          // ※Unity側へイベント発火
          // ※ここでどうやってプレイヤー側のデータを拾ってくるか後々考える
          frame.Events.GameEnded(0,"test");
        }
        frame.SetSingleton(gameRule);
      }

      Debug.Log("経過時間 = " + gameRule.ElapsedTime);
    }
    /// <summary>
    /// ゲーム開始時のルールの初期化
    /// </summary>
    public void StartGameRuleInitialize(Frame frame)
    {
      AsteroidsGameRule gameRule;
      // シングルトン作成
      gameRule = new AsteroidsGameRule
      {
        // ※後でConfig値に変更する
        ElapsedTime = 0,
        MaxTime = 15,
        GameEnded = false
      };
      frame.SetSingleton(gameRule);
    }
  }
}