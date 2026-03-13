# UsefulTools

Unityでのゲーム開発を円滑に進めるための、エディタ拡張および基礎基盤ツール群です。
包括的な設定ウィンドウ（UsefulToolsSettings）を中心に、プロジェクトの初期設定からコード生成、シーン管理までを強力にサポートします。

## 主な機能

### 1. UsefulTools Settings (設定ウィンドウ)
すべてのツールの設定を一箇所で管理できます。
*   **Project Support**: フォルダ構造の視覚的確認、一括作成、および主要パッケージ（UniTask, VContainer等）の導入。
*   **Code Support**: コード生成時のデフォルト設定（ネームスペース、修飾子等）の管理。
*   **Scene Support**: シーン作成時のEnum自動生成タイミングや、検索パスの設定。
*   **Input Support**: Input Systemの `.inputactions` からのEnum自動生成設定。
*   **Debug Support**: デバッグGUIのフォントサイズや、実行時ログ取得の有効化設定。

### 2. Scene Support (シーン管理)
*   **Scene Enum Generator**: プロジェクト内のシーンを自動的にスキャンし、`InListSceneName` / `OutListSceneName` Enumを生成します。
*   **Scene Loader**: 生成されたEnumを元に、プロジェクト内のシーンを階層構造を維持したまま素早く検索・ロードできます。

### 3. Input Support (入力管理)
*   **Input Enum Generator**: `.inputactions` アセットから ActionMap や Action 名を Enum として自動生成します。コード上での文字列指定を排除し、型安全な入力を実現します。

### 4. Code Generator (コード生成)
*   クラス、MonoBehaviour、ScriptableObject、EditorWindowなどの雛形を、設定したルールに基づいて素早く生成します。

### 5. Debug GUI
*   実行時の画面上にログや情報を表示するための軽量なGUIツールです。

## ディレクトリ構成（標準）
*   `Assets/Code/Editor`: エディタ拡張本体
*   `Assets/Code/Scripts`: 実行時スクリプト
*   `Assets/Code/AutoGenerate`: 自動生成されたコード（Enum等）
*   `Assets/Level/Scenes`: シーンファイルの配置場所（標準）

## 導入方法
1. `Assets` フォルダに必要なファイルを配置します。
2. 上部メニューの `Window > UsefulTools > Settings` を開き、プロジェクトに合わせて各パスや設定を調整してください。
3. `Project Support` タブから必要なパッケージを導入し、フォルダ構造を初期化することをお勧めします。
