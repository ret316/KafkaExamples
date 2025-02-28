using System.Collections.Concurrent;
using CAP.Shared;

namespace CAP.Consumer;

public class CapCollection : ConcurrentBag<CapMessageWithId>
{
}
