package com.github.moko256.todoappuaal260805

import com.unity3d.player.UnityPlayerGameActivity

class MainUnityActivity : UnityPlayerGameActivity() {
    override fun onUnityPlayerUnloaded() {
        setResult(RESULT_OK)
        finish()
    }
}
