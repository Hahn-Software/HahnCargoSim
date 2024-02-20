## The setting: 

You are working for a transport company. The company offers you access to their map. This map is composed of **nodes**, representing where pickup and delivery locations are. It has also **edge’s** that describe how long travel time is and how much it costs to use them. And finally, **connections** that tell you what edge connects which two nodes. 
You start with 1000 coins and can also buy your first cargo transporter for free. Additional transporter cost 1000 coins. 
Orders will pop up over time and you can accept them. But be aware, if you fail to deliver them, it will cost you! 
Your job is to automate your cargo transporter – and to make more coins! 

## The challenge: 

We want you to write a frontend in either Angular or React, that allows you to log into the “Hahn Cargo Simulation”. After the login there should be an option to start / stop the simulation and to create Orders (for debug purposes). Also, this frontend should visualize what your transporter automation in the backend is doing. 

When the simulation is started, your program (written in .Net6 or above) should automate everything from accepting orders and moving transporters, to buying new transporters. All that can be done by using the simulations API.
So just hit start and watch your program make coins. 

Your Application should be startable with a docker-compose and checkable. It also should have a repository with a git history.  

Finally, record a video declaring your application (in english).

## The technical background: 

You can see the API over \<your url>/swagger/index.html. 
The endpoint should be self-explainable, but here are some hints:

* The Grid/Get endpoint gives the node, edges and connections. But keep in mind, this grid could represent a handful of warehouses, all airports around the world or all stars in the galaxy. So don’t “optimize” on a special grid. The simulation comes already with three different grids (s. appsettings.json), and we will run your program against a forth one, that you don’t know. Also keep in mind the runtime of your program. You don’t want an order to expire just because you took too long to pick a route. You are allowed to get and analyze the grid before the simulation can be started. 
* The Order/Create Endpoint is for Debug/Testing and should not be called by your automation process. 
* In addition to getting all available orders via the endpoint. New orders will be broadcast over a bus. To use that, RabbitMQ must be installed (https://www.rabbitmq.com). Please make use of it. The queue is "HahnCargoSim_NewOrders". 
* With the CargoTransporter/Move endpoint, you can move to a different node that is directly connected with your current location. You cannot just enter the drop-off location. Picking the route, moving along and maybe changing it midway is part of your automation. 
* When you use the User/Login endpoint, you can pick your own username. But the password is always “Hahn”. You will get a JWT that can be used to access the other endpoints. 
* With the GenerateFile Enpoint for Grid and Orders you can create more test data. But the two enpoints shoud also not be called by your automation process.  

The simulation server should not be changed. It is “legacy” on purpose. But if you think you found a bug, please let us know. 

The repo also contains a postman collection to test the server endpoints. 
