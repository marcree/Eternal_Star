using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName ="Event/CharacterEventSO")]

public class CharacterEventSO : ScriptableObject
{
    public UnityAction<Character> OnEventRaised;
    public void RaisedEvent(Character character)
    {
        OnEventRaised?.Invoke(character);
    }
}
