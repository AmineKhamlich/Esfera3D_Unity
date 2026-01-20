using UnityEngine;

// CLASSE PARE (HERÈNCIA)
// Aquesta classe conté tota la lògica compartida de moviment i rebot.

// que fa aquesta linia de codi? 
// El RequireComponent s'assegura que qualsevol GameObject que tingui aquest script també tingui un Rigidbody i un Collider.
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BouncingObject : MonoBehaviour
{
    [Header("Configuració Base")]
    // 'public' fa que sigui fàcil de veure i modificar des de qualsevol lloc també de l'Inspector
    public float speed = 3.5f;

    // 'public' perquè les classes filles (Item, Hazard) puguin fer servir 'rb' sense problemes
    public Rigidbody rb;

    // S'executa automàticament quan comença el joc.
    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 1. Configuració Física Comuna
        rb.useGravity = false;
        // Bloquejar eix Y i rotacions per evitar girs no desitjats
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        // 2. Impuls Inicial Aleatori
        Vector3 VelocitatInicial = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

        // Assegurar que no sigui zero i si es zero, posar cap endavant un alre cop
        // 
        if (VelocitatInicial == Vector3.zero) VelocitatInicial = Vector3.forward;

        // Aplicar velocitat inicial
        rb.linearVelocity = VelocitatInicial * speed;
    }

    // S'executa a intervals fixos per a la física
    void FixedUpdate()
    {
        // Mantenir velocitat constant
        Vector3 VelocitatConstant = rb.linearVelocity;
        VelocitatConstant.y = 0f; // Assegurar pla horitzontal

        // Comprovar si s'ha aturat completament
        if (VelocitatConstant.sqrMagnitude < 0.1f)
        {
             // Si s'ha parat, re-arrancar
             Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
            // Poso velocitat per direcció aleatòria
            VelocitatConstant = randomDir * speed;
        }
        else
        {
            // Normalitzar i aplicar velocitat exacta
            VelocitatConstant = VelocitatConstant.normalized * speed;
        }

        // Actualitzar velocitat al linearVelocity del Rigidbody
        rb.linearVelocity = VelocitatConstant;
    }

    // Lògica per no quedar-se enganxat a les parets
    void OnCollisionStay(Collision collision)
    {
        // Iterar per tots els punts de contacte
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.y) > 0.5f) continue; // Ignorar terra

            Vector3 vel = rb.linearVelocity;
            if (vel.sqrMagnitude < 0.1f) return; 

            // Detectar lliscament paral·lel
            float dot = Vector3.Dot(vel.normalized, contact.normal);

            if (Mathf.Abs(dot) < 0.25f)
            {
                // Empenta cap enfora
                Vector3 newDir = (vel.normalized + contact.normal * 0.5f).normalized;
                rb.linearVelocity = newDir * speed;
                break;
            }
        }
    }
}
