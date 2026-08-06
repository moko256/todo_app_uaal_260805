package com.github.moko256.todoappuaal260805.ui.detail

import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.viewModelScope
import com.github.moko256.todoappuaal260805.data.Task
import com.github.moko256.todoappuaal260805.data.TaskRepository
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.stateIn

class TodoDetailViewModel(
    taskId: Int,
    taskRepository: TaskRepository,
) : ViewModel() {
    val task: StateFlow<Task?> = taskRepository
        .observeTask(taskId)
        .stateIn(
            scope = viewModelScope,
            started = SharingStarted.WhileSubscribed(5_000),
            initialValue = null,
        )

    companion object {
        fun factory(
            taskId: Int,
            taskRepository: TaskRepository,
        ): ViewModelProvider.Factory =
            object : ViewModelProvider.Factory {
                @Suppress("UNCHECKED_CAST")
                override fun <T : ViewModel> create(modelClass: Class<T>): T {
                    if (modelClass.isAssignableFrom(TodoDetailViewModel::class.java)) {
                        return TodoDetailViewModel(taskId, taskRepository) as T
                    }
                    throw IllegalArgumentException("Unknown ViewModel class: ${modelClass.name}")
                }
            }
    }
}
