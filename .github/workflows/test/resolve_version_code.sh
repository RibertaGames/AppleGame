set -eu

if expr "$1" + 1 &>/dev/null; then
    #���l�Ȃ炻�̂܂ܕԂ�
    echo "$1"
else
    expr $(date +%s) /60
fi