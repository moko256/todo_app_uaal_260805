package com.github.moko256.todoappuaal260805.data.local

import com.github.moko256.todoappuaal260805.data.Task
import org.junit.Assert.assertEquals
import org.junit.Test

class TaskEntityMapperTest {
    @Test
    fun asExternalModel_mapsAllFields() {
        val entity = TaskEntity(id = 3, title = "Title", description = "Description")

        assertEquals(
            Task(id = 3, title = "Title", description = "Description"),
            entity.asExternalModel(),
        )
    }

    @Test
    fun asEntity_mapsAllFields() {
        val task = Task(id = 5, title = "Title", description = "Description")

        assertEquals(
            TaskEntity(id = 5, title = "Title", description = "Description"),
            task.asEntity(),
        )
    }
}
