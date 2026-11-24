using UnityEngine;
using CyberEscape.Player;
using CyberEscape.Environment;
using CyberEscape.UI;
using CyberEscape.Audio;

namespace CyberEscape.Core
{
    /// <summary>
    /// GameInitializer ensures that if any core system, player, lighting, or ground spawner is missing,
    /// it dynamically constructs and links them at runtime.
    /// This makes the project 100% plug-and-play in any Unity version!
    /// </summary>
    public class GameInitializer : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            // Ensure essential singletons exist early
            if (FindObjectOfType<GameManager>() == null)
            {
                GameObject gmObj = new GameObject("GameManager");
                gmObj.AddComponent<GameManager>();
            }

            if (FindObjectOfType<SoundManager>() == null)
            {
                GameObject smObj = new GameObject("SoundManager");
                smObj.AddComponent<SoundManager>();
            }
        }

        private void Awake()
        {
            SetupEnvironment();
        }

        public static void SetupEnvironment()
        {
            // 1. Lighting & Sky
            if (RenderSettings.ambientLight != Color.white)
            {
                RenderSettings.ambientLight = new Color(0.25f, 0.28f, 0.35f);
            }

            Light dirLight = FindObjectOfType<Light>();
            if (dirLight == null)
            {
                GameObject lightObj = new GameObject("Directional Light");
                Light l = lightObj.AddComponent<Light>();
                l.type = LightType.Directional;
                l.color = new Color(1f, 0.95f, 0.85f);
                l.intensity = 1.2f;
                lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }

            // 2. Materials
            Shader standardShader = Shader.Find("Standard");
            if (standardShader == null) standardShader = Shader.Find("Mobile/Diffuse");
            if (standardShader == null) standardShader = Shader.Find("Diffuse");

            Material playerMat = CreateMaterial(standardShader, new Color(0.1f, 0.8f, 1f));     // Cyan
            Material roadMat = CreateMaterial(standardShader, new Color(0.12f, 0.12f, 0.15f));   // Dark asphalt
            Material railMat = CreateMaterial(standardShader, new Color(0.9f, 0.1f, 0.7f));     // Neon Magenta
            Material obsMat = CreateMaterial(standardShader, new Color(1f, 0.2f, 0.3f));        // Neon Red/Pink
            Material coinMat = CreateMaterial(standardShader, new Color(1f, 0.85f, 0.1f));      // Gold
            Material shieldMat = CreateMaterial(standardShader, new Color(0f, 0.9f, 1f, 0.4f), true); // Translucent Cyan
            Material powerUpMat = CreateMaterial(standardShader, new Color(0.2f, 1f, 0.4f));    // Neon Green

            // 3. Player
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj == null)
            {
                playerObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                playerObj.name = "Player";
                playerObj.tag = "Player";
                playerObj.transform.position = new Vector3(0, 1f, 0);
                playerObj.transform.localScale = new Vector3(0.9f, 1.0f, 0.9f);
                playerObj.GetComponent<MeshRenderer>().material = playerMat;

                // CharacterController
                CharacterController cc = playerObj.AddComponent<CharacterController>();
                cc.center = new Vector3(0, 0, 0);
                cc.radius = 0.45f;
                cc.height = 2f;

                // Remove primitive collider (CharacterController handles it)
                CapsuleCollider primitiveCol = playerObj.GetComponent<CapsuleCollider>();
                if (primitiveCol != null && primitiveCol != cc)
                {
                    Destroy(primitiveCol);
                }

                // PlayerController
                PlayerController pc = playerObj.AddComponent<PlayerController>();
                pc.SetMeshRenderer(playerObj.GetComponent<MeshRenderer>());

                // Shield Visual Sphere
                GameObject shieldOrb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                shieldOrb.name = "ShieldVisual";
                shieldOrb.transform.SetParent(playerObj.transform);
                shieldOrb.transform.localPosition = Vector3.zero;
                shieldOrb.transform.localScale = Vector3.one * 1.5f;
                Destroy(shieldOrb.GetComponent<Collider>());
                shieldOrb.GetComponent<MeshRenderer>().material = shieldMat;
                shieldOrb.SetActive(false);
                pc.SetShieldVisual(shieldOrb);
            }

            // 4. Main Camera
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                cam = camObj.AddComponent<Camera>();
                cam.tag = "MainCamera";
                camObj.AddComponent<AudioListener>();
            }

            CameraFollow camFollow = cam.GetComponent<CameraFollow>();
            if (camFollow == null)
            {
                camFollow = cam.gameObject.AddComponent<CameraFollow>();
            }
            camFollow.SetTarget(playerObj.transform);

            // 5. Ground Looper & Spawner
            GroundLooper looper = FindObjectOfType<GroundLooper>();
            if (looper == null)
            {
                GameObject looperObj = new GameObject("GroundLooper");
                looper = looperObj.AddComponent<GroundLooper>();
                looper.InitializeMaterials(roadMat, railMat, obsMat, coinMat, shieldMat, powerUpMat);
            }

            // 6. UI Manager
            if (FindObjectOfType<UIManager>() == null)
            {
                GameObject uiObj = new GameObject("UIManager");
                uiObj.AddComponent<UIManager>();
            }
        }

        private static Material CreateMaterial(Shader shader, Color color, bool transparent = false)
        {
            Material mat = new Material(shader);
            mat.color = color;
            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", color * 0.35f);
            }

            if (transparent)
            {
                mat.SetFloat("_Mode", 3); // Transparent
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }

            return mat;
        }
    }
}
