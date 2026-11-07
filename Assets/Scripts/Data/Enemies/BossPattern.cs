using System;
using System.Collections.Generic;
using UnityEngine;

// Una cadena de ataques del jefe, salen uno detras de otro como el combo del jugador
[Serializable]
public class BossPattern
{
    [Tooltip("Attack state names in order, e.g. Attack1, Attack2. The second phase adds the Flaming suffix by itself")]
    [SerializeField] private List<string> steps = new List<string>();
    [SerializeField] private bool phaseTwoOnly;

    // Unity lo necesita para que un patron nuevo en el inspector arranque vacio
    public BossPattern() { }

    // Para cargar los valores iniciales desde codigo
    public BossPattern(bool phaseTwoOnly, params string[] steps)
    {
        this.phaseTwoOnly = phaseTwoOnly;
        this.steps = new List<string>(steps);
    }

    public IReadOnlyList<string> GetSteps() => steps;
    public bool GetIsPhaseTwoOnly() => phaseTwoOnly;
}
