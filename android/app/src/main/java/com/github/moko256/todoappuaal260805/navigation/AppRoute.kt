package com.github.moko256.todoappuaal260805.navigation

import androidx.navigation3.runtime.NavKey
import kotlinx.serialization.Serializable

@Serializable
sealed interface AppRoute : NavKey {
    @Serializable
    data object Home : AppRoute

    @Serializable
    data class TodoDetail(val todoId: Int) : AppRoute

    @Serializable
    data object New : AppRoute
}
