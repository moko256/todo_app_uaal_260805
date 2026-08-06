package com.github.moko256.todoappuaal260805.ui.newscene

import com.github.moko256.todoappuaal260805.data.Task
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
import org.junit.Assert.assertTrue
import org.junit.Before
import org.junit.Test

@OptIn(ExperimentalCoroutinesApi::class)
class NewSceneViewModelTest {
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
    fun add_createsTaskAndInvokesOnComplete() = runTest {
        val dao = FakeTaskDao()
        val viewModel = NewSceneViewModel(TaskRepository(dao))
        var completed = false

        viewModel.add(title = "Buy milk", description = "From store") {
            completed = true
        }
        advanceUntilIdle()

        assertTrue(completed)
        assertEquals(
            listOf(TaskEntity(id = 1, title = "Buy milk", description = "From store")),
            dao.tasks,
        )
    }

    private class FakeTaskDao : TaskDao {
        var tasks: List<TaskEntity> = emptyList()
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
