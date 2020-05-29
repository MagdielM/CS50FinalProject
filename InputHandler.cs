using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Game1 {
    public static class InputHandler {
        public static Subject<KeyboardState> KeyboardStatesStream { get; set; }
        public static IObservable<KeyboardState> DistinctKeyboardStatesStream { get; private set; }
        private static readonly Subject<Keys[]> pressedKeys;
        private static IObservable<IList<Keys[]>> pressedKeyArrayBuffer;

        public static BehaviorSubject<Keys[]> InputOutKeys { get; private set; }
        public static BehaviorSubject<Keys> LatestHorizontalArrowKey { get; private set; }
        public static BehaviorSubject<Keys> LatestVerticalArrowKey { get; private set; }

        private static Keys latestHorizontalArrowKey;
        private static Keys latestVerticalArrowKey;
        private static (Keys key1, Keys key2) horizontalCounterKeys = (key1: Keys.Left, key2: Keys.Right);
        private static (Keys key1, Keys key2) verticalCounterKeys = (key1: Keys.Up, key2: Keys.Down);

        static InputHandler() {
            KeyboardStatesStream = new Subject<KeyboardState>();
            DistinctKeyboardStatesStream = KeyboardStatesStream.DistinctUntilChanged();
            InputOutKeys = new BehaviorSubject<Keys[]>(new Keys[1]);
            LatestHorizontalArrowKey = new BehaviorSubject<Keys>(Keys.None);
            LatestVerticalArrowKey = new BehaviorSubject<Keys>(Keys.None);
            KeyboardStatesStream.Subscribe(onNext: state => {
                pressedKeys.OnNext(state.GetPressedKeys());
            });
            pressedKeys.Subscribe(onNext: keys => FilterKeys(keys));

            DistinctKeyboardStatesStream.Subscribe(onNext: HandleConflictingInput);
        }

        private static Action<KeyboardState> HandleConflictingInput => state => {
            pressedKeyArrayBuffer = pressedKeys.Buffer(2, 1);
            var horizontalKeyArrayBuffer = new List<Keys[]>();
            var verticalKeyArrayBuffer = new List<Keys[]>();
            pressedKeyArrayBuffer.ForEachAsync(onNext: BuildCounterKeyArrayBuffer(horizontalKeyArrayBuffer, horizontalCounterKeys));
            pressedKeyArrayBuffer.ForEachAsync(onNext: BuildCounterKeyArrayBuffer(verticalKeyArrayBuffer, verticalCounterKeys));
            SetLatestKey(horizontalKeyArrayBuffer, ref latestHorizontalArrowKey);
            SetLatestKey(verticalKeyArrayBuffer, ref latestVerticalArrowKey);

        };
        private static Action<IList<Keys[]>> BuildCounterKeyArrayBuffer(
            List<Keys[]> counterKeyArrayBuffer, (Keys key1, Keys key2) counterKeys) {
                return array => {
                    counterKeyArrayBuffer.Add(
                        (from Keys key in array
                         where key == counterKeys.key1 || key == counterKeys.key2
                         select key).ToArray());
                };
            }

        private static void SetLatestKey(List<Keys[]> keyArrayBuffer, ref Keys key) {
            if (keyArrayBuffer[0].Length > keyArrayBuffer[1].Length) {
                key = keyArrayBuffer[1].Except(keyArrayBuffer[0]).Single();
            }
            else if (keyArrayBuffer[1].Length > keyArrayBuffer[0].Length) {
                key = keyArrayBuffer[0].Except(keyArrayBuffer[1]).Single();
            }
            else if (keyArrayBuffer[1].Length == 0) {
                key = Keys.None;
            }
        }

        public static void Update() {
            KeyboardStatesStream.OnNext(Keyboard.GetState());
            LatestHorizontalArrowKey.OnNext(latestHorizontalArrowKey);
            LatestVerticalArrowKey.OnNext(latestVerticalArrowKey);
        }

        private static void FilterKeys(Keys[] keys) {
            var filteredKeys = (from key in keys
                                where key != Keys.Up
                                && key != Keys.Down
                                && key != Keys.Left
                                && key != Keys.Right
                                select key).ToArray();
            InputOutKeys.OnNext(filteredKeys);
        }
    }
}
