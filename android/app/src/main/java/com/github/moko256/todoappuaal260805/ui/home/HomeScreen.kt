package com.github.moko256.todoappuaal260805.ui.home

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.HorizontalDivider
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.material3.TopAppBar
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import com.github.moko256.todoappuaal260805.ui.theme.Todo_app_uaal_260805Theme

private val sampleTodos = listOf(
    "Buy milk" to "todo-1",
    "Write report" to "todo-2",
    "Walk the dog" to "todo-3",
)

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun HomeScreen(
    onTodoClick: (todoId: String) -> Unit,
    modifier: Modifier = Modifier,
) {
    Scaffold(
        modifier = modifier.fillMaxSize(),
        topBar = {
            TopAppBar(title = { Text("Todos") })
        },
    ) { innerPadding ->
        LazyColumn(
            modifier = Modifier
                .fillMaxSize()
                .padding(innerPadding),
            verticalArrangement = Arrangement.Top,
        ) {
            items(sampleTodos, key = { it.second }) { (title, id) ->
                Column(
                    modifier = Modifier
                        .fillMaxWidth()
                        .clickable { onTodoClick(id) }
                        .padding(horizontal = 16.dp, vertical = 12.dp),
                ) {
                    Text(
                        text = title,
                        style = MaterialTheme.typography.titleMedium,
                    )
                    Text(
                        text = id,
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurfaceVariant,
                    )
                }
                HorizontalDivider()
            }
        }
    }
}

@Preview(showBackground = true)
@Composable
private fun HomeScreenPreview() {
    Todo_app_uaal_260805Theme {
        HomeScreen(onTodoClick = {})
    }
}
