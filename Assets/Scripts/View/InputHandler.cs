using FallingBlocks.Logic;
using UnityEngine.InputSystem;

namespace FallingBlocks.View
{
    /// <summary>キーボード入力を読んで、GameState の操作メソッドを呼ぶ。</summary>
    public sealed class InputHandler
    {
        private const float MoveDelay = 0.17f;
        private const float MoveInterval = 0.05f;
        private const float SoftDropInterval = 0.05f;

        private readonly KeyRepeater leftRepeater = new KeyRepeater(MoveDelay, MoveInterval);
        private readonly KeyRepeater rightRepeater = new KeyRepeater(MoveDelay, MoveInterval);
        private readonly KeyRepeater downRepeater = new KeyRepeater(SoftDropInterval, SoftDropInterval);

        /// <summary>このフレームでリスタートが要求されたか(ゲームオーバー中の R キー)。</summary>
        public bool RestartRequested { get; private set; }

        public void Update(GameState state, float deltaTime)
        {
            RestartRequested = false;

            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (state.IsGameOver)
            {
                RestartRequested = keyboard.rKey.wasPressedThisFrame;
                return;
            }

            int left = leftRepeater.Poll(keyboard.leftArrowKey.isPressed, deltaTime);
            for (int i = 0; i < left; i++)
            {
                state.TryMove(-1);
            }

            int right = rightRepeater.Poll(keyboard.rightArrowKey.isPressed, deltaTime);
            for (int i = 0; i < right; i++)
            {
                state.TryMove(1);
            }

            int down = downRepeater.Poll(keyboard.downArrowKey.isPressed, deltaTime);
            for (int i = 0; i < down; i++)
            {
                state.SoftDrop();
            }

            if (keyboard.upArrowKey.wasPressedThisFrame || keyboard.xKey.wasPressedThisFrame)
            {
                state.TryRotate(1);
            }

            if (keyboard.zKey.wasPressedThisFrame)
            {
                state.TryRotate(-1);
            }

            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                state.HardDrop();
            }
        }
    }
}
