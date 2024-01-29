# TraCI.NET

[![Join the chat at https://gitter.im/mennowo/CodingConnected.TraCI](https://badges.gitter.im/mennowo/CodingConnected.TraCI.svg)](https://gitter.im/mennowo/CodingConnected.TraCI?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)

A simple, rather complete implementation of TraCI (see http://sumo.dlr.de/wiki/TraCI) in C#. 

**Usage**

Usage of the library is very straightforward. First, create an instance of the client.

    var client = new TraCIClient();

Now, connect to SUMO.
*Note 1:* This presupposes SUMO or SUMO-GUI has been started with the "--remote-port" option as described in the TraCI documentation.
*Note 2:* This function is implemented asynchronously, to allow a GUI to remain responsive; subsequent API calls are (for now) all synchronous (they should not take a noticable amount of time).

    await client.ConnectAsync("127.0.0.1", 4001);

If using SUMO-GUI, press the play button after the network has loaded, otherwise no TraCI commands will be handled.
We can now run the simulation, for example using a Task:

    Task.Run(() =>
    {
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            Thread.Sleep(100);
            client.ControlSimStep();
        }
    }

This assumes a step size of 0,1 second. The approach is quite rudimentary and does not take into account the call to ControlSimStep might take time.

Browse the public methods off the TraCIClient class to find out which functionality is exposed.
