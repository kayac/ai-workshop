# Eat and Born

## ゲームのルール

https://docs.google.com/presentation/d/1gYyFVCPvicma9GEbDx4PYy7s7TTTtbfBiryHRKJ1LEo/pub?start=false&loop=false&delayms=3000&slide=id.p

## ゲームの設定・AIを変更する

[Assets/Setting_and_AI_is_here](https://github.com/kayac/ai-workshop/tree/master/Assets/Setting_and_AI_is_here) 以下に設定ファイルとAI・ロジックのサンプルが置かれています。

* [Settings.cs](https://github.com/kayac/ai-workshop/blob/master/Assets/Setting_and_AI_is_here/Settings.cs) : ゲームの設定や使用するロジック・AIの指定が書かれている
* [CharacterAI](https://github.com/kayac/ai-workshop/tree/master/Assets/Setting_and_AI_is_here/CharacerAI) : キャラクターのAIのサンプル
* [Food](https://github.com/kayac/ai-workshop/tree/master/Assets/Setting_and_AI_is_here/Food) : 食べ物の配置ロジック
* [Preset](https://github.com/kayac/ai-workshop/tree/master/Assets/Setting_and_AI_is_here/Preset) : ゲーム開始時のステージ構成


ゲーム進行中の情報を取得したい場合は、[GameManager](https://github.com/kayac/ai-workshop/blob/master/Assets/Game/Scenes/GameScene/Scripts/GameManager.cs#L72)から取得できます。
