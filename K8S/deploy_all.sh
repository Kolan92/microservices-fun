for deployment in *.yaml
do
    kubectl apply -f $deployment
done
