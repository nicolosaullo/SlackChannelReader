#!/bin/bash

# Slack Channel Reader Docker Runner Script
# Usage: ./run-docker.sh [date_from] [date_to]

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
IMAGE_NAME="slack-channel-reader"
CONTAINER_NAME="slack-reader-$(date +%s)"
OUTPUT_DIR="./slack-archive"
CONFIG_FILE="./appsettings.json"

echo -e "${GREEN}🚀 Slack Channel Reader Docker Runner${NC}"
echo "=================================="

# Check if config file exists
if [ ! -f "$CONFIG_FILE" ]; then
    echo -e "${RED}❌ Configuration file not found: $CONFIG_FILE${NC}"
    echo -e "${YELLOW}💡 Please copy appsettings.example.json to appsettings.json and configure your Slack token${NC}"
    exit 1
fi

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo -e "${RED}❌ Docker is not running. Please start Docker and try again.${NC}"
    exit 1
fi

# Build the image
echo -e "${YELLOW}🔨 Building Docker image...${NC}"
docker build -t $IMAGE_NAME .

# Create output directory
mkdir -p $OUTPUT_DIR

# Prepare arguments
DOCKER_ARGS=""
if [ $# -ge 1 ]; then
    DOCKER_ARGS="$1"
fi
if [ $# -ge 2 ]; then
    DOCKER_ARGS="$DOCKER_ARGS $2"
fi

# Run the container
echo -e "${YELLOW}📦 Running Slack Channel Reader...${NC}"
if [ -n "$DOCKER_ARGS" ]; then
    echo -e "${YELLOW}📅 Date range: $DOCKER_ARGS${NC}"
fi

docker run --rm \
    --name $CONTAINER_NAME \
    -v "$(pwd)/$CONFIG_FILE:/app/appsettings.json:ro" \
    -v "$(pwd)/$OUTPUT_DIR:/app/slack-archive" \
    $IMAGE_NAME $DOCKER_ARGS

echo -e "${GREEN}✅ Archive completed! Check the $OUTPUT_DIR directory for results.${NC}"

# Show output summary
if [ -d "$OUTPUT_DIR" ]; then
    echo -e "${YELLOW}📊 Archive Summary:${NC}"
    find "$OUTPUT_DIR" -name "*.jsonl" -exec wc -l {} + 2>/dev/null | tail -1 | awk '{print "Total messages: " $1}'
    find "$OUTPUT_DIR" -name "*.jsonl" | wc -l | awk '{print "Files created: " $1}'
fi