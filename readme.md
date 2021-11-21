# Microservice fun

Store pod id in variable:

```bash
PodId=`kubectl get pods -l app=postgres -o wide | grep -v NAME | awk '{print $1}'`
echo $PodId
```
