package com.github.moko256.todoappuaal260805.navigation

import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.lifecycle.viewmodel.navigation3.rememberViewModelStoreNavEntryDecorator
import androidx.navigation3.runtime.NavBackStack
import androidx.navigation3.runtime.entryProvider
import androidx.navigation3.runtime.rememberSaveableStateHolderNavEntryDecorator
import androidx.navigation3.ui.NavDisplay
import androidx.savedstate.compose.rememberSerializable
import com.github.moko256.todoappuaal260805.ui.detail.TodoDetailScreen
import com.github.moko256.todoappuaal260805.ui.home.HomeScreen
import kotlinx.serialization.serializer

@Composable
fun rememberAppNavBackStack(
    vararg startRoutes: AppRoute = arrayOf(AppRoute.Home),
): NavBackStack<AppRoute> {
    return rememberSerializable(serializer = serializer()) {
        NavBackStack(*startRoutes)
    }
}

@Composable
fun TodoAppNav(
    modifier: Modifier = Modifier,
    backStack: NavBackStack<AppRoute> = rememberAppNavBackStack(),
) {
    NavDisplay(
        backStack = backStack,
        modifier = modifier,
        onBack = { backStack.removeLastOrNull() },
        entryDecorators = listOf(
            rememberSaveableStateHolderNavEntryDecorator(),
            rememberViewModelStoreNavEntryDecorator(),
        ),
        entryProvider = entryProvider {
            entry<AppRoute.Home> {
                HomeScreen(
                    onTodoClick = { todoId ->
                        backStack.add(AppRoute.TodoDetail(todoId))
                    },
                )
            }
            entry<AppRoute.TodoDetail> { route ->
                TodoDetailScreen(
                    todoId = route.todoId,
                    onBack = { backStack.removeLastOrNull() },
                )
            }
        },
    )
}
