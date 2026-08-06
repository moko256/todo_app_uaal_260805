package com.github.moko256.todoappuaal260805.ui.home

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.github.moko256.todoappuaal260805.data.Task
import com.github.moko256.todoappuaal260805.data.TaskRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class HomeViewModel @Inject constructor(
    private val taskRepository: TaskRepository,
) : ViewModel() {
    val tasks: StateFlow<List<Task>> = taskRepository
        .observeTasks()
        .stateIn(
            scope = viewModelScope,
            started = SharingStarted.WhileSubscribed(5_000),
            initialValue = emptyList(),
        )

    private var pendingDeleteTaskId: Int? = null

    /** Remembers which task to delete after MainUnityActivity finishes. */
    fun prepareDelete(taskId: Int) {
        pendingDeleteTaskId = taskId
    }

    /** Deletes the pending task once MainUnityActivity has finished. */
    fun onUnityActivityFinished() {
        val taskId = pendingDeleteTaskId ?: return
        pendingDeleteTaskId = null
        viewModelScope.launch {
            taskRepository.deleteTask(taskId)
        }
    }
}
