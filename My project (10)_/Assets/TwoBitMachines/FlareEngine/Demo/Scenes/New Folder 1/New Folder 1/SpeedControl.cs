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
            // ���x�����ɏグ���Ă���ꍇ�A���̑��x�ɖ߂�
            Time.timeScale = 1f;
        }
        else
        {
            // ���x��1.5�{�ɂ���
            Time.timeScale = 1.5f;
        }

        isSpeedBoosted = !isSpeedBoosted;
    }
}
