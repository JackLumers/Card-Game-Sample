using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CardGameSample.Scripts.Input
{
    public interface IHoldableObject
    {
        public void Hold(Vector2 screenPointerPosition, bool hold);
        
        public UniTask MoveByHold(Vector2 localPosition, float speed);
        
        public GameObject GameObject { get; }
    }
}