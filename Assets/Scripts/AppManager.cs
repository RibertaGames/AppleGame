using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    /// <summary>
    /// �A�v���J�n��
    /// </summary>
    public void Start()
    {
        _InitializeSetting();
    }

    private void _InitializeSetting()
    {
        // �t���[�����[�g���Œ�
        Time.fixedDeltaTime = 1f / GameDefine.FRAME_COUNT;
        Application.targetFrameRate = GameDefine.FRAME_COUNT;


    }
}
