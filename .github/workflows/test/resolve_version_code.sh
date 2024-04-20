set -eu

if expr "$1" + 1 &>/dev/null; then
    #”’l‚È‚ç‚»‚Ì‚Ü‚Ü•Ô‚·
    echo "$1"
else
    expr $(date +%s) /60
fi