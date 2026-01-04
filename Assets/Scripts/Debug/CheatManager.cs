using UnityEngine;

#if ENABLE_CHEATS
public class CheatManager : MonoBehaviour
{
    public static CheatManager Instance { get; private set; }

    [Header("Cheat Toggles (Inspector or Hotkeys)")]
    [SerializeField] private bool playerInfiniteHealth = false;

    [Header("Hotkeys")]
    [SerializeField] private KeyCode togglePlayerHealthKey = KeyCode.I;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        //Debug.Log("<color=cyan><b>🧙‍♂️ CHEAT MANAGER LOADED! Press I for PLAYER-ONLY God Mode</b></color>");
    }

    private void Update()
    {
        if (Input.GetKeyDown(togglePlayerHealthKey))
        {
            playerInfiniteHealth = !playerInfiniteHealth;
            
            // LOUD CONSOLE + On-Screen feedback
            string status = playerInfiniteHealth ? "PLAYER GOD MODE ACTIVATED ✅" : "Player God Mode DEACTIVATED ❌";
            //Debug.Log($"<color=red><b>🚀 {status}</b></color>");
            //Debug.Log($"<color=green><b>Player Infinite Health: {playerInfiniteHealth}</b></color>");
        }
    }

    // PLAYER-ONLY check
    public bool PlayerInfiniteHealth => playerInfiniteHealth;
    
    // Keep old name for backward compatibility (enemies ignore this)
    public bool InfiniteHealth => false; // Always false for non-players
}
#endif