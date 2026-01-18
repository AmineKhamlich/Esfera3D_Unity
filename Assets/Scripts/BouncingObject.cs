using UnityEngine;

// CLASSE PARE (HERÈNCIA)
// Aquesta classe conté tota la lògica compartida de moviment i rebot.
// "abstract" vol dir que no pots posar aquest script directament a un objecte,
// has de crear un fill (com Hazard o Item) que l'hereti.
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public abstract class BouncingObject : MonoBehaviour
{
    [Header("Configuració Base")]
    [SerializeField] protected float speed = 3.5f; // 'protected' vol dir que els fills hi poden accedir

    protected Rigidbody rb;

    // 'virtual' permet que els fills (Hazards/Items) afegeixin més codi al seu Awake si ho necessiten
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 1. Configuració Física Comuna
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        // 2. Impuls Inicial Aleatori
        Vector3 initialDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        if (initialDir == Vector3.zero) initialDir = Vector3.forward;
        
        rb.linearVelocity = initialDir * speed;
    }

    // Aquesta funció es igual per tots, així que no cal fer-la 'virtual' (opcional)
    protected void FixedUpdate()
    {
        // Mantenir velocitat constant
        Vector3 currentVel = rb.linearVelocity;
        currentVel.y = 0f; // Assegurar pla horitzontal

        if (currentVel.sqrMagnitude < 0.1f)
        {
             // Si s'ha parat, re-arrancar
             Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
             currentVel = randomDir * speed;
        }
        else
        {
            // Normalitzar i aplicar velocitat exacta
            currentVel = currentVel.normalized * speed;
        }

        rb.linearVelocity = currentVel;
    }

    // Lògica per no quedar-se enganxat a les parets
    protected void OnCollisionStay(Collision collision)
    {
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
