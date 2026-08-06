package com.github.moko256.todoappuaal260805.data

import com.github.moko256.todoappuaal260805.data.local.TaskDao
import com.github.moko256.todoappuaal260805.data.local.TaskEntity
import com.github.moko256.todoappuaal260805.data.local.asEntity
import com.github.moko256.todoappuaal260805.data.local.asExternalModel
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map
import javax.inject.Inject
import javax.inject.Singleton

/**
 * Data-layer single source of truth for tasks.
 * Converts Room [TaskDao] entities into external [Task] models for the UI layer.
 */
@Singleton
class TaskRepository @Inject constructor(
    private val taskDao: TaskDao,
) {
    fun observeTasks(): Flow<List<Task>> =
        taskDao.observeAll().map { entities ->
            entities.map(TaskEntity::asExternalModel)
        }

    fun observeTask(id: Int): Flow<Task?> =
        taskDao.observeById(id).map { entity ->
            entity?.asExternalModel()
        }

    suspend fun getTask(id: Int): Task? =
        taskDao.getById(id)?.asExternalModel()

    suspend fun createTask(title: String, description: String): Int {
        val rowId = taskDao.insert(
            TaskEntity(
                title = title,
                description = description,
            ),
        )
        return rowId.toInt()
    }

    suspend fun updateTask(task: Task) {
        taskDao.update(task.asEntity())
    }

    suspend fun deleteTask(id: Int) {
        taskDao.deleteById(id)
    }
}
