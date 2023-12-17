docker-compose up --build -d
echo "waiting for start all services..."
sleep 5s
curl localhost:1234/
curl localhost:1234/test
curl localhost:1234/network
sleep 30s