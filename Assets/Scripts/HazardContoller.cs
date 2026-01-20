using UnityEngine;

// CLASSE FILLA: HAZARD (Enemic/Fantasma)
// Hereta de BouncingObject, així que ja té tot el moviment i rebot
public class HazardContoller : BouncingObject
{
    // Update és únic del Hazard perquè volem que els ulls mirin cap on vagi
    void Update()
    {
        // Puc accedir a 'rb' perquè l'he fet public a la classe pare BouncingObject
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rb.linearVelocity.normalized);
            // Rotació suau (Slerp)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}
