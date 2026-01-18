using UnityEngine;

// CLASSE FILLA: HAZARD (Enemic/Fantasma)
// Hereta de BouncingObject, així que ja té tot el moviment i rebot inclòs automàticament.
// Només afegim el que és únic pels Fantasmes: Rotació Visual.
public class HazardContoller : BouncingObject
{
    // NO cal reescriure Awake, FixedUpdate ni OnCollisionStay, perquè ja els té el pare (BouncingObject)

    // Update és únic del Hazard perquè volem que els ulls mirin cap on vagi
    void Update()
    {
        // Només si es mou, girem el model visualment
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rb.linearVelocity.normalized);
            // Rotació suau (Slerp)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}
