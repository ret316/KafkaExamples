using Rebus.Shared;
using System.Collections.Concurrent;

namespace Rebus.Consumer;

public class MessageCollection : ConcurrentBag<RebusMessageWithId>
{
}
