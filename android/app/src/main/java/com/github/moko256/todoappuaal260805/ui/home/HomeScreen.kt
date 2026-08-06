package com.github.moko256.todoappuaal260805.ui.home

import android.content.Intent
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Add
import androidx.compose.material.icons.filled.Close
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.HorizontalDivider
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.ListItem
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.MediumFlexibleTopAppBar
import androidx.compose.material3.Scaffold
import androidx.compose.material3.SmallExtendedFloatingActionButton
import androidx.compose.material3.Text
import androidx.compose.material3.TopAppBarDefaults
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.input.nestedscroll.nestedScroll
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.tooling.preview.Preview
import androidx.hilt.lifecycle.viewmodel.compose.hiltViewModel
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import com.github.moko256.todoappuaal260805.MainUnityActivity
import com.github.moko256.todoappuaal260805.R
import com.github.moko256.todoappuaal260805.data.Task
import com.github.moko256.todoappuaal260805.ui.theme.Todo_app_uaal_260805Theme

@Composable
fun HomeScreen(
    onTodoClick: (todoId: Int) -> Unit,
    onAddClick: () -> Unit,
    modifier: Modifier = Modifier,
    viewModel: HomeViewModel = hiltViewModel(),
) {
    val tasks by viewModel.tasks.collectAsStateWithLifecycle()
    val context = LocalContext.current
    val unityActivityLauncher = rememberLauncherForActivityResult(
        contract = ActivityResultContracts.StartActivityForResult(),
    ) {
        viewModel.onUnityActivityFinished()
    }

    HomeScreen(
        tasks = tasks,
        onTodoClick = onTodoClick,
        onAddClick = onAddClick,
        onDeleteClick = { taskId ->
            viewModel.prepareDelete(taskId)
            unityActivityLauncher.launch(Intent(context, MainUnityActivity::class.java))
        },
        modifier = modifier,
    )
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun HomeScreen(
    tasks: List<Task>,
    onTodoClick: (todoId: Int) -> Unit,
    onAddClick: () -> Unit,
    onDeleteClick: (todoId: Int) -> Unit,
    modifier: Modifier = Modifier,
) {
    val scrollBehavior = TopAppBarDefaults.exitUntilCollapsedScrollBehavior()
    Scaffold(
        modifier = modifier
            .fillMaxSize()
            .nestedScroll(scrollBehavior.nestedScrollConnection),
        topBar = {
            MediumFlexibleTopAppBar(
                title = {
                    Text(
                        text = stringResource(R.string.home_title),
                        style = MaterialTheme.typography.headlineMediumEmphasized,
                    )
                },
                scrollBehavior = scrollBehavior,
            )
        },
        floatingActionButton = {
            SmallExtendedFloatingActionButton (
                onClick = onAddClick,
                icon = {
                    Icon(
                        imageVector = Icons.Filled.Add,
                        contentDescription = stringResource(R.string.add),
                    )
                },
                text = { Text(stringResource(R.string.add)) },
            )
        },
    ) { innerPadding ->
        LazyColumn(
            modifier = Modifier
                .fillMaxSize()
                .padding(innerPadding),
            verticalArrangement = Arrangement.Top,
        ) {
            items(tasks, key = { it.id }) { task ->
                ListItem(
                    modifier = Modifier.clickable { onTodoClick(task.id) },
                    leadingContent = null,
                    trailingContent = {
                        IconButton(onClick = { onDeleteClick(task.id) }) {
                            Icon(
                                imageVector = Icons.Filled.Close,
                                contentDescription = stringResource(R.string.delete),
                            )
                        }
                    },
                    overlineContent = null,
                    supportingContent = { Text(task.description) },
                    content = {
                        Text(
                            text = task.title,
                        )
                    },
                )
                HorizontalDivider()
            }
        }
    }
}

@Preview(showBackground = true)
@Composable
private fun HomeScreenPreview() {
    Todo_app_uaal_260805Theme {
        HomeScreen(
            tasks = listOf(
                Task(
                    id = 1,
                    title = stringResource(R.string.sample_task_buy_milk_title),
                    description = stringResource(R.string.sample_task_buy_milk_description),
                ),
                Task(
                    id = 2,
                    title = stringResource(R.string.sample_task_write_report_title),
                    description = stringResource(R.string.sample_task_write_report_description),
                ),
                Task(
                    id = 3,
                    title = stringResource(R.string.sample_task_walk_the_dog_title),
                    description = stringResource(R.string.sample_task_walk_the_dog_description),
                ),
            ),
            onTodoClick = {},
            onAddClick = {},
            onDeleteClick = {},
        )
    }
}
