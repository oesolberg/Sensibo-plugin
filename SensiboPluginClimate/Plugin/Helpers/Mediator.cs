using System.Collections.Generic;

namespace HSPI_SensiboClimate.Plugin.Helpers
{
	public interface IHandler
	{
		void Handle(IRequest request);
		string Name { get; }
	}

	public interface IRequest
	{

	}

	public interface IEvent:IRequest
	{

	}

	public interface IMediator
	{
		void AddHandler(IHandler objectToAdd);
		void Send(IRequest command, IHandler sender);
	}

	//Mediator class for handling requests
	public class Mediator : IMediator
	{
		private static List<IHandler> _handlers = new List<IHandler>();

		public Mediator()
		{

		}

		public void AddHandler(IHandler objectToAdd)
		{
			if (_handlers.Count > 0 && _handlers.Exists(x => x.Name == objectToAdd.Name))
				return;

			_handlers.Add(objectToAdd);
		}

		public void Send(IRequest command, IHandler sender)
		{
			foreach (var handler in _handlers)
			{
				if (sender == null || handler.Name != sender.Name)
				{
					handler.Handle(command);
				}
			}
		}
	}
	
}