using UnityEngine;

// AQUEST SCRIPT NOMÉS CONTROLA EL MOVIMENT I LA FÍSICA DE LA BOLA O JUGADOR.
// (Tota la lògica de punts, UI i menús s'ha mogut al GameController) un cop creat el script GameController.cs
public class PlayerController : MonoBehaviour
{
    Rigidbody rb; // Variable per guardar la referència al component físic (Rigidbody) del jugador, ja creat a la classe BouncingObject

    [Header("Movement Settings")] // Configuració de valors de moviment a l'Inspector
    [SerializeField] float speed = 10f; // Velocitat de rodatge de la pilota
    [SerializeField] float jumpForce = 5f; // Força del salt al xocar amb enemics
    [SerializeField] float fallThreshold = -10f; // Alçada límit per considerar que ha caigut

    [Header("Input References")]
    [SerializeField] Joystick joystick; // Referència al Joystick Virtual (necessària per llegir-ne els valors)
    
    // Referència al GameController per comunicar events (com agafar items o caure)
    [SerializeField] GameController gameController; 

    private bool useAccelerometre = false; // Aquesta variable ens la canvia el GameController segons què triï l'usuari

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Assignem el Rigidbody per poder aplicar forces físiques
        
        // Si no hem assignat el GameController manualment a l'Inspector, el busquem automàticament
        if (gameController == null)
        {
            gameController = FindFirstObjectByType<GameController>();
        }
    }

    // Aquesta funció és cridada pel GameController quan l'usuari prem Start
    // Li diu si ha de fer servir l'acceleròmetre (true) o el joystick (false)
    public void SetInputType(bool enableAccelerometer)
    {
        this.useAccelerometre = enableAccelerometer;
    }

    void Update()
    {
        // Update es crida a cada frame gràfic
        // Comprovem si ha caigut fora del mapa
        if (transform.position.y < fallThreshold)
        {
            // Si ha caigut, avisem al GameController perquè reiniciï la partida
            if (gameController != null) gameController.RestartGame();
        }
    }

    // FixedUpdate es fa servir per a tot el que són CÀLCULS DE FÍSICA (Forces, Velocitats)
    // S'executa a intervals de temps fixos i precisos
    private void FixedUpdate()
    {
        // ==========================================================================================
        // 1. INPUT DE TECLAT (Ordinador)
        // ==========================================================================================
        //GetAxis ja té suavitzat (smooth) i funciona amb fletxes i WASD
        float moveHorizontal = Input.GetAxis("Horizontal"); 
        float moveVertical = Input.GetAxis("Vertical");

        // ==========================================================================================
        // 2. INPUT DE ANDROID (Mòbil)
        // A quí mostrem DOS MÈTODES diferents de fer-ho.
        // ==========================================================================================

        // --- MÈTODE A: PREPROCESSADOR (#if UNITY_ANDROID) ---
        // Aquest codi és el MILLOR per al joc final (Optimització).
        // El símbol '#' indica que això no és codi normal, sinó instruccions per al Compilador.
        // Si estàs compilant per a PC, tot el que hi ha dins d'aquest bloc S'ESBORRA automàticament abans de crear el joc.
        // Si estàs compilant per a Android (APK), aquest codi s'hi inclou.
        // PER EXPORTAR: No has de tocar res. Unity detecta automàticament que està fent un APK i activa aquest codi.
        
        #if UNITY_ANDROID
            // Joystick Virtual
            if (joystick != null) 
            {
                // Sumem l'input del joystick al que ja tenim (si n'hi ha)
                moveHorizontal += joystick.Horizontal;
                moveVertical += joystick.Vertical;
            }

            // Acceleròmetre (Només si l'usuari ho ha triat al menú)
            if (useAccelerometre)
            {
                // L'acceleròmetre mesura la inclinació del telèfon.
                // .x és inclinar esquerra/dreta.
                // .y és inclinar endavant/enrere (en apaïsant).
                moveHorizontal += Input.acceleration.x;
                moveVertical += Input.acceleration.y;
            }
        #endif

        // --- MÈTODE B: RUNTIME CHECK (Application.platform) ---
        // Aquest codi s'executa SEMPRE, però té un 'if' normal que comprova on estem jugant.
        // ÉS DOLENT per rendiment final (fer un 'if' a cada frame és innecessari si ja saps que estàs al mòbil).
        // PERÒ ÉS ÚTIL si vols provar coses a l'Editor simulant que ets un mòbil (usant eines com Unity Remote).
        
        // IMPORTANT PER EXPORTAR: 
        // Si deixes això activat I el mètode A activat, quan juguis al mòbil es sumaran els dos!! 
        // El jugador anirà al doble de velocitat.
        // RECOMANACIÓ: Comenta aquest bloc sencer (posa // o /* */) abans de fer la Build final per Android.
        
        /*
        if (Application.platform == RuntimePlatform.Android) 
        {
             if (joystick != null) 
            {
                moveHorizontal += joystick.Horizontal;
                moveVertical += joystick.Vertical;
            }
        }
        */

        // ==========================================================================================
        // 3. APLICAR MOVIMENT
        // ==========================================================================================
        
        // Creem el vector final amb tota la suma d'inputs
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        
        // Empenyem la bola
        rb.AddForce(movement * speed);
    }

    // Gestió de xocs físics
    private void OnCollisionEnter(Collision collision)
    {
        // Si xoquem amb un item
        if (collision.gameObject.CompareTag("Items"))
        {
            collision.gameObject.SetActive(false); // L'amaguem
            
            // Avisem al GameController: "He agafat un item!"
            if (gameController != null)
            {
                gameController.ItemCollected();
            }
        }

        // Si xoquem amb un enemic (Fantasmes/Boles)
        if (collision.gameObject.CompareTag("Hazards"))
        {
            // Aquí no cal avisar al GameController, el salt es concecuencia del xoc
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
