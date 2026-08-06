package com.github.moko256.todoappuaal260805.data.local

import androidx.room.Entity
import androidx.room.PrimaryKey
import com.github.moko256.todoappuaal260805.data.Task

@Entity(tableName = "tasks")
data class TaskEntity(
    @PrimaryKey(autoGenerate = true)
    val id: Int = 0,
    val title: String,
    val description: String,
)

fun TaskEntity.asExternalModel(): Task = Task(
    id = id,
    title = title,
    description = description,
)

fun Task.asEntity(): TaskEntity = TaskEntity(
    id = id,
    title = title,
    description = description,
)
