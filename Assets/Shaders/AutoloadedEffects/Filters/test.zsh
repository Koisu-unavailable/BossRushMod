#!/bin/zsh

# Run MGCB normally, but convert absolute Unix paths in arguments
# to Windows paths before they reach Wine.
args=()

for arg in "$@"; do
    if [[ "$arg" == /* && -e "$arg" ]]; then
        args+=("$(winepath -w "$arg")")
    else
        args+=("$arg")
    fi
done

exec mgcb "${args[@]}"
