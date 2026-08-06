package com.github.moko256.todoappuaal260805

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.ui.Modifier
import com.github.moko256.todoappuaal260805.navigation.TodoAppNav
import com.github.moko256.todoappuaal260805.ui.theme.Todo_app_uaal_260805Theme

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            Todo_app_uaal_260805Theme {
                TodoAppNav(modifier = Modifier.fillMaxSize())
            }
        }
    }
}
