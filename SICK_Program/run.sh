#!/bin/bash
while true
do
	./Sick-test
	if [ $? -eq 123 ]
	then 
		echo "Program exit, restart"
        sleep 3
	else
		echo "Program failed, trying restart"
		sleep 3
	fi
done
