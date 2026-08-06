package com.github.moko256.todoappuaal260805.navigation

import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.lifecycle.viewmodel.navigation3.rememberViewModelStoreNavEntryDecorator
import androidx.navigation3.runtime.NavBackStack
import androidx.navigation3.runtime.NavKey
import androidx.navigation3.runtime.entryProvider
import androidx.navigation3.runtime.rememberNavBackStack
import androidx.navigation3.runtime.rememberSaveableStateHolderNavEntryDecorator
import androidx.navigation3.ui.NavDisplay
import androidx.savedstate.serialization.SavedStateConfiguration
import com.github.moko256.todoappuaal260805.ui.detail.TodoDetailScreen
import com.github.moko256.todoappuaal260805.ui.home.HomeScreen
import com.github.moko256.todoappuaal260805.ui.newscene.NewScene
import kotlinx.serialization.modules.SerializersModule
import kotlinx.serialization.modules.polymorphic

private val navSavedStateConfiguration = SavedStateConfiguration {
    serializersModule = SerializersModule {
        polymorphic(NavKey::class) {
            subclass(AppRoute.Home::class, AppRoute.Home.serializer())
            subclass(AppRoute.TodoDetail::class, AppRoute.TodoDetail.serializer())
            subclass(AppRoute.New::class, AppRoute.New.serializer())
        }
    }
}

@Composable
fun rememberAppNavBackStack(
    vararg startRoutes: AppRoute = arrayOf(AppRoute.Home),
): NavBackStack<NavKey> {
    return rememberNavBackStack(navSavedStateConfiguration, *startRoutes)
}

@Composable
fun TodoAppNav(
    modifier: Modifier = Modifier,
    backStack: NavBackStack<NavKey> = rememberAppNavBackStack(),
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
                    onAddClick = {
                        backStack.add(AppRoute.New)
                    },
                )
            }
            entry<AppRoute.TodoDetail> { route ->
                TodoDetailScreen(
                    todoId = route.todoId,
                    onBack = { backStack.removeLastOrNull() },
                )
            }
            entry<AppRoute.New> {
                NewScene(
                    onClose = { backStack.removeLastOrNull() },
                )
            }
        },
    )
}
