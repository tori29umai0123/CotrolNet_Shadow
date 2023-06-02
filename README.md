# CotrolNet_Shadow
特定のディレクトリに入った3Dモデルと光源の向き情報をレンダリングするだけのやつ。<br>
CotrolNet用のデータセットを作る為に作った。
分かっている人だけ使ってください

# 使い方
①有料アセットTriLib 2をインポート<br>
https://assetstore.unity.com/packages/tools/modeling/trilib-2-model-loading-package-157548?locale=ja-JP<br>

②unitypackageをインポート<br>
https://github.com/tori29umai0123/CotrolNet_Shadow/releases/tag/CotrolNet_Shadow_V1.0

③CotrolNet_Shadowシーンを開いて、Hierarchy→System→Camera_Captureを設定する<br>
outpath：出力先<br>
modelpath：3Dモデルがある場所<br>
max_count：出力画像の枚数<br>

④Unityを実行すると画像が出力されるはず<br>
読みこむ3Dモデルを以下のものに最適化しているので、座標とかおかしくなる場合はModelLoader.csを修正してください。<br>
https://www.cgtrader.com/3d-model-collections/1400-people-crowds

⑤Python_Script.zipをDL<br>
https://github.com/tori29umai0123/CotrolNet_Shadow/releases/tag/CotrolNet_Shadow_V1.0<br>
image_converter.pyで画像加工<br>
generate_prompt.pyでタグ付けができます。<br>
導入方法は面倒くさいので割愛。各種ライブラリインストールしたら動くよ
