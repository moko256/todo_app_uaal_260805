package com.github.moko256.todoappuaal260805.navigation

import kotlinx.serialization.json.Json
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import org.junit.Test

class AppRouteTest {
    private val json = Json { encodeDefaults = true }

    @Test
    fun home_roundTripsThroughSerialization() {
        val encoded = json.encodeToString(AppRoute.serializer(), AppRoute.Home)
        val decoded = json.decodeFromString(AppRoute.serializer(), encoded)

        assertEquals(AppRoute.Home, decoded)
    }

    @Test
    fun todoDetail_roundTripsThroughSerialization() {
        val route = AppRoute.TodoDetail(todoId = 42)
        val encoded = json.encodeToString(AppRoute.serializer(), route)
        val decoded = json.decodeFromString(AppRoute.serializer(), encoded)

        assertEquals(route, decoded)
        assertTrue(decoded is AppRoute.TodoDetail)
        assertEquals(42, (decoded as AppRoute.TodoDetail).todoId)
    }

    @Test
    fun new_roundTripsThroughSerialization() {
        val encoded = json.encodeToString(AppRoute.serializer(), AppRoute.New)
        val decoded = json.decodeFromString(AppRoute.serializer(), encoded)

        assertEquals(AppRoute.New, decoded)
    }
}
