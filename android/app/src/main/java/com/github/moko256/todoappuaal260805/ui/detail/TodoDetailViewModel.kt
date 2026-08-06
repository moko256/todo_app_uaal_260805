package com.github.moko256.todoappuaal260805.ui.detail

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.github.moko256.todoappuaal260805.data.Task
import com.github.moko256.todoappuaal260805.data.TaskRepository
import dagger.assisted.Assisted
import dagger.assisted.AssistedFactory
import dagger.assisted.AssistedInject
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.stateIn

@HiltViewModel(assistedFactory = TodoDetailViewModel.Factory::class)
class TodoDetailViewModel @AssistedInject constructor(
    @Assisted taskId: Int,
    taskRepository: TaskRepository,
) : ViewModel() {
    val task: StateFlow<Task?> = taskRepository
        .observeTask(taskId)
        .stateIn(
            scope = viewModelScope,
            started = SharingStarted.WhileSubscribed(5_000),
            initialValue = null,
        )

    @AssistedFactory
    interface Factory {
        fun create(taskId: Int): TodoDetailViewModel
    }
}
