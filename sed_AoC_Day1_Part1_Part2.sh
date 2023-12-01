#!/bin/bash
# AoC_2023 Day 1 - sed comand to substitute pattern to get desired output
echo "1one2abcd5" | sed -E '{s/one/o1e/g ;p;  s/[[:alpha:]]//g; p; s/^([[:digit:]])[[:digit:]]+([[:digit:]])$/\1\2/g; }'
