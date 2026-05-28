#!/bin/env /bin/bash

PROJECT_DIRECTORY=$(git rev-parse --show-toplevel)
GIT_HOOKS_DIRECTORY="${PROJECT_DIRECTORY}/.git/hooks"
HOOKS_DIRECTORY="${PROJECT_DIRECTORY}/hooks"

cp -f ${HOOKS_DIRECTORY}/commit-msg ${GIT_HOOKS_DIRECTORY}/commit-msg
cp -f ${HOOKS_DIRECTORY}/pre-push ${GIT_HOOKS_DIRECTORY}/pre-push
chmod +x ${GIT_HOOKS_DIRECTORY}/commit-msg
chmod +x ${GIT_HOOKS_DIRECTORY}/pre-push
