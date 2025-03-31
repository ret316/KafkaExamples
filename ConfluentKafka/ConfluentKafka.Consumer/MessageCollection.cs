using Rebus.Shared;
using System.Collections.Concurrent;

namespace ConfluentKafka.Consumer;

public class MessageCollection : ConcurrentBag<RebusMessageWithId>
{
}
