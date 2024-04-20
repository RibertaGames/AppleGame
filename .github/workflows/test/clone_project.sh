set -eu

#name: �r���h�Ώۂ̃��|�W�g�����N���[��
rm -rf app

if ! git clone --depth 1 -b ${branch} https://${GIT_USER}:${GIT_TOKEN}@github.com/${GIT_TEAM}/${repository} app --recursive
then
   echo >&2 git clone failed
   exit 1
fi