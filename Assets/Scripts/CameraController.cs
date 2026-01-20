using UnityEngine;

// Script perquè la càmera segueixi al jugador mantenint la distància
public class CameraController : MonoBehaviour
{
    // Variable pública (o serialitzada) per assignar el Jugador des de l'Inspector
    [SerializeField]
    GameObject player;

    // Variable privada per guardar la distància (offset) inicial entre càmera i jugador
    private Vector3 offset;          

    // Start es crida a l'inici del joc
    void Start()
    {
        // Calculem la distància inicial: Posició Càmera - Posició Jugador
        // Això ens diu a quina distància i angle estem del jugador al principi
        offset = this.transform.position - player.transform.position;
    }

    // LateUpdate es crida després que s'hagin processat tots els Update
    // És ideal per a càmeres, perquè ens assegurem que el jugador ja s'ha mogut del tot
    void LateUpdate()
    {
        // Actualitzem la posició de la càmera:
        // Nova posició = Posició actual del jugador + la distància inicial (offset)
        // Així la càmera es mou exactament igual que el jugador, mantenint la perspectiva
        this.transform.position = player.transform.position + offset;
    }
}
