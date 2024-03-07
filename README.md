## The setting: 

You are working for a transport company. The company offers you access to their map. This map is composed of **nodes**, representing where pickup and delivery locations are. It has also **edge’s** that describe how long travel time is and how much it costs to use them. And finally, **connections** that tell you what edge connects which two nodes. 
You start with 1000 coins and can also buy your first cargo transporter for free. Additional transporter cost 1000 coins. 
Orders will pop up over time and you can accept them. But be aware, if you fail to deliver them, it will cost you! 
Your job is to automate your cargo transporter – and to make more coins! 

## The challenge: 

We want you to write an application (back- and frontend), that uses the given simulation api. Your backend, written in .net6 or above, should handle the following use cases:
- Receive orders via RabbitMQ and decide on accepting them or not.
- Navigate transporters to fulfill the accepted orders.
- Buy new transporters, when it is beneficial.
 
The backend should implement your strategy to do all the use cases automatically, without required interactions from a user. The overall goal of this strategy is of course to earn coins.
 
Your frontend write in either Angular or React, should allow the user to log into the “Hahn Cargo Simulation”.
After the login there should be an option to start / stop the simulation. (There can be other options for debug purposes e.g. to create Orders.)
When the simulation is started, the frontend should visualize what your automation in the backend is doing. Feel free to display what you think is great for a user, but at a minimum we want to see the updating coin amount, the owned transporter and there status, as well as the accepted orders.
 
Your Application should be start able with a docker-compose and checkable. It also should have a repository with a git history.
 
Finally, record a video declaring your application (in English).
 
Additional remarks:
- Your application will most likely look somewhat like this: [Hahn Server] <-> [your backend] <-> [your frontend]
- We know that there are a lot of packages available. But choose them carefully, since we want to hire you, not the package author.

## The technical background: 

You can see the API over \<your url>/swagger/index.html. 
The endpoint should be self-explainable, but here are some hints:

* The Grid/Get endpoint gives the node, edges and connections. But keep in mind, this grid could represent a handful of warehouses, all airports around the world or all stars in the galaxy. So don’t “optimize” on a special grid. The simulation comes already with three different grids (s. appsettings.json), and we will run your program against a forth one, that you don’t know. Also keep in mind the runtime of your program. You don’t want an order to expire just because you took too long to pick a route. You are allowed to get and analyze the grid before the simulation can be started. 
* The Order/Create Endpoint is for Debug/Testing and should not be called by your automation process. 
* In addition to getting all available orders via the endpoint. New orders will be broadcast over a bus. To use that, RabbitMQ must be installed (https://www.rabbitmq.com). Please make use of it. The queue is "HahnCargoSim_NewOrders". 
* With the CargoTransporter/Move endpoint, you can move to a different node that is directly connected with your current location. You cannot just enter the drop-off location. Picking the route, moving along and maybe changing it midway is part of your automation. 
* When you use the User/Login endpoint, you can pick your own username. But the password is always “Hahn”. You will get a JWT that can be used to access the other endpoints. 
* With the GenerateFile Enpoint for Grid and Orders you can create more test data. But the two enpoints shoud also not be called by your automation process.  

The simulation server should not be changed. It is “legacy” on purpose. But if you think you found a bug, please let us know (best open up an issue here on git). 

The repo also contains a postman collection to test the server endpoints. 
