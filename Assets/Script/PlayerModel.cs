using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerModel
{
    #region - Player - 

    public static class PlayerSettingsModel
    {
        [Header("화면 설정")]
        public float ViewXSensitivity;
        public float ViewYSensitivity;


        public bool ViewXInverted;
        public bool ViewYInverted;
    }

    #endregion
}
