using UnityEngine;
using UnityEngine.SceneManagement; // Importar gestor d'escenes
using TMPro; // Importar TextMeshPro per UI avançada

// Aquesta classe és el "Cervell" del joc.
// Gestiona tot el que no sigui moure la bola: Punts, Menús, Regles, Victòria.
public class GameController : MonoBehaviour
{
    // ==========================================================
    // REFERÈNCIES UI: Cal arrossegar els objectes des de Unity
    // ==========================================================
    [Header("UI References")]
    [SerializeField] GameObject startPanel;     // El panell gris que tapa el joc al principi
    [SerializeField] TMP_InputField nameInput;  // La caixa on s'escriu el nom
    [SerializeField] TMP_Text winText;          // El text gran de "GUANYA!"
    [SerializeField] TMP_Dropdown controlDropdown; // El desplegable per triar tipus de control

    // ==========================================================
    // ELEMENTS DEL JOC
    // ==========================================================
    [Header("Game Elements")]
    [SerializeField] Joystick joystick;         // Referència al Joystick visual per poder amagar-lo si cal
    [SerializeField] PlayerController player;   // Referència al Jugador per passar-li la configuració triada

    // ==========================================================
    // ESTAT INTERN
    // ==========================================================
    private int itemCount;      // Quants items queden a l'escena?
    private string playerName;  // Com es diu el jugador?

    void Start()
    {
        // 1. Comptar Items
        // Busquem tots els objectes amb tag "Items" per saber quan s'acaba el joc
        itemCount = GameObject.FindGameObjectsWithTag("Items").Length;
        Debug.Log("GameController: Items to collect: " + itemCount);

        // 2. Netejar Visuals
        // Assegurar que el text de victòria no es veu quan comencem
        if (winText != null) winText.gameObject.SetActive(false);
        
        // 3. Mostrar Menú d'Inici
        if (startPanel != null)
        {
            startPanel.SetActive(true); // Ensenya el menú
            Time.timeScale = 0f;        // PAUSA EL TEMPS: Així res es mou de fons mentre tries el nom
        }
    }

    // Aquesta funció s'executa quan cliques el botó "START" del menú
    public void StartGame()
    {
        // A. Guardar el Nom
        if (nameInput != null)
        {
            playerName = nameInput.text;
            // Si el jugador és mandrós i no posa nom, li diem "Player"
            if (string.IsNullOrEmpty(playerName)) playerName = "Player";
        }

        // B. Llegir i Configurar els Controls
        // Mirem què ha triat l'usuari al Dropdown (Selector)
        bool useAccelerometer = false; // Per defecte desactivat
        
        if (controlDropdown != null)
        {
            if (controlDropdown.value == 0) // Opció 0 = Joystick
            {
                useAccelerometer = false;
                // Si fem servir Joystick, ens assegurem que es vegi a la pantalla
                if (joystick != null) joystick.gameObject.SetActive(true);
            }
            else if (controlDropdown.value == 1) // Opció 1 = Acceleròmetre
            {
                useAccelerometer = true;
                // Si fem servir inclinació, el Joystick visual molesta, així que l'amaguem
                if (joystick != null) joystick.gameObject.SetActive(false);
            }
        }

        // C. Enviar la configuració al Player
        // Li diem al script del jugador com s'ha de comportar
        if (player != null)
        {
            player.SetInputType(useAccelerometer);
        }

        // D. Començar el Joc!
        if (startPanel != null) startPanel.SetActive(false); // Amaguem el menú
        Time.timeScale = 1f; // TORNEM A ARRENCAR EL TEMPS. Ara tot es comença a moure.
    }

    // Aquesta funció és pública perquè el Player la cridi cada cop que toca un Item
    public void ItemCollected()
    {
        itemCount--; // Restem 1
        
        Debug.Log("Item retrieved! Remaining: " + itemCount);

        // Comprovem si ja hem guanyat
        if (itemCount <= 0)
        {
            WinGame();
        }
    }

    // Aquesta funció és pública perquè el Player la cridi si cau pel forat
    public void RestartGame()
    {
        // Recarrega l'escena sencera, tornant tot a l'estat inicial (Start)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Lògica interna de Victòria
    private void WinGame()
    {
        Debug.Log("YOU WIN!");
        Time.timeScale = 0f; // PAUSA EL JOC (Congela l'acció per celebrar)

        if (winText != null)
        {
            // Construim el missatge personalitzat
            winText.text = playerName + " Wins!";
            // El mostrem
            winText.gameObject.SetActive(true);
        }
    }
}
