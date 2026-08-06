package com.github.moko256.todoappuaal260805.data.local

import android.content.Context
import androidx.room.Database
import androidx.room.Room
import androidx.room.RoomDatabase
import androidx.sqlite.db.SupportSQLiteDatabase
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch

@Database(
    entities = [TaskEntity::class],
    version = 1,
    exportSchema = false,
)
abstract class AppDatabase : RoomDatabase() {
    abstract fun taskDao(): TaskDao

    companion object {
        @Volatile
        private var instance: AppDatabase? = null

        fun getInstance(context: Context): AppDatabase {
            return instance ?: synchronized(this) {
                instance ?: buildDatabase(context.applicationContext).also { instance = it }
            }
        }

        private fun buildDatabase(context: Context): AppDatabase {
            return Room.databaseBuilder(
                context,
                AppDatabase::class.java,
                "todo_app.db",
            )
                .addCallback(SeedCallback())
                .build()
        }

        private class SeedCallback : Callback() {
            override fun onCreate(db: SupportSQLiteDatabase) {
                super.onCreate(db)
                instance?.let { database ->
                    CoroutineScope(Dispatchers.IO).launch {
                        database.taskDao().insertAll(
                            listOf(
                                TaskEntity(
                                    title = "Buy milk",
                                    description = "2% milk from the corner store",
                                ),
                                TaskEntity(
                                    title = "Write report",
                                    description = "Quarterly summary for the team",
                                ),
                                TaskEntity(
                                    title = "Walk the dog",
                                    description = "Evening walk around the park",
                                ),
                            ),
                        )
                    }
                }
            }
        }
    }
}
