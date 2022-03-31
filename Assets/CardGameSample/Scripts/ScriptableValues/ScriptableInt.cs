using System;
using UnityEngine;

namespace CardGameSample.Scripts.ScriptableValues
{
    [CreateAssetMenu(fileName = "NewScriptableInt", menuName = "Scriptable Values/Integer")]
    public class ScriptableInt : ScriptableObject
    {
        [SerializeField] private int value;

        public int Value
        {
            get => value;
            set
            {
                this.value = value;
                Changed?.Invoke(value);
            }
        }

        public Action<int> Changed;
    }
}