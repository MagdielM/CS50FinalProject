using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Game1 {
    public static class InputHandler {
        public static Subject<KeyboardState> KeyboardStatesStream { get; set; }
        public static IObservable<KeyboardState> DistinctKeyboardStatesStream { get; }
        private static readonly Subject<Keys[]> pressedKeys;
        private static readonly IObservable<Keys[]> distinctPressedKeys;
        private static readonly IObservable<IList<Keys[]>> distinctPressedKeyArrayBuffer;
        private static readonly Random randomKeySelector;

        private static Keys latestHorizontalArrowKey;
        private static Keys latestVerticalArrowKey;
        private static List<Keys[]> horizontalKeyArrayBuffer;
        private static List<Keys[]> verticalKeyArrayBuffer;
        private static (Keys key1, Keys key2) horizontalCounterKeys = (key1: Keys.Left, key2: Keys.Right);
        private static (Keys key1, Keys key2) verticalCounterKeys = (key1: Keys.Up, key2: Keys.Down);

        private static readonly IObserver<IList<Keys[]>> horizontalInputFilterObserver = Observer
            .Create(SetHorizontalKeyArrayBuffer(horizontalCounterKeys));
        private static readonly IObserver<IList<Keys[]>> verticalInputFilterObserver = Observer
            .Create(SetVerticalKeyArrayBuffer(verticalCounterKeys));

        public static BehaviorSubject<Keys[]> InputOutKeys { get; }
        public static BehaviorSubject<Keys> LatestHorizontalArrowKey { get; }
        public static BehaviorSubject<Keys> LatestVerticalArrowKey { get; }

        static InputHandler() {
            randomKeySelector = new Random();
            KeyboardStatesStream = new Subject<KeyboardState>();
            pressedKeys = new Subject<Keys[]>();
            DistinctKeyboardStatesStream = KeyboardStatesStream.DistinctUntilChanged();
            distinctPressedKeys = pressedKeys.DistinctUntilChanged();
            InputOutKeys = new BehaviorSubject<Keys[]>(new Keys[1]);
            horizontalKeyArrayBuffer = new List<Keys[]>();
            verticalKeyArrayBuffer = new List<Keys[]>();
            LatestHorizontalArrowKey = new BehaviorSubject<Keys>(Keys.None);
            LatestVerticalArrowKey = new BehaviorSubject<Keys>(Keys.None);
            KeyboardStatesStream.Subscribe(onNext: state => pressedKeys.OnNext(state.GetPressedKeys()));
            pressedKeys.Subscribe(onNext: FilterKeys);
            distinctPressedKeyArrayBuffer = distinctPressedKeys.Buffer(2, 1);
            distinctPressedKeyArrayBuffer.Subscribe(horizontalInputFilterObserver);
            distinctPressedKeyArrayBuffer.Subscribe(verticalInputFilterObserver);
            distinctPressedKeyArrayBuffer.Subscribe(HandleConflictingInput);
        }

        private static Action<IList<Keys[]>> HandleConflictingInput => _ => {
            SetLatestKey(horizontalKeyArrayBuffer, ref latestHorizontalArrowKey);
            SetLatestKey(verticalKeyArrayBuffer, ref latestVerticalArrowKey);
        };
        private static Action<IList<Keys[]>> SetHorizontalKeyArrayBuffer(
            (Keys key1, Keys key2) counterKeys) {
            return array => {
                List<Keys[]> newKeyArray = BuildCounterKeyArrayBuffer(counterKeys, array);
                horizontalKeyArrayBuffer = newKeyArray;
            };
        }

        private static Action<IList<Keys[]>> SetVerticalKeyArrayBuffer(
            (Keys key1, Keys key2) counterKeys) {
            return array => {
                List<Keys[]> newKeyArray = BuildCounterKeyArrayBuffer(counterKeys, array);
                verticalKeyArrayBuffer = newKeyArray;
            };
        }

        private static List<Keys[]> BuildCounterKeyArrayBuffer((Keys key1, Keys key2) counterKeys, IList<Keys[]> array) {
            var newKeyArray = new List<Keys[]>();
            foreach (Keys[] keyArray in array) {
                var outKeys = (from Keys key in keyArray
                               where key == counterKeys.key1
                               || key == counterKeys.key2
                               select key).ToArray();
                newKeyArray.Add(outKeys);
            }

            return newKeyArray;
        }

        private static void SetLatestKey(List<Keys[]> keyArrayBuffer, ref Keys key) {
            if (keyArrayBuffer?.Count == 2) {
                if (keyArrayBuffer[1].Length == 0) {
                    key = Keys.None;
                }
                else if (keyArrayBuffer[0].SequenceEqual(keyArrayBuffer[1]) && keyArrayBuffer[0].Length == 1) {
                    key = keyArrayBuffer[1].Single();
                }
                else if (keyArrayBuffer[1].Length > keyArrayBuffer[0].Length) {
                    try {
                        key = keyArrayBuffer[1].Except(keyArrayBuffer[0].DefaultIfEmpty()).Single();
                    }
                    catch (InvalidOperationException) {
                        key = keyArrayBuffer[1][randomKeySelector.Next(0, keyArrayBuffer[1].Length - 1)];
                    }
                }
                else if (keyArrayBuffer[0].Length > keyArrayBuffer[1].Length) {
                    key = keyArrayBuffer[1].Single();
                }
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
