#!/bin/bash
cd /home/kavia/workspace/code-generation/preventive-maintenance-management-system-176377-176388/backend
dotnet build --no-restore -v quiet -nologo -consoleloggerparameters:NoSummary /p:TreatWarningsAsErrors=false
LINT_EXIT_CODE=$?
if [ $LINT_EXIT_CODE -ne 0 ]; then
  exit 1
fi

