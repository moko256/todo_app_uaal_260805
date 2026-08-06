package com.github.moko256.todoappuaal260805.ui.newscene

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.github.moko256.todoappuaal260805.data.TaskRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class NewSceneViewModel @Inject constructor(
    private val taskRepository: TaskRepository,
) : ViewModel() {
    fun add(title: String, description: String, onComplete: () -> Unit) {
        viewModelScope.launch {
            taskRepository.createTask(title = title, description = description)
            onComplete()
        }
    }
}
