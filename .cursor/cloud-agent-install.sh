#!/usr/bin/env bash
#
# Cloud Agent install script for todo_app_uaal_260805.
#
# Provisions everything needed to build and test the Android app in `android/`:
#   * Temurin JDK 25  -> required by android/gradle/gradle-daemon-jvm.properties
#                        (Gradle 9.5 daemon toolchain criteria: toolchainVersion=25)
#   * Android SDK     -> cmdline-tools, platform-tools, platform android-37.0,
#                        build-tools 37.0.0 (compileSdk/targetSdk 37) and 36.0.0
#                        (pulled in by AGP 9.3.1).
#
# The script is idempotent: existing installs are detected and skipped, so it is
# safe to run repeatedly and when building from a cached snapshot.
#
# NOTE: The Unity project in `unity/` (Unity 6000.5.6f1) is intentionally NOT set
# up here. Building it requires the Unity Editor with the Android module plus a
# Unity license, which must be supplied as a secret. See the PR description.
set -euo pipefail

JDK_URL="https://api.adoptium.net/v3/binary/latest/25/ga/linux/x64/jdk/hotspot/normal/eclipse"
CMDLINE_TOOLS_URL="https://dl.google.com/android/repository/commandlinetools-linux-15859902_latest.zip"
JDK_ROOT="/opt/jdk"
SDK_ROOT="/opt/android-sdk"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"

if command -v sudo >/dev/null 2>&1; then SUDO="sudo"; else SUDO=""; fi

log() { printf '\n>> %s\n' "$*"; }

# 1. JDK 25 --------------------------------------------------------------------
if [ ! -x "$JDK_ROOT/current/bin/java" ]; then
  log "Installing Temurin JDK 25 into $JDK_ROOT ..."
  $SUDO mkdir -p "$JDK_ROOT"
  tmp="$(mktemp -d)"
  curl -fL --retry 4 --retry-delay 4 -o "$tmp/jdk.tar.gz" "$JDK_URL"
  $SUDO tar xzf "$tmp/jdk.tar.gz" -C "$JDK_ROOT"
  jdkdir="$($SUDO sh -c "ls -d $JDK_ROOT/jdk-25* | head -1")"
  $SUDO ln -sfn "$jdkdir" "$JDK_ROOT/current"
  rm -rf "$tmp"
else
  log "JDK 25 already present at $JDK_ROOT/current"
fi
export JAVA_HOME="$JDK_ROOT/current"

# 2. Android SDK command-line tools -------------------------------------------
if [ ! -x "$SDK_ROOT/cmdline-tools/latest/bin/sdkmanager" ]; then
  log "Installing Android command-line tools into $SDK_ROOT ..."
  $SUDO mkdir -p "$SDK_ROOT/cmdline-tools"
  tmp="$(mktemp -d)"
  curl -fL --retry 4 --retry-delay 4 -o "$tmp/cmdtools.zip" "$CMDLINE_TOOLS_URL"
  unzip -q "$tmp/cmdtools.zip" -d "$tmp/extract"
  $SUDO rm -rf "$SDK_ROOT/cmdline-tools/latest"
  $SUDO mv "$tmp/extract/cmdline-tools" "$SDK_ROOT/cmdline-tools/latest"
  rm -rf "$tmp"
else
  log "Android command-line tools already present at $SDK_ROOT"
fi

# Make the SDK writable by the current user so sdkmanager/AGP can add packages.
$SUDO chown -R "$(id -u):$(id -g)" "$SDK_ROOT"

export ANDROID_HOME="$SDK_ROOT"
export ANDROID_SDK_ROOT="$SDK_ROOT"
export PATH="$JAVA_HOME/bin:$SDK_ROOT/cmdline-tools/latest/bin:$SDK_ROOT/platform-tools:$PATH"

# 3. SDK packages + licenses (idempotent) -------------------------------------
log "Accepting licenses and installing SDK packages ..."
yes | sdkmanager --licenses >/dev/null 2>&1 || true
sdkmanager --install \
  "platform-tools" \
  "platforms;android-37.0" \
  "build-tools;37.0.0" \
  "build-tools;36.0.0" >/dev/null

# 4. Persistent environment for future shells ---------------------------------
PROFILE_SNIPPET="$(cat <<'EOF'
export JAVA_HOME=/opt/jdk/current
export ANDROID_HOME=/opt/android-sdk
export ANDROID_SDK_ROOT=/opt/android-sdk
export PATH="$JAVA_HOME/bin:$ANDROID_HOME/cmdline-tools/latest/bin:$ANDROID_HOME/platform-tools:$PATH"
EOF
)"
if [ -n "$SUDO" ] || [ -w /etc/profile.d ]; then
  printf '%s\n' "$PROFILE_SNIPPET" | $SUDO tee /etc/profile.d/android-cloud.sh >/dev/null
fi
if ! grep -q 'ANDROID_HOME=/opt/android-sdk' "$HOME/.bashrc" 2>/dev/null; then
  printf '\n# Android/JDK toolchain (Cloud Agent)\n%s\n' "$PROFILE_SNIPPET" >> "$HOME/.bashrc"
fi

# 5. Let Gradle discover JDK 25 for its daemon toolchain criteria -------------
mkdir -p "$HOME/.gradle"
if ! grep -q 'org.gradle.java.installations.paths' "$HOME/.gradle/gradle.properties" 2>/dev/null; then
  echo 'org.gradle.java.installations.paths=/opt/jdk/current' >> "$HOME/.gradle/gradle.properties"
fi

# 6. Warm Gradle wrapper + dependency caches ---------------------------------
# Soft-fail: agents often edit sources while install is still running, so a
# mid-change compile error must not mark environment setup as INSTALL_FAILED.
# AGENTS.md already has agents run assembleDebug / unit tests themselves.
log "Warming Gradle caches (building debug APK) ..."
chmod +x "$REPO_ROOT/android/gradlew"
if ! ( cd "$REPO_ROOT/android" && ./gradlew :app:assembleDebug ); then
  log "WARNING: assembleDebug failed (non-fatal). JDK/SDK toolchain is ready; agents can rebuild after code changes."
fi

log "Cloud Agent environment ready."
