using UnityEngine;

#if ENABLE_CHEATS
public class CheatManager : MonoBehaviour
{
    public static CheatManager Instance { get; private set; }

    [Header("Cheat Toggles (Inspector or Hotkeys)")]
    [SerializeField] private bool infiniteHealth = false;

    [Header("Hotkeys")]
    [SerializeField] private KeyCode toggleInfiniteHealthKey = KeyCode.I;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleInfiniteHealthKey))
        {
            infiniteHealth = !infiniteHealth;
            Debug.Log($"🛡️ Infinite Health: {(infiniteHealth ? "ON ✅" : "OFF ❌")}");
        }
    }

    public bool InfiniteHealth => infiniteHealth;
}
#endif