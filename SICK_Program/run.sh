#!/bin/bash
while true
do
	./Sick-test
	if [ $? -eq 0 ]
	then 
		echo "Program exit, restart"
                sleep 3
	else
		echo "Program failed, trying restart"
		sleep 3
	fi
done
