package com.github.moko256.todoappuaal260805.data

import android.content.Context
import com.github.moko256.todoappuaal260805.data.local.AppDatabase

interface AppContainer {
    val taskRepository: TaskRepository
}

class DefaultAppContainer(
    context: Context,
) : AppContainer {
    private val database = AppDatabase.getInstance(context)

    override val taskRepository: TaskRepository by lazy {
        TaskRepository(database.taskDao())
    }
}
