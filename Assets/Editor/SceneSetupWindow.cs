#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using CyberEscape.Core;

namespace CyberEscape.Editor
{
    public static class SceneSetupWindow
    {
        [MenuItem("Tools/Cyber Escape/Setup Complete Scene", false, 1)]
        public static void SetupScene()
        {
            // Prompt confirmation
            if (!EditorUtility.DisplayDialog("Setup Cyber Escape Scene",
                "Thao tác này sẽ thiết lập toàn bộ GameObjects, Camera, Player, Lighting và Ground Looper vào Scene hiện tại.\n\nBạn có muốn tiếp tục?",
                "Đồng ý (Setup)", "Hủy"))
            {
                return;
            }

            // Create or get Initializer
            GameInitializer init = Object.FindObjectOfType<GameInitializer>();
            if (init == null)
            {
                GameObject initObj = new GameObject("GameInitializer");
                init = initObj.AddComponent<GameInitializer>();
            }

            GameInitializer.SetupEnvironment();

            // Mark scene dirty and save
            Scene currentScene = SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(currentScene);

            string scenePath = "Assets/Scenes/MainScene.unity";
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }

            EditorSceneManager.SaveScene(currentScene, scenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Thành công!", 
                $"Đã hoàn tất thiết lập Scene Cyber Escape tại:\n{scenePath}\n\nBạn chỉ cần nhấn nút Play để trải nghiệm game ngay!", "Tuyệt vời!");
        }
    }
}
#endif
