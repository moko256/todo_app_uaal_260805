package com.github.moko256.todoappuaal260805.di

import android.content.Context
import androidx.room.Room
import androidx.room.RoomDatabase
import androidx.sqlite.db.SupportSQLiteDatabase
import com.github.moko256.todoappuaal260805.R
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
                                        title = context.getString(R.string.sample_task_buy_milk_title),
                                        description = context.getString(R.string.sample_task_buy_milk_description),
                                    ),
                                    TaskEntity(
                                        title = context.getString(R.string.sample_task_write_report_title),
                                        description = context.getString(R.string.sample_task_write_report_description),
                                    ),
                                    TaskEntity(
                                        title = context.getString(R.string.sample_task_walk_the_dog_title),
                                        description = context.getString(R.string.sample_task_walk_the_dog_description),
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
