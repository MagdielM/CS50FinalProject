using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Game1 {
    public static class InputHandler {
        public static Subject<KeyboardState> KeyboardStatesStream { get; set; }
        private static IObservable<KeyboardState> distinctKeyboardStatesStream;
        private static Subject<Keys[]> pressedKeys;
        private static IObservable<IList<Keys[]>> pressedKeyArrayBuffer;
        private static Keys latestHorizontalArrowKey;
        public static BehaviorSubject<Keys> LatestHorizontalArrowKey { get; set; }

        static InputHandler() {
            KeyboardStatesStream = new Subject<KeyboardState>();
            distinctKeyboardStatesStream = KeyboardStatesStream.DistinctUntilChanged();
            LatestHorizontalArrowKey = new BehaviorSubject<Keys>(Keys.Right);
            distinctKeyboardStatesStream.Subscribe(state => {
                pressedKeys.OnNext(state.GetPressedKeys());
                pressedKeyArrayBuffer = pressedKeys.Buffer(2, 1);
                List<Keys[]> pressedHorizontalKeyArrayBuffer = new List<Keys[]>();
                pressedKeyArrayBuffer.ForEachAsync(array => {
                    pressedHorizontalKeyArrayBuffer.Add((from Keys key in array
                                                         where key == Keys.Left || key == Keys.Right
                                                         select key).ToArray());
                });
                if (pressedHorizontalKeyArrayBuffer[0].Length > pressedHorizontalKeyArrayBuffer[1].Length) {
                    latestHorizontalArrowKey = pressedHorizontalKeyArrayBuffer[1].Except(pressedHorizontalKeyArrayBuffer[0]).Single();
                }
                else if (pressedHorizontalKeyArrayBuffer[1].Length > pressedHorizontalKeyArrayBuffer[0].Length) {
                    latestHorizontalArrowKey = pressedHorizontalKeyArrayBuffer[0].Except(pressedHorizontalKeyArrayBuffer[1]).Single();
                }
                else if (pressedHorizontalKeyArrayBuffer[1].Length == 0) {
                    latestHorizontalArrowKey = Keys.None;
                }
            });
        }

        public static void Update() {
            KeyboardStatesStream.OnNext(Keyboard.GetState());
            LatestHorizontalArrowKey.OnNext(latestHorizontalArrowKey);
        }
    }
}
