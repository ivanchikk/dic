#!/bin/bash

sourceBranch=${1:-main}

git diff --name-status "$sourceBranch" -- . | grep '^M' | cut -f2 | while read -r file; do
    git restore --source="$sourceBranch" -- "$file"
done