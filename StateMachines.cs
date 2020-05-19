using Stateless;
using State;

namespace General {
    class StateMachines {
        public static StateMachine<CharStates, CharTriggers> PlayerStateMachine() {

            StateMachine<CharStates, CharTriggers> machine =
                    new StateMachine<CharStates, CharTriggers>(CharStates.IDLE);

            #region State Flow

            // Idle <-> Running
            machine.Configure(CharStates.IDLE)
                .Permit(CharTriggers.RUN_START, CharStates.RUNNING);
            machine.Configure(CharStates.IDLE)
                .Ignore(CharTriggers.RUN_END);

            machine.Configure(CharStates.RUNNING)
                .Permit(CharTriggers.RUN_END, CharStates.IDLE);
            machine.Configure(CharStates.RUNNING)
                .Ignore(CharTriggers.RUN_START);


            // Idle -> Jumping
            machine.Configure(CharStates.IDLE)
                .Permit(CharTriggers.JUMP_START, CharStates.JUMPING);

            // Running -> Jumping
            machine.Configure(CharStates.RUNNING)
                .Permit(CharTriggers.JUMP_START, CharStates.JUMPING);

            // Jumping -> Falling 
            machine.Configure(CharStates.JUMPING)
                .Permit(CharTriggers.FALL_START, CharStates.FALLING);

            // Falling -> Idle
            machine.Configure(CharStates.FALLING)
                .Permit(CharTriggers.FALL_END, CharStates.IDLE);

            // Falling -> Running
            machine.Configure(CharStates.FALLING)
                .Permit(CharTriggers.FALL_END, CharStates.RUNNING);
            #endregion

            #region Behavior

            machine.Configure(CharStates.IDLE)
                    .OnEntry(Actions.IdleBehavior);
            machine.Configure(CharStates.RUNNING)
                    .OnEntry(Actions.RunBehavior);
            machine.Configure(CharStates.JUMPING)
                    .OnEntry(Actions.JumpBehavior);
            machine.Configure(CharStates.FALLING)
                    .OnEntry(Actions.FallBehavior);
            #endregion

            return machine;
        }
    }
}
