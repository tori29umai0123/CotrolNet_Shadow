# CotrolNet_Shadow
特定のディレクトリに入った3Dモデルと光源の向き情報をレンダリングするだけのやつ。<br>
CotrolNet用のデータセットを作る為に作った。
分かっている人だけ使ってください

# 使い方
①3Dモデルをobjかfbxに変換して、0001で始まる四桁の数字名にして、Assets\Resourcesフォルダ以下に配置<br>

②unitypackageをインポート<br>
https://github.com/tori29umai0123/CotrolNet_Shadow/releases/tag/CotrolNet_Shadow_V1.1<br>

③CotrolNet_Shadowシーンを開いて、Hierarchy→System→Camera_Captureを設定する<br>
outpath：出力先<br>
modelpath：Resourcesフォルダ一<br>
max_count：出力画像の枚数<br>

④Unityを実行すると画像が出力されるはず<br>
読みこむ3Dモデルを以下のものに最適化しているので、座標とかおかしくなる場合はCamera_Capture.csを修正してください。<br>
https://www.cgtrader.com/3d-model-collections/1400-people-crowds<br>
https://www.myminifactory.com/category/SMK-Statens-museum-for-Kunst-The-National-Gallery-of-Denmark

⑤Python_Script.zipをDL<br>
https://github.com/tori29umai0123/CotrolNet_Shadow/releases/tag/CotrolNet_Shadow_V1.0<br>
image_converter.pyで画像加工<br>
generate_prompt.pyでタグ付けができます。<br>
導入方法は面倒くさいので割愛。各種ライブラリインストールしたら動くよ
