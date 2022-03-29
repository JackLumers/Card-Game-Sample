using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CardGameSample.Scripts.Input
{
    public interface IHoldableItem
    {
        public void Hold(bool hold);

        public UniTask Move(Vector2 screenPointerPosition);
    }
}