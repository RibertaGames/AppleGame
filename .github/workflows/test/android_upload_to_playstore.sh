set -eu

if [ "${BUILD_TARGET-}" = "android" ]; then
    source ${PROJECT_PATH}/BUILD_SETTING.sh
    if[ ! -e "$PROJECT_PATH/GooglePlayAccessKey.json" ]; then
        echo >&2 "プロジェクトパスに GooglePlayAccessKey.jsonがありません"
        exit 1
    fi
    fastlane android upload_hyper_casual_dev "aab:$BUILD_OUTPUT" "play_track:$play_track" "json_key:$PROJECT_PATH/GooglePlayAccessKey.json"
fi