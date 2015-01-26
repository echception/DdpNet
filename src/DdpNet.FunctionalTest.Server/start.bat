@echo off
start /wait mintty -h error -e vagrant up
start mintty -h error -e vagrant ssh
start mintty -h error -e vagrant rsync-auto
