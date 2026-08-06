package com.github.moko256.todoappuaal260805.di

import android.content.Context
import androidx.room.Room
import androidx.room.RoomDatabase
import androidx.sqlite.db.SupportSQLiteDatabase
import com.github.moko256.todoappuaal260805.data.local.AppDatabase
import com.github.moko256.todoappuaal260805.data.local.TaskDao
import com.github.moko256.todoappuaal260805.data.local.TaskEntity
import dagger.Module
import dagger.Provides
import dagger.hilt.InstallIn
import dagger.hilt.android.qualifiers.ApplicationContext
import dagger.hilt.components.SingletonComponent
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import javax.inject.Singleton

@Module
@InstallIn(SingletonComponent::class)
object DatabaseModule {
    @Provides
    @Singleton
    fun provideAppDatabase(
        @ApplicationContext context: Context,
    ): AppDatabase {
        lateinit var database: AppDatabase
        database = Room.databaseBuilder(
            context,
            AppDatabase::class.java,
            "todo_app.db",
        )
            .addCallback(
                object : RoomDatabase.Callback() {
                    override fun onCreate(db: SupportSQLiteDatabase) {
                        super.onCreate(db)
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
                },
            )
            .build()
        return database
    }

    @Provides
    fun provideTaskDao(database: AppDatabase): TaskDao = database.taskDao()
}
