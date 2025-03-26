
AppleGame

※個人開発で作ったプロジェクトです。なるべく共有はせず、ポートフォリオの一部として見て頂ければ幸いです。
※無断使用禁止とさせていただきます。

ロジックを切り分けて実装するのが好きで、このプロジェクトではUniTaskとUniRxを使用しています。
主にMVPパターンで実装しており、Presenter周りの処理をUniRxを使用した、いわゆるMV(R)Pパターンで実装しています。
ModelとViewはお互いを知らないので、結合した実装にならず、例えばUIを変更してもModelには影響を受けない部分など気に入って個人開発では使用しています。
(ゲームロジック部分のModel,ゲーム画面UI部分のView,それを繋ぐPresenter)

ゲーム自体はコード量もなく簡単な実装です。
ゲームロジック部分は、[Assets/Scripts/GameModel.cs](https://github.com/RibertaGames/AppleGame/blob/0404f229318d3cb18589cbbbfd586cb2d54cdc27/Assets/Scripts/GameModel.cs) この辺が参考になるかと思いますので、よろしくお願い致します。
