export INSIGHT_TEST_HOST=$DOCKER_HOST
if [ ! $INSIGHT_TEST_HOST ]; then export INSIGHT_TEST_HOST=127.0.0.1; fi
export INSIGHT_TEST_PASSWORD=Insight!!!Test

echo $INSIGHT_TEST_HOST
echo $INSIGHT_TEST_PASSWORD
echo Insight docker environment variables set
