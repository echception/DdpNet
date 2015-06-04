# Ddp.NET #

Ddp.NET is a library for connecting your .NET applications to a Meteor server.

The documentation assumes you are already familiar with Meteor; if you are not familiar, refer to the [Meteor Documentation](http://docs.meteor.com/).

## Platforms ##

Ddp.NET works on the following platforms:

* .NET 4.5
* Windows 8.1
* Windows Phone 8.1

## Features ##

* All major DDP features are supported: connect to a server, login, create a collection, subscribe to data, modify collections, call server methods, and more
* Collections are strongly typed
* Collections events are handled automatically in the background; Ddp.NET will keep all the collections up to date with the latest events from the server
* Easy integration with WPF/XAML- Ddp.NET collections are ObservableCollections, meaning the UI can easily react to changes, and provides easy integration into existing frameworks

## Installing ##
Easiest method to get started is to use nuget:

    Install-Package DdpNet

## Examples ##

Here are a couple of quick examples of how to perform common Ddp.NET operations:

### Connect to a Meteor server ###


	MeteorClient client = new MeteorClient(new Uri("ws://localhost:3000/websocket"));
	await client.ConnectAsync();

### Create a typed collection ###

	DdpCollection<Post> posts = client.GetCollection<Post>("posts");

### Subscribe ###

	await client.Subscribe("posts");

### Call a server method ###

	await client.Call("someMethodName");

### Login with username & password ###

	await client.LoginPassword("userName", "password");

### Modify a collection ###

	// Add a new item
	Post post = new Post("Github", "http://github.com");
	string id = await posts.AddAsync(post);

	// Update the item with a dictionary of fields
	var fieldsToUpdate = new Dictionary<string, object>()
	{
		{ "Title", "Github Homepage" }
	};
	await posts.UpdateAsync(id, fieldsToUpdate);

	// Or update with another object
	Post updatedPost = new Post("Github2", "http://github.com");
	await posts.UpdateAsync(id, updatedPost);

	// Remove the item
	await posts.RemoveAsync(id);

## Getting Started ##

Head to the [Wiki](https://github.com/echception/DdpNet/wiki) for more information on using Ddp.NET.

The source code also includes a couple of sample applications using Ddp.NET.  One is a WPF application that connects to the Meteor Leaderboard example. The other is a Windows 8.1 application that connects to a Microscope application (the application built in The Meteor Book). 

Building Ddp.NET will require Visual Studio 2013 and >= .NET 4.5.




