@echo off
docker-compose -f .\docker-compose.yml up --build --detach
echo "Local environment has started. Press any key to stop and remove the containers"
pause
docker-compose -f .\docker-compose.yml down