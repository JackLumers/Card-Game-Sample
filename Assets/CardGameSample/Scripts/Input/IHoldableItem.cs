using UnityEngine;

namespace CardGameSample.Scripts.Input
{
    public interface IHoldableItem
    {
        public void Hold(bool hold);

        public void Move(Vector2 screenPointerPosition);
    }
}