namespace FallingBlocks.Logic
{
    /// <summary>
    /// キーの押しっぱなしを「最初の 1 回 → 少し待つ → 一定間隔で連続」に変換する(DAS / ARR)。
    /// 毎フレーム Poll を呼ぶと、そのフレームで何回入力したことにするかを返す。
    /// </summary>
    public sealed class KeyRepeater
    {
        private readonly float delay;
        private readonly float interval;

        private bool wasDown;
        private float heldTime;
        private float nextFireTime;

        public KeyRepeater(float delay, float interval)
        {
            this.delay = delay;
            this.interval = interval;
        }

        public int Poll(bool isDown, float deltaTime)
        {
            if (!isDown)
            {
                wasDown = false;
                return 0;
            }

            if (!wasDown)
            {
                // 押した瞬間に 1 回。次は delay 秒後から
                wasDown = true;
                heldTime = 0f;
                nextFireTime = delay;
                return 1;
            }

            heldTime += deltaTime;
            int count = 0;
            while (heldTime >= nextFireTime)
            {
                count++;
                nextFireTime += interval;
            }

            return count;
        }
    }
}
