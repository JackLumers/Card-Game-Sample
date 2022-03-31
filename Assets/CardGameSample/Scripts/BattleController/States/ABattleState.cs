using UnityEngine;

namespace CardGameSample.Scripts.BattleController.States
{
    public abstract class ABattleState : ScriptableObject
    {
        protected BattleController BattleController;
        protected BattleStateMachine StateMachine;
        
        public void Initialize(BattleController controller, BattleStateMachine stateMachine)
        {
            BattleController = controller;
            StateMachine = stateMachine;
        }

        public abstract void ResetState();

        public abstract void Enter();

        public abstract void Exit();
    }
}