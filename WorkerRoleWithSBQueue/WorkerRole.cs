using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WorkerRoleWithSBQueue
{
	public class WorkerRole : RoleEntryPoint
	{
		// The name of your queue
		const string QueueName = "testqueue";

		// QueueClient is thread-safe. Recommended that you cache 
		// rather than recreating it on every request
		QueueClient Client;
		private QueueClient DeadLetterClient;
		ManualResetEvent CompletedEvent = new ManualResetEvent(false);

		public override void Run()
		{
			while (true)
			{
				BrokeredMessage message = Client.Receive(TimeSpan.FromSeconds(10));
				//ToDo
				Thread.Sleep(TimeSpan.FromMilliseconds(5));
			}
		}

		public override bool OnStart()
		{
			// Set the maximum number of concurrent connections 
			ServicePointManager.DefaultConnectionLimit = 12;

			// Create the queue if it does not exist already
			string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
			//var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
			MessagingFactory factory = MessagingFactory.CreateFromConnectionString(connectionString);
			Client = factory.CreateQueueClient(QueueName);

			DeadLetterClient = factory.CreateQueueClient(QueueClient.FormatDeadLetterPath(Client.Path));


			// Initialize the connection to Service Bus Queue
			return base.OnStart();
		}

		public override void OnStop()
		{
			// Close the connection to Service Bus Queue
			Client.Close();
			DeadLetterClient.Close();
			CompletedEvent.Set();
			base.OnStop();
		}
	}
}
