using UnityEngine;

public class SlowDownGame : MonoBehaviour
{
    private bool isSlowingDown = false;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!isSlowingDown)
            {
                isSlowingDown = true;
                Time.timeScale = 0.2f;
            }
        }
        else
        {
            if (isSlowingDown)
            {
                isSlowingDown = false;
                Time.timeScale = 1f;
            }
        }
    }
}