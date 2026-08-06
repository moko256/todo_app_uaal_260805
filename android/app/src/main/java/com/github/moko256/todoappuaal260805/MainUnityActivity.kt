package com.github.moko256.todoappuaal260805

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.enableEdgeToEdge

/**
 * Placeholder host for the Unity paper-toss experience.
 * Finishing this activity (e.g. via Back) returns to the caller via the activity result API.
 */
class MainUnityActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
    }
}
