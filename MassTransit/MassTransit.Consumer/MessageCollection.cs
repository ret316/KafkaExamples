using MassTransit.Shared;
using System.Collections.Concurrent;

namespace MassTransit.Consumer;

public class MessageCollection : ConcurrentBag<MassTransitMessageWithId>
{
}
