using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public static class PlayerModel
{
    #region - Player - 


    public enum PlayerStance
    {
        Standing,
        Crouching,
        Prone
    }





    [Serializable]
    public class PlayerSettingsModel
    {
        [Header("화면 설정")]
        public float ViewXSensitivity;
        public float ViewYSensitivity;




        [Header("이동 설정")]
        public float WalkingFowardSpeed;
        public float WalkingBackwardSpeed;
        public float WalkingStrafeSpeed;

        [Header("점프 설정")]
        public float JumpingHeight;
        public float JumpingFalloff;
        
    }

    [Serializable]
    public class CharacterStance
    {
        public float CameraHeight;
        public CapsuleCollider StanceCollider;
    }

    #endregion
}
