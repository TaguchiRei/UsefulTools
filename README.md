# Useful Tools for Unity

Unityでの開発を効率化するためのエディタ拡張ツールセットです。
プロジェクトの初期設定、コード生成、シーン管理、デバッグ支援など、多岐にわたる機能を統合しています。

## 🚀 はじめに

すべての設定は、Unityメニューの **UsefulTools > Settings** から一括で管理できます。

---

## 🛠 機能一覧

### 1. Project Support (プロジェクト支援)
プロジェクトの土台作りをサポートします。
- **Package Manager 連携**: UniTask, VContainer, Addressables などの主要パッケージをワンクリックでインポートできます。
- **フォルダ構造管理**: 規約に基づいた標準的なフォルダ構成を自動生成し、現在のプロジェクト構造を可視化します。

<details>
<summary>💡 使い方</summary>

1. **Settings > Project Support** タブを開きます。
2. **Package Import**: 必要なパッケージの横にある `Import` ボタンを押すと、プロジェクトに追加されます。
3. **Hierarchy Support**: `Apply Intended Structure` ボタンを押すと、プロジェクトルートに必要な基本フォルダ群（Art, Code, Audio等）が一括作成されます。
</details>

### 2. Scene Support (シーン管理)
シーン遷移や読み込みをタイプセーフに行うためのツールです。
- **Scene Enum 生成**: プロジェクト内のシーンを解析し、自動的に `SceneEnum.cs` を生成します。
- **自動更新**: アセットの変更を検知して自動的に Enum を更新可能です。

<details>
<summary>💡 使い方</summary>

1. **Settings > Scene Support** タブを開きます。
2. **Path Settings**: シーンの検索ルートと、Enumファイルの出力先を指定します。
3. **Generate Timing**: `On Asset Changed` に設定すると、シーンファイルを追加・削除した際に自動でEnumが更新されます。手動更新は `Generate Scene Enum Now` を押してください。
4. **コードでの利用**: `UsefulTools.AutoGenerate.InListSceneName.SampleScene.ToString()` のように利用できます。
</details>

### 3. Code Support (コーディング支援)
スクリプト作成の効率化とリファレンスへのアクセスを容易にします。
- **Code Generator**: テンプレートから名前空間や修飾子を指定してC#スクリプトを高速生成します。
- **ドキュメントリンク**: エディタから直接公式ドキュメントを開けます。

<details>
<summary>💡 使い方</summary>

1. **Settings > Code Support** タブを開きます。
2. `Open Code Generator` ボタンを押して生成ウィンドウを開きます。
3. クラス名を入力し、`Create Class` を押すと指定のフォルダにスクリプトが生成されます。
4. ウィンドウ内の `Reference URLs` セクションから、C#やUnity APIの公式リファレンスをワンクリックで開けます。
</details>

### 4. Input Support (入力システム支援)
Input System (Actions) をより使いやすくします。
- **Input Action Enum 生成**: `.inputactions` アセット内のアクション名を Enum 化します。

<details>
<summary>💡 使い方</summary>

1. **Settings > Input Support** タブを開きます。
2. `Scan & Generate All Input Enums` を実行すると、プロジェクト内のすべての `.inputactions` アセットに対応する Enum が生成されます。
3. これにより、マジック文字列を使わずに `PlayerInput.actions[ActionEnum.Fire.ToString()]` のようなアクセスが可能になります。
</details>

### 5. Debug Support (デバッグ支援)
実行時のデバッグを強力にサポートします。
- **Debug GUI**: FPSやログを画面上に表示するオーバーレイを生成します。

<details>
<summary>💡 使い方</summary>

1. **Settings > Debug Support** タブを開きます。
2. `Generate Debug GUI in Scene` ボタンを押すと、現在のシーンに `DebugGUI` オブジェクトが作成されます。
3. **表示設定**: 画面上の位置やフォントサイズを Settings 画面からリアルタイムに調整できます。
4. **ログ確認**: `Capture Application Logs` を有効にすると、通常の `Debug.Log` もゲーム画面上に表示されます。
</details>

---

## ✨ カスタムアトリビュート (Attributes)

<details>
<summary>💡 利用可能なアトリビュート一覧と書き方</summary>

- **`[SubclassSelector]`**: インターフェースや抽象クラスのフィールドに付与すると、インスペクターで実体化する子クラスをドロップダウンで選択できます。
  ```csharp
  [SerializeReference, SubclassSelector]
  private IEffect _effect;
  ```
- **`[ShowOnly]` / `[KeyReadOnly]`**: インスペクター上で値を表示のみ（編集不可）にします。
  ```csharp
  [ShowOnly] public int currentHp;
  ```
- **`[KeyGrouping]`**: 変数を視覚的な枠で囲み、グループ化して表示します。
  ```csharp
  [KeyGrouping] public string playerName;
  ```
</details>

---

## 📂 ディレクトリ構造

- `Art/`: 素材（マテリアル、モデル、テクスチャ）
- `Audio/`: 音響資産（BGM, SE）
- `Code/`: プログラム
    - `Attribute/`: カスタムアトリビュートの定義
    - `Editor/`: エディタ拡張本体
    - `Scripts/`: ランタイムスクリプト
    - `AutoGenerate/`: 自動生成されたコードの出力先
- `Data/`: 非アセットデータ（InputActions等）
- `Docs/`: ドキュメント
- `Level/`: シーンおよびプレハブ
