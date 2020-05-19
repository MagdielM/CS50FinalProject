using Automatonymous;
using System;
namespace States {
    public class CharacterState {
        public State Current { get; set; }
    }

    #region Actions
    public static class Actions {
        public static Action IdleBehavior;
        public static Action RunBehavior;
        public static Action JumpBehavior;
        public static Action FallBehavior;
    }
    #endregion
}
