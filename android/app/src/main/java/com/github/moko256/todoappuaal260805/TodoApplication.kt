package com.github.moko256.todoappuaal260805

import android.app.Application
import com.github.moko256.todoappuaal260805.data.AppContainer
import com.github.moko256.todoappuaal260805.data.DefaultAppContainer

class TodoApplication : Application() {
    lateinit var container: AppContainer
        private set

    override fun onCreate() {
        super.onCreate()
        container = DefaultAppContainer(this)
    }
}
