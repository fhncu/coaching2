using UnityEngine;

public class SpeedControl : MonoBehaviour
{
    private bool isSpeedBoosted = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleSpeed();
        }
    }

    private void ToggleSpeed()
    {
        if (isSpeedBoosted)
        {
            // 速度が既に上げられている場合、元の速度に戻す
            Time.timeScale = 1f;
        }
        else
        {
            // 速度を1.5倍にする
            Time.timeScale = 1.5f;
        }

        isSpeedBoosted = !isSpeedBoosted;
    }
}
