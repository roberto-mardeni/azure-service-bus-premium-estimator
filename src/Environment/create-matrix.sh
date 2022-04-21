#!/bin/bash
#This script will produce a sequence of numbers separated by comma
declare -a matrix
for i in {1..$1}
do
    matrix+=(i)
done
echo $(echo ${matrix[@]} | tr ' ' ,)