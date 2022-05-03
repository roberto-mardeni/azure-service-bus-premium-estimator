from site import execsitecustomize
from diagrams import Cluster, Diagram
from diagrams.azure.integration import ServiceBus
from diagrams.azure.web import AppServicePlans
from diagrams.azure.compute import FunctionApps
from diagrams.azure.storage import TableStorage, QueuesStorage
from diagrams.azure.devops import ApplicationInsights

with Diagram("", filename="diagram", show=False):
    bus = ServiceBus("bus")

    with Cluster("Estimator"):
        estimator = FunctionApps("Estimator")
        orchestration = QueuesStorage("Orchestration")
        state = TableStorage("State")

    # with Cluster("Receiver 1"):
    receiver1 = FunctionApps("Receiver 1")

    # with Cluster("Receiver 2"):
    receiver2 = FunctionApps("Receiver 2")

    receivers = [receiver1, receiver2]

    monitor = ApplicationInsights("monitor")

    estimator >> bus << receivers
    estimator >> [ state, orchestration ]
    [ estimator, receiver1, receiver2 ] >> monitor
        