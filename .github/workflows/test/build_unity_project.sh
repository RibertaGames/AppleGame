set -eu

#name: 変数の設定
ENV_OPTION="-env $stage"

#name: ビルド対象の設定
source ${PROJECT_PATH}/BUILD_SETTING.sh
if [ "$BUILD_TARGET" = "ios"]; then
    EXECUTE_METHOD="AppBuilder.BuildForIOS"
elif [ "$BUILD_TARGET" = "android" ]; then
    EXECUTE_METHOD="AppBuilder.BuildForAndroid"
    VERSION_OPTION="-versionCode $version_code"
fi

echo "Project: $PROJECT_PATH"

#name: PATH前処理
BUILD_LOG_PATH=$PROJECT_PATH/log/build.log
if [ $(uname -r | sed -n 's/.*\(*Microsoft *\).*/\1/ip')]; then
    PROJECT_PATH=$(wslpath -w $PROJECT_PATH)

    mkdir -p "$(dirname $BUILD_LOG_PATH)"
    touch "$BUILD_LOG_PATH"
    BUILD_LOG_PATH=$(wslpath -w $BUILD_LOG_PATH)
fi

#name: Unityビルド
rm -rf $BUILD_OUTPUT

#name: WSL用処理
BUILD_OUTPUT_PATH=${BUILD_OUTPUT}
if [[ "$(uname -r)" == *microsoft*]]; then
    mkdir -p ${BUILD_OUTPUT_PATH}
    BUILD_OUTPUT_PATH="$(wslpath -w ${BUILD_OUTPUT})"
fi
echo "出力先: $BUILD_OUTPUT"

"$UNITY_APP" -quit -batchmode -nographics -silent-crashes \
    -projectPath "$PROJECT_PATH" \
    -logFIle "$BUILD_LOG_PATH" \
    -buildTarget $BUILD_TARGET \
    -buildOutput $BUILD_OUTPUT_PATH \
    -executeMethod $EXECUTE_METHOD \
    -username $UNITY_EMAIL -password $UNITY_PASSWORD \
    $ENV_OPTION ${VERSION_OPTION-} ${AAB_OPTION-}

#name: Unity成果物チェック
if [ ! -e $BUILD_OUTPUT ]; then
    echo >&2 Unity build error
    exit 1
fi

echo "Build Complete!"