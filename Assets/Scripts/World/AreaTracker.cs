using System;
using UnityEngine;

// Sabe en que area esta el jugador y avisa cuando cambia. Las areas son sus hijos
public class AreaTracker : MonoBehaviour
{
    [SerializeField] private Transform target;

    private Area[] areas;
    private Area currentArea;

    public event Action<Area> OnAreaChanged;

    private void Awake()
    {
        areas = GetComponentsInChildren<Area>();
    }

    private void Update()
    {
        Refresh();
    }

    // Se pregunta por la posicion y no por triggers, asi volver atras sin salir del todo tambien cambia de area.
    // Publico para que la camara lo fuerce al reaparecer, sin esperar al proximo Update
    public void Refresh()
    {
        if (currentArea != null && currentArea.Contains(target.position)) return;

        foreach (Area area in areas)
        {
            if (area == currentArea || !area.Contains(target.position)) continue;

            currentArea = area;
            OnAreaChanged?.Invoke(area);
            return;
        }
    }
}
