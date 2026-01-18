using UnityEngine;

// CLASSE FILLA: ITEM (Píndola)
// Hereta de BouncingObject.
// L'única diferència amb el Hazard és que ha de tenir molt poca massa.
public class ItemController : BouncingObject
{
    // 'override' vol dir: "Vull fer servir la versió del pare, però afegint-hi coses meves"
    protected override void Awake()
    {
        // 1. Primer fem tot el que fa el pare (configurar Rigidbody, impuls, etc.)
        base.Awake();

        // 2. Després apliquem la nostra personalització
        rb.mass = 0.001f; // Massa tiny perquè el jugador no noti el xoc
    }

    // No cal res més! FixedUpdate i OnCollisionStay ja funcionen sols gràcies a l'herència.
}
