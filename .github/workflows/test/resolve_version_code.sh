set -eu

if expr "$1" + 1 &>/dev/null; then
    #数値ならそのまま返す
    echo "$1"
else
    expr $(date +%s) /60
fi