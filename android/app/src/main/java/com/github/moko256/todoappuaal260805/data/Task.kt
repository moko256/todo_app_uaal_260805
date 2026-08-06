package com.github.moko256.todoappuaal260805.data

/**
 * External model for a todo task, exposed by [TaskRepository] to the UI layer.
 */
data class Task(
    val id: Int,
    val title: String,
    val description: String,
)
