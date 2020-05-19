using System;
namespace State {
    public enum CharStates {
        IDLE,
        RUNNING,
        JUMPING,
        FALLING
    }
    public enum CharTriggers {
        RUN_START,
        RUN_END,
        JUMP_START,
        FALL_START,
        FALL_END
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
