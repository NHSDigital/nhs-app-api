#!/bin/sh
#
# An example hook script to verify what is about to be committed.
# Called by "git commit" with no arguments.  The hook should
# exit with non-zero status after issuing an appropriate message if
# it wants to stop the commit.
#
# Kube-score has been removed until Helm3 is in place.

if git rev-parse --verify HEAD >/dev/null 2>&1
then
	against=HEAD
else
	# Initial commit: diff against an empty tree object
	against=$(git hash-object -t tree /dev/null)
fi

# Redirect output to stderr.
exec 1>&2

git stash save -q --keep-index "Precommit stash"

run=$(./pre-commit.d/02-helm-lint.sh)
if [[ $? -eq 1 ]]; then
  git stash pop -q --index
  ./pre-commit.d/02-helm-lint.sh
  echo "Unable to commit. Please fix errors flagged by helm lint."
  exit 1
fi

git stash pop -q --index
exit 0