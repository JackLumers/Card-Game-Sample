namespace CardGameSample.Scripts.BattleController.States
{
    public class BattleStateMachine
    {
        public ABattleState CurrentState { get; private set; }

        public void ResetState()
        {
            CurrentState?.ResetState();
            CurrentState = null;
        }
        
        public void Initialize(ABattleState startingState)
        {
            CurrentState = startingState;
            startingState.Enter();
        }

        public void ChangeState(ABattleState newState)
        {
            CurrentState.Exit();

            CurrentState = newState;
            newState.Enter();
        }
    }
}