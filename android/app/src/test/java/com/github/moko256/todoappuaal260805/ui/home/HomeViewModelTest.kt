package com.github.moko256.todoappuaal260805.ui.home

import com.github.moko256.todoappuaal260805.data.TaskRepository
import com.github.moko256.todoappuaal260805.data.local.TaskDao
import com.github.moko256.todoappuaal260805.data.local.TaskEntity
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.flowOf
import kotlinx.coroutines.test.StandardTestDispatcher
import kotlinx.coroutines.test.advanceUntilIdle
import kotlinx.coroutines.test.resetMain
import kotlinx.coroutines.test.runTest
import kotlinx.coroutines.test.setMain
import org.junit.After
import org.junit.Assert.assertEquals
import org.junit.Before
import org.junit.Test

@OptIn(ExperimentalCoroutinesApi::class)
class HomeViewModelTest {
    private val testDispatcher = StandardTestDispatcher()

    @Before
    fun setUp() {
        Dispatchers.setMain(testDispatcher)
    }

    @After
    fun tearDown() {
        Dispatchers.resetMain()
    }

    @Test
    fun onUnityActivityFinished_deletesPreparedTask() = runTest {
        val dao = FakeTaskDao(
            initialTasks = listOf(
                TaskEntity(id = 1, title = "Keep", description = "Stays"),
                TaskEntity(id = 2, title = "Delete me", description = "Goes"),
            ),
        )
        val viewModel = HomeViewModel(TaskRepository(dao))

        viewModel.prepareDelete(taskId = 2)
        viewModel.onUnityActivityFinished()
        advanceUntilIdle()

        assertEquals(
            listOf(TaskEntity(id = 1, title = "Keep", description = "Stays")),
            dao.tasks,
        )
    }

    @Test
    fun onUnityActivityFinished_withoutPrepare_doesNothing() = runTest {
        val dao = FakeTaskDao(
            initialTasks = listOf(
                TaskEntity(id = 1, title = "Keep", description = "Stays"),
            ),
        )
        val viewModel = HomeViewModel(TaskRepository(dao))

        viewModel.onUnityActivityFinished()
        advanceUntilIdle()

        assertEquals(
            listOf(TaskEntity(id = 1, title = "Keep", description = "Stays")),
            dao.tasks,
        )
    }

    private class FakeTaskDao(
        initialTasks: List<TaskEntity> = emptyList(),
    ) : TaskDao {
        var tasks: List<TaskEntity> = initialTasks
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
