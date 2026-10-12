# Hello World

## 概要
プレイしながらC言語が学べるコマンドバトル型のゲーム

## 開発環境
- Unity 6000.4.3f
- C#
- 製作期間：半年

## 見てほしいポイント
- `BattleScript/BattleManager.cs` が自作のC言語のインタプリタで、これでプレイヤーの書いたコードを攻撃に変えています

## ファイル構成
- `BattleScript`はバトルを行うBattleScene用のスクリプト
- `BootScript`はゲーム起動時のBootScene用のスクリプト
- `ChangeSceneScript`がシーン変更を行う用のスクリプト
- `ClearScript`はステージクリア時のClearScene用のスクリプト
- `CustomScript`は関数を作るCustomScene用のスクリプト
- `InfoScript`はプレイヤーステータスや敵図鑑、ヘルプといった情報を教えるInfoScene用のスクリプト
- `ScriptObject`はステージ用のスクリプト
- `StageSelectScript`はステージ選びをするStageSelectScene用のスクリプト
- `StatusScript`はセーブやステータス用のスクリプト
- `SystemSceneScript`はSystemScene用のスクリプト
- `TitleScript`はタイトル画面のTitleScene用のスクリプト
- `TutorialScript`はチュートリアル表示用のスクリプト
- ファイルに入ってないスクリプトは複数シーンで使うがまとめるほどでもなかった雑務用のスクリプト
