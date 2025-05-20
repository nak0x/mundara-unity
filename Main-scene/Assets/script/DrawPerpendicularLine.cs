using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class DrawPerpendicularLine : MonoBehaviour
{
    public GameObject sourceObject;      // L'objet d'origine
    public GameObject screenObject;      // L'objet cible
    public GameObject playerHead;
    public GameObject objecttoMove;

    private float angleWithYAxis;        // L'angle par rapport à l'axe Y

    public bool autoUpdateAngleOfTarget = false;

    public GameObject kinect;
    public bool drawKinectLine = true   ;

    void Update()
    {

        if (autoUpdateAngleOfTarget)
        {
            List<Vector3> pointsOfObject = drawPerpendicularLineBetween(sourceObject, screenObject, Color.red);
            //getAngleBetweenVector(pointsOfObject[1] - pointsOfObject[0], Vector3.up);

            //List<Vector3> pointsOfHead = drawPerpendicularLineBetween(playerHead, screenObject, Color.green);
            Debug.DrawLine(playerHead.transform.position, sourceObject.transform.position, Color.green);
            //getAngleBetween(playerHead.transform.position, sourceObject.transform.position, Vector3.up);

            float angle = getAngleBetweenLines(sourceObject.transform.position, playerHead.transform.position, pointsOfObject[0], pointsOfObject[1]);
            objecttoMove.transform.eulerAngles = new Vector3(angle, 0, 0);
        }

        if (drawKinectLine)
        {
            Debug.DrawLine(kinect.transform.position, Vector3.forward * 10, Color.magenta);
        }

        
    }

    // Méthode pour accéder à l'angle depuis d'autres scripts
    public float GetAngleWithYAxis()
    {
        return angleWithYAxis;
    }

    public List<Vector3> drawPerpendicularLineBetween(GameObject source, GameObject target, Color colorLine)
    {

        // Position de départ (votre premier objet)
        Vector3 startPoint = source.transform.position;

        // Position de l'objet cible
        Vector3 targetPosition = target.transform.position;

        // Direction de l'objet source vers l'objet cible
        Vector3 directionToTarget = targetPosition - startPoint;

        // Calculer le point le plus proche sur l'objet cible (projection)
        Vector3 closestPoint = Vector3.Project(directionToTarget, target.transform.forward);

        // Direction perpendiculaire
        Vector3 perpendicularDirection = directionToTarget - closestPoint;
        perpendicularDirection.Normalize();

        // Point d'arrivée de la ligne perpendiculaire
        Vector3 endPoint = startPoint + perpendicularDirection * 5f;

        Debug.DrawLine(startPoint, endPoint, colorLine);


        List<Vector3> points = new List<Vector3>();
        points.Add(startPoint);
        points.Add(endPoint);
        return points;
    }

    public float getAngleBetween(Vector3 startPoint, Vector3 endPoint, Vector3 axisComparaison)
    {
        // Calculer l'angle par rapport à l'axe Y
        Vector3 lineDirection = endPoint - startPoint;
        Vector3 yAxis = axisComparaison; // L'axe Y dans Unity

        // Calculer l'angle entre la ligne et l'axe Y (en degrés)
        angleWithYAxis = Vector3.Angle(lineDirection, yAxis);

        // Si vous voulez savoir dans quelle direction est l'angle (horaire ou anti-horaire)
        Vector3 cross = Vector3.Cross(yAxis, lineDirection);
        if (cross.z < 0)
            angleWithYAxis = 360 - angleWithYAxis;

        //Debug.Log("Angle avec l'axe Y: " + angleWithYAxis + " degrés");

        return angleWithYAxis;
    }

    public float getAngleBetweenVector(Vector3 Vector1, Vector3 vector2)
    {
        // Calculer l'angle entre la ligne et l'axe Y (en degrés)
        angleWithYAxis = Vector3.Angle(Vector1, vector2);

        // Si vous voulez savoir dans quelle direction est l'angle (horaire ou anti-horaire)
        Vector3 cross = Vector3.Cross(vector2, Vector1);
        if (cross.z < 0)
            angleWithYAxis = 360 - angleWithYAxis;

        //Debug.Log("Angle avec l'axe Y: " + angleWithYAxis + " degrés");

        return angleWithYAxis;
    }

    public float getAngleBetweenLines(Vector3 line1Start, Vector3 line1End, Vector3 line2Start, Vector3 line2End)
    {
        // Calculer les vecteurs directeurs des deux lignes
        Vector3 direction1 = (line1End - line1Start).normalized;
        Vector3 direction2 = (line2End - line2Start).normalized;

        // Calculer l'angle entre les deux vecteurs directeurs (en degrés)
        float angle = Vector3.Angle(direction1, direction2);

        // Si vous voulez déterminer le sens de rotation (horaire ou anti-horaire)
        Vector3 cross = Vector3.Cross(direction1, direction2);

        // Déterminer le signe de l'angle en fonction de la direction du produit vectoriel
        // par rapport à un axe de référence (ici on utilise l'axe Y, mais vous pouvez changer)
        if (Vector3.Dot(cross, Vector3.up) < 0)
            angle = 360 - angle;

        //Debug.Log("Angle entre les deux lignes: " + angle + " degrés");

        return angle;
    }
}