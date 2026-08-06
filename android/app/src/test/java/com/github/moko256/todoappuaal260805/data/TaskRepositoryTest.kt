package com.github.moko256.todoappuaal260805.data

import com.github.moko256.todoappuaal260805.data.local.TaskDao
import com.github.moko256.todoappuaal260805.data.local.TaskEntity
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.flow.flowOf
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNull
import org.junit.Test

class TaskRepositoryTest {
    @Test
    fun observeTasks_mapsEntitiesToTasks() = runTest {
        val dao = FakeTaskDao(
            tasks = listOf(
                TaskEntity(id = 1, title = "Buy milk", description = "From store"),
                TaskEntity(id = 2, title = "Write report", description = "Q1 summary"),
            ),
        )
        val repository = TaskRepository(dao)

        val tasks = repository.observeTasks().first()

        assertEquals(
            listOf(
                Task(id = 1, title = "Buy milk", description = "From store"),
                Task(id = 2, title = "Write report", description = "Q1 summary"),
            ),
            tasks,
        )
    }

    @Test
    fun observeTask_mapsEntityToTask() = runTest {
        val dao = FakeTaskDao(
            tasks = listOf(
                TaskEntity(id = 7, title = "Walk the dog", description = "Park loop"),
            ),
        )
        val repository = TaskRepository(dao)

        val task = repository.observeTask(7).first()

        assertEquals(
            Task(id = 7, title = "Walk the dog", description = "Park loop"),
            task,
        )
    }

    @Test
    fun getTask_returnsNullWhenMissing() = runTest {
        val repository = TaskRepository(FakeTaskDao())

        assertNull(repository.getTask(99))
    }

    @Test
    fun createTask_insertsEntityWithoutId() = runTest {
        val dao = FakeTaskDao()
        val repository = TaskRepository(dao)

        val id = repository.createTask(title = "New", description = "Desc")

        assertEquals(1, id)
        assertEquals(
            listOf(TaskEntity(id = 1, title = "New", description = "Desc")),
            dao.tasks,
        )
    }

    private class FakeTaskDao(
        initial: List<TaskEntity> = emptyList(),
    ) : TaskDao {
        var tasks: List<TaskEntity> = initial
            private set

        override fun observeAll(): Flow<List<TaskEntity>> = flowOf(tasks)

        override fun observeById(id: Int): Flow<TaskEntity?> =
            flowOf(tasks.find { it.id == id })

        override suspend fun getById(id: Int): TaskEntity? =
            tasks.find { it.id == id }

        override suspend fun insert(task: TaskEntity): Long {
            val id = if (task.id == 0) {
                (tasks.maxOfOrNull { it.id } ?: 0) + 1
            } else {
                task.id
            }
            tasks = tasks.filterNot { it.id == id } + task.copy(id = id)
            return id.toLong()
        }

        override suspend fun insertAll(tasks: List<TaskEntity>) {
            tasks.forEach { insert(it) }
        }

        override suspend fun update(task: TaskEntity) {
            this.tasks = this.tasks.map { if (it.id == task.id) task else it }
        }

        override suspend fun delete(task: TaskEntity) {
            tasks = tasks.filterNot { it.id == task.id }
        }

        override suspend fun deleteById(id: Int) {
            tasks = tasks.filterNot { it.id == id }
        }
    }
}
