using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Game1 {
    public static class InputHandler {
        public static Subject<KeyboardState> KeyboardStates { get; set; }
        private static IObservable<KeyboardState> distinctKeyboardStates;
        private static IObservable<IList<KeyboardState>> keyStateBuffer;
        private static List<Keys> newlyPressedKeys;
        public static BehaviorSubject<Keys> LatestHorizontalArrowKey { get; set; }

        static InputHandler() {
            KeyboardStates = new Subject<KeyboardState>();
            newlyPressedKeys = new List<Keys>();
            LatestHorizontalArrowKey = new BehaviorSubject<Keys>(Keys.Right);
            keyStateBuffer = distinctKeyboardStates.Buffer(2, 1);
        }
        public void Update() { }

        }
    }
}
