# AGENTS.md

## Android app (`android/`)

- Build: `cd android && ./gradlew :app:assembleDebug`
- Tests: JUnit unit tests via `cd android && ./gradlew :app:testDebugUnitTest`
- Toolchain: requires JDK 25 (Gradle 9.5 daemon toolchain criteria) and the Android SDK
  platform `android-37.0`. The Cloud Agent environment provisions these automatically via
  `.cursor/cloud-agent-install.sh` (installs Temurin JDK 25 and the Android SDK, and sets
  `JAVA_HOME` / `ANDROID_HOME`).

## Cursor Cloud specific instructions

- **Testing uses JUnit unit tests only. Do NOT use the Android emulator.** The emulator is
  intentionally not part of the workflow, and nested KVM-accelerated guests do not run in the
  Cloud Agent VM, so an emulator cannot boot there anyway.
- **Building is enough.** Verifying a change by building the app
  (`./gradlew :app:assembleDebug`) and running the JUnit unit tests
  (`./gradlew :app:testDebugUnitTest`) is sufficient. Do not attempt on-device / emulator runs.
