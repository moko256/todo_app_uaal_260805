# AGENTS.md

## Android app (`android/`)

- Build: `cd android && ./gradlew :app:assembleDebug`
- Test: `cd android && ./gradlew :app:testDebugUnitTest` (JUnit unit tests)

## Cursor Cloud specific instructions

- Test with JUnit unit tests only; do not use the Android emulator.
- Building and running the JUnit unit tests is sufficient verification.
