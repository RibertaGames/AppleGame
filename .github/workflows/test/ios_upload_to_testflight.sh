set -eu

if [ "${BUILD_TARGET-}" = "ios" ]; then
    cp -r "${BUILD_DIR}/fastlane" $BUILD_OUTPUT/.
    cp -p ${PROJECT_PATH}/BUILD_SETTING.sh $BUILD_OUTPUT/.
    cd $BUILD_OUTPUT
    source BUILD_SETTING.sh && fastlane ios_testflight
fi